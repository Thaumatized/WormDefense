using System.Collections.Generic;
using Godot;

public struct SnakeLocomotionState {
	public Vector3I integerPosition;
	public Vector3I direction;
	public bool pregnant;
}

public partial class Player : Node3D
{
	[Export]
	public double speed = 1f/3;
	private double movementCounter;

	[Export]
	public PackedScene segmentTemplate;

	private Vector3I previousDirection = Vector3I.Forward;

	// order is head to tail
	private List<SnakeSegment> segments;

	private bool birthing;
	private bool pregnating;
	private bool wasPregnating;

	private static float pregnantScale = 1.2f;

	private double stunTime = 0;

	public List<Vector3I> inputQueue = new List<Vector3I>();

	public override void _EnterTree()
	{
		segments = [GetChild<SnakeSegment>(0), GetChild<SnakeSegment>(1)];
		foreach(SnakeSegment segment in segments)
		{
			segment.locomotionState.direction = previousDirection;
			segment.locomotionState.pregnant = false;
		}
		movementCounter = speed;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Player_1_Move_Left"))
		{
			inputQueue.Add(Vector3I.Left);
		}
		else if (@event.IsActionPressed("Player_1_Move_Right"))
		{
			inputQueue.Add(Vector3I.Right);
		}
		else if (@event.IsActionPressed("Player_1_Move_Forward"))
		{
			inputQueue.Add(Vector3I.Forward);
		}
		else if (@event.IsActionPressed("Player_1_Move_Back"))
		{
			inputQueue.Add(Vector3I.Back);
		}
	}

	public override void _PhysicsProcess(double delta){
		if(stunTime > 0)
		{
			stunTime -= delta;
			return;
		}

		if(movementCounter >= speed)
		{
			wasPregnating = pregnating;
			
			movementCounter -= speed;

			if(inputQueue.Count > 0)
			{
				Vector3I direction = inputQueue[0];
				inputQueue.RemoveAt(0);
				if(
					(previousDirection.X == 0 && direction.X != 0)
					|| (previousDirection.Z == 0 && direction.Z != 0)
					)
				previousDirection = direction;
			}

			birthing = false;
			pregnating = false;

			KinematicCollision3D collision = segments[0].MoveAndCollide(previousDirection, true);
			if(collision != null)
			{
				if(!headHit(collision)) {
					stunTime += speed;
					return;
				}
			}

			int i = segments.Count-1;

			if(segments[i-1].locomotionState.pregnant)
			{
				segments[i-1].locomotionState.pregnant = false;
				growSegment();
				birthing = true;
			}

			for (; i > 0; i--)
			{
				segments[i].locomotionState = segments[i-1].locomotionState;
			}
			
			segments[0].locomotionState.integerPosition += segments[0].locomotionState.direction;
			segments[0].locomotionState.direction = previousDirection;
			segments[0].locomotionState.pregnant = wasPregnating;
		}

		float movementDecimal = (float)(movementCounter / speed);
		for (int i = segments.Count - (birthing ? 2 : 1); i >= 0; i--)
		{
			segments[i].Position = segments[i].locomotionState.integerPosition + ((Vector3)segments[i].locomotionState.direction) * movementDecimal;
			
			bool scaleAfterDeterminator = i == 0 ? pregnating : segments[i-1].locomotionState.pregnant;
			float scaleBefore = segments[i].locomotionState.pregnant ? pregnantScale : 1;
			float scaleAfter = scaleAfterDeterminator ? pregnantScale : 1;

			segments[i].meshInstance3D.Scale = Vector3.One * float.Lerp(scaleBefore, scaleAfter, movementDecimal);
		}

		movementCounter += delta;
	}

	/*
		returns true if we can move
	*/
	public bool headHit(KinematicCollision3D collision) {
		Node3D collidedObject = collision.GetCollider() as Node3D;
		if(collidedObject.Name == "Apple")
		{
			pregnating = true;
			GameManager.instance.SpawnApple();
			collidedObject.QueueFree();
			return true;
		}
		else
		{
			for (int i = segments.Count-1; i >= 0; i--)
			{
				segments[i].locomotionState.integerPosition += segments[i].locomotionState.direction;
				segments[i].locomotionState.direction *= -1;
				segments[i].locomotionState.pregnant = i > 0 ? segments[i-1].locomotionState.pregnant : wasPregnating;
			}

			segments.Reverse();
			inputQueue = new List<Vector3I>();
			previousDirection = segments[0].locomotionState.direction;

			// Move head back to front of list
			(segments[0], segments[segments.Count-1]) = (segments[segments.Count-1], segments[0]);
			(segments[0].locomotionState, segments[segments.Count-1].locomotionState) = (segments[segments.Count-1].locomotionState, segments[0].locomotionState);
			(segments[0].Position, segments[segments.Count-1].Position) = (segments[segments.Count-1].Position, segments[0].Position);

			if(segments[segments.Count-1].locomotionState.pregnant)
			{
				segments[segments.Count-1].locomotionState.pregnant = false;
				growSegment();
				birthing = true;
			}

			return false;
		}
	}

	private void growSegment()
	{
		SnakeSegment newSegment = segmentTemplate.Instantiate<SnakeSegment>();
		newSegment.Position = segments[segments.Count-1].Position;
		newSegment.locomotionState = segments[segments.Count-1].locomotionState;
		this.AddChild(newSegment);
		segments.Insert(segments.Count-1, newSegment);
	}
}
