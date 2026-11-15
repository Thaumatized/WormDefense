using System;
using System.Collections.Generic;
using Godot;

public partial class Player : Node3D
{
	[Export]
	public ulong moveEveryXFrames = 20;

	[Export]
	public PackedScene segmentTemplate;

	private Vector3I direction = Vector3I.Forward;
	private Vector3I previousDirection = Vector3I.Forward;

	// order is head to tail
	private List<SnakeSegment> segments;

	private bool birthing;

	public override void _EnterTree()
	{
		segments = [GetChild<SnakeSegment>(0), GetChild<SnakeSegment>(1)];
		foreach(SnakeSegment segment in segments)
		{
			segment.direction = previousDirection;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Player_1_Move_Left"))
		{
			if(previousDirection.X == 0)
			{
				direction = Vector3I.Left;
			}
		}
		else if (@event.IsActionPressed("Player_1_Move_Right"))
		{
			if(previousDirection.X == 0)
			{
				direction = Vector3I.Right;
			}
		}
		else if (@event.IsActionPressed("Player_1_Move_Forward"))
		{
			if(previousDirection.Z == 0)
			{
				direction = Vector3I.Forward;
			}
		}
		else if (@event.IsActionPressed("Player_1_Move_Back"))
		{
			if(previousDirection.Z == 0)
			{
				direction = Vector3I.Back;
			}
		}
	}

	public override void _PhysicsProcess(double delta){

		if(Engine.GetPhysicsFrames() % moveEveryXFrames == 0)
		{
			previousDirection = direction;
			birthing = false;

			KinematicCollision3D collision = segments[0].MoveAndCollide(previousDirection, true);
			if(collision != null)
			{
				bool proceed = headHit(collision);
				if(!proceed) return;
			}

			int i = segments.Count-1;
			if(segments[i].Pregnant)
			{
				segments[i].Pregnant = false;
				growSegment();
				birthing = true;
			}

			for (; i > 0; i--)
			{
				segments[i].Position = new Vector3(Convert.ToInt32(segments[i].Position.X), segments[i].Position.Y, Convert.ToInt32(segments[i].Position.Z));
				segments[i].direction = segments[i-1].direction;
				segments[i].Pregnant = segments[i-1].Pregnant;
			}
			segments[0].Position = new Vector3(Convert.ToInt32(segments[0].Position.X), segments[0].Position.Y, Convert.ToInt32(segments[0].Position.Z));
			segments[0].direction = previousDirection;
			segments[0].Pregnant = false;
		}
		else
		{
			for (int i = segments.Count - (birthing ? 2 : 1); i >= 0; i--)
			{
				segments[i].Position += ((Vector3)segments[i].direction) * (float)delta * (60f/moveEveryXFrames);
				//segments[i].Pregnant = segments[i-1].Pregnant;
			}
		}
	}

	/*
		returns if we should move anyways
	*/
	public bool headHit(KinematicCollision3D collision) {
		Node3D collidedObject = collision.GetCollider() as Node3D;
		if(collidedObject.Name == "Apple")
		{
			segments[0].Pregnant = true;
			GameManager.instance.SpawnApple();
			collidedObject.QueueFree();
			return true;
		}
		else
		{
			foreach(SnakeSegment segment in segments)
			{
				segment.direction *= -1;
			}
			segments.Reverse();
			direction = previousDirection = segments[0].direction;

			// Move head back to front
			(segments[0], segments[segments.Count-1]) = (segments[segments.Count-1], segments[0]);
			(segments[0].direction, segments[segments.Count-1].direction) = (segments[segments.Count-1].direction, segments[0].direction);
			(segments[0].Position, segments[segments.Count-1].Position) = (segments[segments.Count-1].Position, segments[0].Position);
			(segments[0].Pregnant, segments[segments.Count-1].Pregnant) = (segments[segments.Count-1].Pregnant, segments[0].Pregnant);

			return false;
		}
	}

	private void growSegment()
	{
		SnakeSegment newSegment = segmentTemplate.Instantiate<SnakeSegment>();
		newSegment.Position = segments[segments.Count-1].Position;
		newSegment.direction = segments[segments.Count-1].direction;
		this.AddChild(newSegment);
		segments.Insert(segments.Count-1, newSegment);
	}
}
