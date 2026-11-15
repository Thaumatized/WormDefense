using System.Collections.Generic;
using Godot;

public partial class GameManager : Node
{
	public static GameManager instance;

	[Export]
	public bool randomizeLevel;

	[Export]
	public Node3D tileRoot;

	private Dictionary<Vector2I, Tile> tiles;

    public override void _EnterTree()
    {
		instance = this;

		if(randomizeLevel)
		{
			(tileRoot as RandomLevelGenerator).Randomize();
		}

		foreach (Node child in tileRoot.GetChildren())
		{
			Tile child3D = child as Tile;
			tiles.Add(new Vector2I((int)(child3D.Position.X + 0.5f), (int)(child3D.Position.Z + 0.5f)), child3D);
		}
    }
}