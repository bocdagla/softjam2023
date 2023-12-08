using Godot;
using System;

public partial class Bird : Node3D
{

	[Export] public BirdSky BirdSky;

	private Node3D seekNode;
	
	private float Speed = 5f;
	private Vector3 direction_forward = new Vector3(0, 0, 1);
	private float lastDistance = 99999f;
	
	public override void _Ready()
	{
		Node3D firstNode = BirdSky.getRandomNode();
		GlobalTransform = firstNode.GlobalTransform;
		NextPath();
	}

	private void NextPath()
	{
		seekNode = BirdSky.getRandomNode();
		lastDistance = GlobalPosition.DistanceTo(seekNode.GlobalPosition);
		LookAt(seekNode.GlobalPosition);
	}

	public override void _PhysicsProcess(double delta)
	{
		Translate(direction_forward.Normalized() * Speed * (float) delta);
		float newDistance = GlobalPosition.DistanceTo(seekNode.GlobalPosition);
		if (newDistance > lastDistance)
		{
			NextPath();
		}
		else
		{
			lastDistance = newDistance;
		}
	}
}
