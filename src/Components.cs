using Raylib_cs;

class RectangleRenderer:Component
{
    public Color color = Color.Black;

    public override void Render()
    {
        var go = gameObject;
        Raylib.DrawRectangle(
            (int)(go.position.X - go.scale.X), 
            (int)(go.position.Y - go.scale.Y), 
            (int)(go.scale.X*2), 
            (int)(go.scale.Y*2), 
            color);
    }
}