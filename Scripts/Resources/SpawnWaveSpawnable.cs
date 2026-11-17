using Godot;

[GlobalClass]
public partial class SpawnWaveSpawnable : Resource
{
    [Export]
    public PackedScene prefab;

    [Export]
    public float weight;

    [Export]
    public int cost;
}