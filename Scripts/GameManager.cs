using System.Collections.Generic;
using Godot;

public partial class GameManager : Node
{
	public static GameManager instance;

	[Export]
	public bool randomizeLevel;

	[Export]
	public Node3D tileRoot;

	private Dictionary<Vector2I, Tile> tilesByCoordinates;
	private List<Tile> tiles;

	[Export]
	public PackedScene Apple;

	[Export]
	public SpawnWave[] spawnWaves;

	public int waveIndex = -1;
	public SpawnWave wave;
	private double waveSpendCounter = 0;

	public List<Tile> enemySpawnTiles = new List<Tile>();


    public override void _PhysicsProcess(double delta)
    {		
		if(wave == null || (wave.budget < wave.cheapestSpawnable && Enemy.enemies.Count == 0))
		{
			startWave();
		}

		waveSpendCounter += delta;

		if(waveSpendCounter > 0 && wave.budget >= wave.cheapestSpawnable)
		{
			SpawnEnemy();
		}

        base._PhysicsProcess(delta);
    }

    public override void _EnterTree()
    {
		instance = this;

		if(randomizeLevel)
		{
			(tileRoot as RandomLevelGenerator).Randomize();
		}

		tilesByCoordinates = new Dictionary<Vector2I, Tile>();
		tiles = new List<Tile>();
		foreach (Node child in tileRoot.GetChildren())
		{
			Tile tile = child as Tile;
			tilesByCoordinates[new Vector2I((int)(tile.Position.X + 0.5f), (int)(tile.Position.Z + 0.5f))] = tile;
			tiles.Add(tile);

			if(tile.enemySpawn)
			{
				enemySpawnTiles.Add(tile);
			}
		}
    }

	public void SpawnApple()
	{
		Tile freeTile = null;
		while(freeTile == null || freeTile.TileObject != null)
		{
			freeTile = tiles[(int)(GD.Randi() % tiles.Count)];
		}
		Node3D newapple = Apple.Instantiate<Node3D>();
		newapple.Position = Vector3.Up * 0.5f;
		freeTile.AddChild(newapple);
		freeTile.TileObject = newapple;
	}


	public void startWave()
	{
		waveIndex++;
		if(waveIndex < spawnWaves.Length)
		{
			GD.Print("Starting Wave ", waveIndex);
			wave = spawnWaves[waveIndex];
			wave.cheapestSpawnable = int.MaxValue;
			foreach(SpawnWaveSpawnable spawnable in wave.spawnables)
			{
				if(spawnable.cost < wave.cheapestSpawnable)
				{
					wave.cheapestSpawnable = spawnable.cost;
				}
			}
			waveSpendCounter = 0;
		}
		else
		{
			GD.Print("WINNER WINNER CHICKEN DINNER");
		}
	}

	public void SpawnEnemy()
	{
		Tile spawnTile = enemySpawnTiles[(int)(GD.Randi() % enemySpawnTiles.Count)];
		SpawnWaveSpawnable spawnable;
		do {
			spawnable = wave.spawnables[(int)(GD.Randi() % wave.spawnables.Length)];
		}
		while(spawnable.cost > wave.budget);

		Node3D newEnemy = spawnable.prefab.Instantiate<Node3D>();
		newEnemy.Position = spawnTile.TileObject.GlobalPosition;
		this.AddChild(newEnemy);

		waveSpendCounter -= spawnable.cost;
		wave.budget -= spawnable.cost;
	}
}