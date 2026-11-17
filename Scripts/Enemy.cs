using System.Collections.Generic;
using System.ComponentModel;
using Godot;

public partial class Enemy : Area3D
{
	public static List<Enemy> enemies = new List<Enemy>();

	[Export]
	public float speed = 1;

	private DefenceTarget target;

    public override void _EnterTree()
    {
		enemies.Add(this);
		BodyEntered+=EnterHandler;

        base._EnterTree();
    }

	public void Destroy()
	{
		enemies.Remove(this);
		QueueFree();
	}

    public override void _PhysicsProcess(double delta)
    {
		if(target == null || !IsInstanceValid(target))
		{
			List<DefenceTarget> potentialTargets = DefenceTarget.defenceTargets;
			float lowestDistance = float.MaxValue;
			DefenceTarget closestTarget = null;
			for(int i = 0; i < potentialTargets.Count; i++)
			{
				float distance = (potentialTargets[i].GlobalPosition - Position).LengthSquared();
				if(distance < lowestDistance)
				{
					lowestDistance = distance;
					closestTarget = potentialTargets[i];
				}
			}
			target = closestTarget;
		}
		else
		{
			Vector3 motion = target.GlobalPosition - Position;
			motion.Y = 0;
			motion = motion.Normalized();
			motion *=  speed * (float)delta;

			Position += motion;
		}

        base._PhysicsProcess(delta);
    }

	private void EnterHandler(Node3D body)
	{
		switch(body.GetType().Name)
		{
			case "DefenceTarget":
				DefenceTarget hitTarget = body as DefenceTarget;
				hitTarget.destroy();
				break;
			case "SnakeSegment":
				Destroy();
				break;
		}
	}
}
