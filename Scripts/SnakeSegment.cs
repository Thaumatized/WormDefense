using Godot;

public partial class SnakeSegment : RigidBody3D
{
	
	[Export]
	public MeshInstance3D meshInstance3D;

	public Player player;

	public SnakeLocomotionState locomotionState;
}
