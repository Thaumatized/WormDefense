using Godot;

[GlobalClass]
public partial class WeightedScene : Resource
{
    [Export]
    public PackedScene prefab;

    [Export]
    public float weight;
}