using Godot;

public partial class Tile : Node3D
{
    [Export]
    public Node3D TileObject;

    [Export]
    public bool enemySpawn;
}