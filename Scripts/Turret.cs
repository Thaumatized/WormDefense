using System.Collections.Generic;
using Godot;

public partial class Turret : Area3D
{
    public List<Enemy> enemiesInRange = new List<Enemy>();

    [Export]
    public int damage = 1;

    [Export]
    public double fireRate = 1;

    private double firingCounter = 0;

    [Export]
    public Node3D beam;
    [Export]
    public Node3D barrel;

    public override void _EnterTree()
    {
        AreaEntered += seeSomething;
        AreaExited += lostSight;
        base._EnterTree();
    }

    public override void _PhysicsProcess(double delta)
    {
        if(firingCounter < fireRate && enemiesInRange.Count > 0)
        {
            firingCounter += delta;
        }

        if(firingCounter >= fireRate && enemiesInRange.Count > 0)
        {
            firingCounter -= fireRate;

            Enemy targetEnemy = enemiesInRange[0];

            targetEnemy.takeDamage(damage);

            barrel.LookAt(targetEnemy.GlobalPosition);

            beam.Scale = new Vector3(1, 1, (targetEnemy.GlobalPosition - GlobalPosition).Length());
            beam.LookAt(targetEnemy.GlobalPosition);
            beam.Visible = true;
        }
        else
        {
            beam.Visible = false;
        }

        base._PhysicsProcess(delta);
    }

    public void seeSomething(Node3D body){
        if(body.GetType().Name == "Enemy")
        {
            enemiesInRange.Add(body as Enemy);
        }
    }
    public void lostSight(Node3D body){
        enemiesInRange.Remove(body as Enemy);
    }
}