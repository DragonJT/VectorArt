using Raylib_cs;
using System.Numerics;

class RectangleRenderer:Component
{
    public Color color = Color.Blue;
    public Vector2 size = new (100,100);

    public override void Render()
    {
        var go = gameObject;
        Library.DrawRectangle(Matrix3x2.CreateScale(size.X, size.Y) * go.WorldMatrix(), color);
    }
}

class EllipseRenderer:Component
{
    public Color color = Color.Blue;
    public Vector2 size = new (100,100);

    public override void Render()
    {
        var go = gameObject;
        Library.DrawEllipse(Matrix3x2.CreateScale(size.X, size.Y) * go.WorldMatrix(), 20, color);
    }
}