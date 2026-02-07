using System.Numerics;
using Raylib_cs;

static class Library
{
    public const int fontSize = 35;
    public const int lineSize = 45;
    public const int treeIndentSize = 80;
    public const int contextMenuWidth = 400;
    public const int spacing = 3;

    public static Rectangle GrowX(this Rectangle rect, int x)
    {
        return new Rectangle(rect.X - x, rect.Y, rect.Width + x * 2, rect.Height);
    }

    public static bool IsKeyPressed(KeyboardKey key)
    {
        return Raylib.IsKeyPressed(key) || Raylib.IsKeyPressedRepeat(key);
    }

    public static void UpdateText(ref string text)
    {
        int key = Raylib.GetCharPressed();
        while(key > 0)
        {
            text += (char)key;
            key = Raylib.GetCharPressed();
        }
        if (IsKeyPressed(KeyboardKey.Backspace))
        {
            if(text.Length > 0)
            {
                text = text[..^1];
            }
        }
    }

    static void DrawShape(Vector2[] points, Color color)
    {
        for(var i = 2; i < points.Length; i++)
        {
            Raylib.DrawTriangle(points[0], points[i], points[i-1], color);
        }
    }

    static Vector2[] TransformPoints(Matrix3x2 matrix, Vector2[] points)
    {
        return [.. points.Select(p=>Vector2.Transform(p, matrix))];
    }

    static float DegreesToRadians(float degrees)
    {
        return degrees * MathF.PI/180;
    }

    static Matrix3x2 TRS(Vector2 position, float degrees, Vector2 scale)
    {
        return Matrix3x2.CreateScale(scale.X, scale.Y) * 
            Matrix3x2.CreateRotation(DegreesToRadians(degrees)) * 
            Matrix3x2.CreateTranslation(position);
    }

    public static void DrawRectangle(Vector2 center, float degrees, Vector2 size, Color color)
    {
        var m = TRS(center, degrees, size);

        List<Vector2> points = [];
        points.Add(new Vector2(-1, -1));
        points.Add(new Vector2(1, -1));
        points.Add(new Vector2(1, 1));
        points.Add(new Vector2(-1, 1));
        DrawShape(TransformPoints(m, [..points]), color);
    }

    public static void DrawEllipse(Vector2 center, float degrees, Vector2 size, int pointCount, Color color)
    {
        var m = TRS(center, degrees, size);

        List<Vector2> points = [];
        var rads = 0f;
        var delta = MathF.PI * 2f / pointCount;
        for(var i = 0; i < pointCount; i++)
        {
            points.Add(new Vector2(MathF.Cos(rads), MathF.Sin(rads)));
            rads += delta;
        }
        DrawShape(TransformPoints(m, [..points]), color);
    }
}