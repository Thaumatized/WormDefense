using Godot;

public partial class RandomLevelGenerator : Node3D
{
	[Export]
	WeightedScene[] tilePrefabs;

	[Export]
	PackedScene safeTile;
	[Export]
	PackedScene wallTile;

	[Export]
	Vector2I Area;

	[Export]
	Vector2I StartingArea;

    public void Randomize()
    {
		NormalizeWeights();
		
		/* 
			The -Vector2I.One is flipped for the area defining vectors to add and edge of 1 tile
			which we will fill with walls to define a limited play area.
		*/
		Vector2I minimumPoint = -Area / 2 - Vector2I.One;
		Vector2I maximumPoint = minimumPoint + Area;

		Vector2I startingMinimumPoint = -StartingArea / 2;
		Vector2I startingMaximumPoint = startingMinimumPoint + StartingArea- Vector2I.One;

		for(int x = minimumPoint.X; x <= maximumPoint.X; x++)
		{
			for(int z = minimumPoint.Y; z <= maximumPoint.Y; z++)
			{
				PackedScene prefab;

				if(x >= startingMinimumPoint.X && x <= startingMaximumPoint.X
					&& z >= startingMinimumPoint.Y && z <= startingMaximumPoint.Y)
				{
					prefab = safeTile;
				}
				else if (x == minimumPoint.X || x == maximumPoint.X
					|| z == minimumPoint.Y || z == maximumPoint.Y)
				{
					prefab = wallTile;
				}
				else
				{
					float randomFloat = GD.Randf();
					int index = 0;
					/*
						The index check is to handle floating point precision errors when the random float is 1.
						Should that happen we just use the last item. Technically this makes the last item slightly more common,
						but the amount is negligible.
					*/
					while(randomFloat > tilePrefabs[index].weight && index < tilePrefabs.Length - 1)
					{
						randomFloat -= tilePrefabs[index].weight;
						index++;
					}

					prefab = tilePrefabs[index].prefab;
				}

				Node3D newTile = prefab.Instantiate<Node3D>();
				newTile.Position = new Vector3(x, 0, z);
				this.AddChild(newTile);
			}
		}
    }

	/*
		Normalizes tile weights to total 1. This makes it easy to pick random ones with the weight as GD.Randf returns 0-1
	*/
	private void NormalizeWeights()
	{
		float tilePrefabsTotalWeight = 0;
		for (int i = 0; i < tilePrefabs.Length; i++)
		{
			tilePrefabsTotalWeight += tilePrefabs[i].weight;
		}
		for (int i = 0; i < tilePrefabs.Length; i++)
		{
			tilePrefabs[i].weight /= tilePrefabsTotalWeight;
		}
	}
}