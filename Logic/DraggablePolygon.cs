using Godot;

namespace pddenhar.Planets.Logic;

public partial class DraggablePolygon : Polygon2D
{
    protected bool IsDragging;
    protected Vector2 DragOffset;

    public override void _Input(InputEvent @event)
    {
        // Check for a mouse button press
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButtonEvent)
        {
            if (mouseButtonEvent.Pressed && IsMouseOver())
            {
                // Start dragging
                IsDragging = true;
                // Calculate the offset between the mouse and the polygon's position
                DragOffset = GetGlobalMousePosition() - GlobalPosition;
                GD.Print($"Dragging {Name}. Drag offset: {DragOffset}");
            }
            else
            {
                // Stop dragging when the mouse button is released
                IsDragging = false;
            }
        }
    }
	
    private bool IsMouseOver()
    {
        // Check if the mouse is over the polygon
        Vector2 localClick = ToLocal(GetGlobalMousePosition());
        return Polygon.Length > 2 && Geometry2D.IsPointInPolygon(localClick, Polygon);
    }
}