using Raylib_cs;
using System.Numerics;

class RectangleRenderer:Component
{
    public Color color = Color.Blue;
    public Vector2 size = new (100,100);

    public override void Render()
    {
        var go = gameObject;
        Library.DrawRectangle(go.position, go.degrees, size*go.scale, color);
    }
}

class EllipseRenderer:Component
{
    public Color color = Color.Blue;
    public Vector2 size = new (100,100);

    public override void Render()
    {
        var go = gameObject;
        Library.DrawEllipse(go.position, go.degrees, size*go.scale, 20, color);
    }
}