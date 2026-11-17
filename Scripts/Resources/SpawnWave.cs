using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class SpawnWave : Resource
{
    [Export]
    public int budget;
    [Export]
    public float spendingRate;
    [Export]
    public SpawnWaveSpawnable[] spawnables;
    
    public int cheapestSpawnable;
}