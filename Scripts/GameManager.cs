using System.Collections.Generic;
using System.Linq;
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
}