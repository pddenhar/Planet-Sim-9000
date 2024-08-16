using Godot;
using System;
using pddenhar.Planets.Logic;

public partial class Moon : DraggablePolygon
{
	private Vector2 _initialPos;
	// speed unit m/s 
	[Export]
	public Vector2 Speed { get; set; } = new Vector2(0, 0);
	// mass unit kg
	[Export]
	public float Mass = 2.0F;

	public Vector2 Force = Vector2.Zero;
	// Spring constant for dragging
	private float _k;
	// Damping constant for dragging
	private float _c;

	private DebugVector _forceVector;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_k = Mass / 2;
		_c = Mass / 20;
		_initialPos = Position;
		_forceVector = new DebugVector();
		AddChild(_forceVector);
	}

	// Called at 60hz
	public override void _PhysicsProcess(double delta)
	{
		if(IsDragging)
			Force += HandleDrag();
		
		Vector2 acceleration = Force / Mass;
		Speed += acceleration * (float)delta;
		Position += Speed * (float)delta;
		
		_forceVector.DrawVector = Force;
		
		// Return force to zero now that it has been consumed for this delta
		Force = Vector2.Zero;
	}
	
	protected Vector2 HandleDrag()
	{
		if (IsDragging)
		{
			Vector2 TargetPosition = GetGlobalMousePosition() - _dragOffset;
			Vector2 PositionDelta = TargetPosition - GlobalPosition;

			Vector2 SpringForce = PositionDelta * _k;

			float RelativeSpeed = PositionDelta.Normalized().Dot(Speed);
			Vector2 DamperForce = _k * RelativeSpeed * PositionDelta.Normalized();
			GD.Print($"Damper Force: {DamperForce} Spring Force: {SpringForce}");
			return SpringForce - DamperForce;
		}

		return Vector2.Zero;
	}
}
