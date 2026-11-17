using System.Collections.Generic;
using Godot;

public partial class DefenceTarget : StaticBody3D
{
	public static List<DefenceTarget> defenceTargets = new List<DefenceTarget>();

	[Export]
	public int health = 10;

	public override void _EnterTree()
	{
		defenceTargets.Add(this);
		base._EnterTree();
	}

	public void destroy() {
		defenceTargets.Remove(this);
		QueueFree();
	}
}
