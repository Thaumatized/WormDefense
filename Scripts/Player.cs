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

	private List<AnimatableBody3D> segments;
	private List<Vector3> previousDirectionList;

	public override void _EnterTree()
	{
		segments = [GetChild<AnimatableBody3D>(0), GetChild<AnimatableBody3D>(1)];
		previousDirectionList = [previousDirection, previousDirection];
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

			previousDirectionList.RemoveAt(0);
			previousDirectionList.Add(previousDirection);

			for (int i = segments.Count-1; i > 0; i--)
			{
				segments[i].Position = segments[i-1].Position;
			}
			segments[0].Position += previousDirection;
		}
	}

	/*
		returns if we should move anyways
	*/
	public bool headHit(KinematicCollision3D collision) {
		Node3D collidedObject = collision.GetCollider() as Node3D;
		if(collidedObject.Name == "Apple")
		{
			growSegment();
			GameManager.instance.SpawnApple();
			collidedObject.QueueFree();
			return true;
		}
		else
		{
			int headIndex = 0;
			int tailIndex = segments.Count-1;
			while(headIndex < tailIndex)
			{
				Vector3 pos = segments[headIndex].Position;
				segments[headIndex].Position = segments[tailIndex].Position;
				segments[tailIndex].Position = pos;
				headIndex++;
				tailIndex--;
			}

			for(int i = 0; i < previousDirectionList.Count; i++)
			{
				previousDirectionList[i] *= -1;
			}
			direction = previousDirection = previousDirectionList[0];
			previousDirectionList.Reverse();

			return false;
		}
	}

	private void growSegment()
	{
		AnimatableBody3D newSegment = segmentTemplate.Instantiate<AnimatableBody3D>();
		newSegment.Position = segments[segments.Count-2].Position;
		this.AddChild(newSegment);
		segments.Insert(segments.Count-1, newSegment);
		previousDirectionList.Insert(0, previousDirectionList[0]);
	}
}
