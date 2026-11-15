using System;
using System.Collections.Generic;
using Godot;

public partial class Player : Node3D
{
	[Export]
	public ulong moveEveryXFrames = 20;

	[Export]
	public PackedScene segmentTemplate;

	private Vector3 direction = Vector3.Forward;
	private Vector3 previousDirection = Vector3.Forward;

	// order is head to tail
	private List<SnakeSegment> segments;

	public override void _EnterTree()
	{
		segments = [GetChild<SnakeSegment>(0), GetChild<SnakeSegment>(1)];
		foreach(SnakeSegment segment in segments)
		{
			segment.previousDirection = previousDirection;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Player_1_Move_Left"))
		{
			if(previousDirection.X == 0)
			{
				direction = Vector3.Left;
			}
		}
		else if (@event.IsActionPressed("Player_1_Move_Right"))
		{
			if(previousDirection.X == 0)
			{
				direction = Vector3.Right;
			}
		}
		else if (@event.IsActionPressed("Player_1_Move_Forward"))
		{
			if(previousDirection.Z == 0)
			{
				direction = Vector3.Forward;
			}
		}
		else if (@event.IsActionPressed("Player_1_Move_Back"))
		{
			if(previousDirection.Z == 0)
			{
				direction = Vector3.Back;
			}
		}
	}

	public override void _PhysicsProcess(double delta){

		if(Engine.GetPhysicsFrames() % moveEveryXFrames == 0)
		{
			previousDirection = direction;

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
			}
			else
			{
				segments[i].Position = segments[i-1].Position;
				segments[i].previousDirection = segments[i-1].previousDirection;
				segments[i].Pregnant = segments[i-1].Pregnant;
				i--;
			}

			for (; i > 0; i--)
			{
				segments[i].Position = segments[i-1].Position;
				segments[i].previousDirection = segments[i-1].previousDirection;
				segments[i].Pregnant = segments[i-1].Pregnant;
			}
			segments[0].Position += previousDirection;
			segments[0].previousDirection = previousDirection;
			segments[0].Pregnant = false;
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
				segment.previousDirection *= -1;
			}
			segments.Reverse();
			direction = previousDirection = segments[0].previousDirection;

			// Move head back to front
			(segments[0], segments[segments.Count-1]) = (segments[segments.Count-1], segments[0]);
			(segments[0].previousDirection, segments[segments.Count-1].previousDirection) = (segments[segments.Count-1].previousDirection, segments[0].previousDirection);
			(segments[0].Position, segments[segments.Count-1].Position) = (segments[segments.Count-1].Position, segments[0].Position);
			(segments[0].Pregnant, segments[segments.Count-1].Pregnant) = (segments[segments.Count-1].Pregnant, segments[0].Pregnant);

			return false;
		}
	}

	private void growSegment()
	{
		SnakeSegment newSegment = segmentTemplate.Instantiate<SnakeSegment>();
		newSegment.Position = segments[segments.Count-1].Position;
		newSegment.previousDirection = segments[segments.Count-1].previousDirection;
		this.AddChild(newSegment);
		segments.Insert(segments.Count-1, newSegment);
	}
}
