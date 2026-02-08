

using System.Numerics;
using Raylib_cs;

class Gizmo
{
    bool dragging = false;

    public void Update(GameObject gameObject, Camera2D camera)
    {
        if(gameObject == null)
        {
            return;
        }
        var matrix = Matrix3x2.Identity;
        if (gameObject.parent != null)
        {
            matrix = gameObject.parent.WorldMatrix();
        }
        Matrix3x2.Invert(matrix, out var imatrix);
        var pos = Vector2.Transform(gameObject.position, matrix);
        var radius = 10;
        var rect = new Rectangle(pos.X - radius, pos.Y - radius, radius*2, radius*2);
        Raylib.DrawRectangleRec(rect, Color.Red);
        MouseOver.TrySetMouseOver(rect, camera, this);
        if(MouseOver.current == this && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            dragging = true;
        }
        if (dragging)
        {
            var pout = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
            gameObject.position = Vector2.Transform(pout, imatrix);
        }
        if (Raylib.IsMouseButtonUp(MouseButton.Left))
        {
            dragging = false;
        }
    }
}