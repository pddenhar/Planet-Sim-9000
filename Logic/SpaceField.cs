using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace pddenhar.Planets.Logic;

public partial class SpaceField : Node2D
{
	private List<Moon> _moons;
	private const float G = 6.673e-11F; // m^3 / (kg*s^2)
	private RandomNumberGenerator _rng = new();
	
	private const float InnerRadius = 50f;
	private const float OuterRadius = 200f;

	private const float MaxMass = 10000f;
	
	private const int StartingPlanets = 100;

	private float _lastPhysicsTime = 0;
	private Label _physicsTimeLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize the list
		_moons = new List<Moon>();
		
		Button addPlanetButton = GetNode<Button>("CanvasLayer/ButtonContainer/AddPlanetButton");
		// New idiomatic way of connecting to signals in Godot
		addPlanetButton.ButtonUp += OnAddPlanetButtonPressed;
		
		_physicsTimeLabel = GetNode<Label>("CanvasLayer/ButtonContainer/PhysicsTimeLabel");

		// Get all the child nodes and check if they are of type Moon
		foreach (Node child in GetChildren())
		{
			if (child is Moon moon)
			{
				_moons.Add(moon);
			}
		}

		// Now you can use the moons list for further logic
		GD.Print($"Found {_moons.Count} moons.");

		for (int i = 0; i < StartingPlanets; i++)
		{
			AddRandomPlanet();
		}
	}

	private void OnAddPlanetButtonPressed()
	{
		AddRandomPlanet();
	}

	private void AddRandomPlanet()
	{
		Moon centralPlanet = GetNode<Moon>("Planet");

		float orbitRadius = _rng.RandfRange(InnerRadius, OuterRadius);
		float theta = _rng.RandfRange(0, 2*Mathf.Pi);
		float targetVelocity = Mathf.Sqrt((G * centralPlanet.Mass / orbitRadius));
		float sizePct = _rng.RandfRange(0.2f, 1f);
		float newMass = sizePct * MaxMass;
		
		GD.Print($"Creating planet at radius {orbitRadius}. Theta: {theta}, Velocity: {targetVelocity}, Mass: {newMass}");
		Vector2 newPosition = CalculatePosition(centralPlanet.GlobalPosition, theta, orbitRadius);
		GD.Print($"Main planet is at position: {centralPlanet.GlobalPosition}. Creating moon at position: {newPosition}");
		Moon newMoon = (Moon)centralPlanet.Duplicate();
		newMoon.GlobalPosition = newPosition;
		newMoon.Scale = new Vector2(sizePct/10, sizePct/10);
		newMoon.Mass = newMass;
		newMoon.Speed = targetVelocity*Vector2.FromAngle(theta+Mathf.Pi/2);
		_moons.Add(newMoon);
		AddChild(newMoon);
	}
	
	private Vector2 CalculatePosition(Vector2 originalPosition, float theta, float targetRadius)
	{
		// Calculate the x and y offsets
		float offsetX = targetRadius * Mathf.Cos(theta);
		float offsetY = targetRadius * Mathf.Sin(theta);

		// Calculate the new position
		return originalPosition + new Vector2(offsetX, offsetY);
	}

	public override void _Process(double delta)
	{
		_physicsTimeLabel.Text = $"{_lastPhysicsTime} ms";
	}

	public override void _PhysicsProcess(double delta)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		_DoNBody();
		stopwatch.Stop();
		//GD.Print($"DoNBody executed in {stopwatch.ElapsedMilliseconds} ms");
		_lastPhysicsTime = stopwatch.ElapsedMilliseconds;
	}

	private void _DoNBody()
	{
		// Update the speed of each moon based on gravity
		for (int i = 0; i < _moons.Count; i++)
		{
			for (int j = i + 1; j < _moons.Count; j++)
			{
				Moon moon = _moons[i];
				Moon otherMoon = _moons[j];
				//GD.Print($"Processing gravity for {moon.Name} and {otherMoon
				//.Name}");

				// Calculate the direction from the moon to the planet (center)
				Vector2 direction = otherMoon.GlobalPosition - moon.GlobalPosition;

				// Calculate the distance squared (to avoid computing square root for efficiency)
				float distanceSquared = direction.LengthSquared();

				// Ensure the distance isn't too small to avoid extreme forces
				if (distanceSquared < 100.0f)
				{
					//GD.Print($"Distance between {moon.Name} and {otherMoon.Name} too small. Skipping.");
					break;
				}

				// Normalize the direction vector
				direction = direction.Normalized();

				// Calculate the gravitational force magnitude (F = G * (m1 * m2) / r^2)
				float forceMagnitude = G * (otherMoon.Mass * moon.Mass) / distanceSquared;
				//GD.Print($"Force Magnitude: {forceMagnitude} between {moon.Name} and {otherMoon.Name} at distance of {Math.Sqrt(distanceSquared)} m.");
				moon.Force += forceMagnitude * direction;
				otherMoon.Force -= forceMagnitude * direction;
			}
		}
	}
}
