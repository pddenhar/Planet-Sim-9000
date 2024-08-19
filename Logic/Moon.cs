using Godot;

namespace pddenhar.Planets.Logic;

public partial class Moon : DraggablePolygon
{
	private Vector2 _initialPos;
	// speed unit m/s 
	public Vector2 Speed { get; set; } = new Vector2(0, 0);
	// mass unit kg
	[Export]
	public float Mass = 2.0F;

	public Vector2 Force = Vector2.Zero;
	// Spring constant for dragging
	private float _k;
	// Damping constant for dragging
	private float _c;

	private DebugVector _forceVector = new DebugVector();
	private DebugVector _speedVector = new DebugVector();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_k = Mass / 2;
		_c = Mass / 20;
		_initialPos = Position;

		AddChild(_forceVector);
		_speedVector.DrawColor = Colors.Red;
		AddChild(_speedVector);
	}

	// Called at 60hz
	public override void _PhysicsProcess(double delta)
	{
		if(IsDragging)
			Force += HandleDrag();
		
		Vector2 acceleration = Force / Mass;
		Speed += acceleration * (float)delta;
		Position += Speed * (float)delta;
		
		_forceVector.DrawVector = Force/1000;
		_speedVector.DrawVector = Speed;
		
		// Return force to zero now that it has been consumed for this delta
		Force = Vector2.Zero;
	}
	
	private Vector2 HandleDrag()
	{
		if (IsDragging)
		{
			Vector2 targetPosition = GetGlobalMousePosition() - DragOffset;
			Vector2 positionDelta = targetPosition - GlobalPosition;

			Vector2 springForce = positionDelta * _k;

			float relativeSpeed = positionDelta.Normalized().Dot(Speed);
			Vector2 damperForce = _k * relativeSpeed * positionDelta.Normalized();
			//GD.Print($"Damper Force: {DamperForce} Spring Force: {SpringForce}");
			return springForce - damperForce;
		}

		return Vector2.Zero;
	}
}
