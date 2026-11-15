using Godot;

public partial class SnakeSegment : RigidBody3D
{
	public Vector3I direction;
	public bool Pregnant {
	get;
	set
	{
		field = value;
		this.Scale = Vector3.One * (value ? 1.2f : 1f);
		}
	}
}
