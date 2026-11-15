using Godot;

public partial class SnakeSegment : RigidBody3D
{
	public Vector3I direction;
	public bool pregnant;
	[Export]
	public MeshInstance3D meshInstance3D;
}
