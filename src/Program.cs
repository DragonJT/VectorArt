using System.Numerics;
using Raylib_cs;

static class MouseOver
{
    public static object current;
    public static object last;

    public static void LateUpdate()
    {
        current = last;
        last = null;
    }
}

static class Program
{
    public static ContextMenu contextMenu = null;

    static void Main()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1000,800,"VectorArt");
        Raylib.MaximizeWindow();
        var hierarchy = new Hierarchy();
        const int inspectorWidth = 600;
        var inspector = new Inspector();
        while (!Raylib.WindowShouldClose())
        {
            var scrWidth = Raylib.GetScreenWidth();
            var scrHeight = Raylib.GetScreenHeight();
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(0.2f, 0.2f, 0.2f));

            var camera = new Camera2D{Offset = Raylib.GetScreenCenter(), Rotation = 0, Target = Vector2.Zero, Zoom = 1};
            Raylib.BeginMode2D(camera);
            foreach(var g in Scene.gameObjects)
            {
                foreach(var c in g.components)
                {
                    c.Render();
                }
            }
            Raylib.EndMode2D();

            hierarchy.Draw(new Rectangle(20,20,500,scrHeight-40), Scene.gameObjects);
            var inspectorRect = new Rectangle(scrWidth - inspectorWidth - 20, 20, inspectorWidth, scrHeight - 40);
            inspector.Update(inspectorRect, hierarchy.selected);
            contextMenu?.Update();
            MouseOver.LateUpdate();
            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }
}