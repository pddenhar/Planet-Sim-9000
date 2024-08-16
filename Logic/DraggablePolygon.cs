using Godot;
using System;

namespace pddenhar.Planets.Logic;

public partial class DraggablePolygon : Polygon2D
{
    protected bool IsDragging = false;
    protected Vector2 _dragOffset;

    public override void _Input(InputEvent @event)
    {
        // Check for a mouse button press
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseButtonEvent.Pressed && IsMouseOver())
                {
                    // Start dragging
                    IsDragging = true;
                    // Calculate the offset between the mouse and the polygon's position
                    _dragOffset = GetGlobalMousePosition() - GlobalPosition;
                    GD.Print($"Dragging {Name}. Drag offset: {_dragOffset}");
                }
                else
                {
                    // Stop dragging when the mouse button is released
                    IsDragging = false;
                }
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