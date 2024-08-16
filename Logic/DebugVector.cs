using Godot;

namespace pddenhar.Planets.Logic;

public partial class DebugVector : Node2D
{
    private Vector2 _drawVector;

    public Vector2 DrawVector
    {
        get => _drawVector;
        set
        {
            _drawVector = value;
            QueueRedraw();
        }
    }

    public Color DrawColor = Colors.Aqua;

    public override void _Draw()
    {
        DrawLine(Vector2.Zero, _drawVector, DrawColor, 2f);
        //GD.Print($"Draw called, DrawVector: {_drawVector}");
    }
}