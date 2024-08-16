using Godot;
using System;
using System.Collections.Generic;

public partial class planets : Node2D
{
	private List<Moon> moons;
	private const float G = 9.8F; // m/s
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize the list
		moons = new List<Moon>();

		// Get all the child nodes and check if they are of type Moon
		foreach (Node child in GetChildren())
		{
			if (child is Moon moon)
			{
				moons.Add(moon);
			}
		}

		// Now you can use the moons list for further logic
		GD.Print($"Found {moons.Count} moons.");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Update the speed of each moon based on gravity
		for (int i = 0; i < moons.Count; i++)
		{
			for (int j = i + 1; j < moons.Count; j++)
			{
				Moon moon = moons[i];
				Moon othermoon = moons[j];
				//GD.Print($"Processing gravity for {moon.Name} and {othermoon.Name}");

				// Calculate the direction from the moon to the planet (center)
				Vector2 direction = othermoon.GlobalPosition - moon.GlobalPosition;

				// Calculate the distance squared (to avoid computing square root for efficiency)
				float distanceSquared = direction.LengthSquared();

				// Ensure the distance isn't too small to avoid extreme forces
				if (distanceSquared < 100.0f)
				{
					GD.Print($"Distance between {moon.Name} and {othermoon.Name} too small. Skipping.");
					break;
				}

				// Normalize the direction vector
				direction = direction.Normalized();

				// Calculate the gravitational force magnitude (F = G * (m1 * m2) / r^2)
				float forceMagnitude = G * (othermoon.Mass * moon.Mass) / distanceSquared;
				moon.Force += forceMagnitude * direction;
				othermoon.Force -= forceMagnitude * direction;
			}
		}
	}
}
