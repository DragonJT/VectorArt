
using Raylib_cs;

class HierarchyLayout
{
    int x;
    int y;
    int width;
    public Rectangle rect;

    public HierarchyLayout(Rectangle rect)
    {
        this.rect = rect;
        x = (int)rect.X;
        y = (int)rect.Y;
        width = (int)rect.Width;
    }

    public Rectangle GetRect()
    {
        var rect = new Rectangle(x,y,width,Library.lineSize);
        y += Library.lineSize;
        return rect;
    }
}

class Hierarchy
{
    public GameObject selected;

    void Draw(HierarchyLayout layout, List<GameObject> gameObjects, int indent)
    {
        foreach(var g in gameObjects)
        {
            var rect = layout.GetRect();
            if(g == selected)
            {
                Raylib.DrawRectangleRec(rect, Color.SkyBlue);
                Raylib.DrawRectangleLinesEx(rect, 4, Color.Blue);
            }
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect))
            {
                MouseOver.last = g;
            }
            if(MouseOver.current == g)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    selected = g;
                    g.open = !g.open;
                }
                else if (Raylib.IsMouseButtonPressed(MouseButton.Right))
                {
                    selected = g;
                    g.children.Add(new GameObject{name = "GameObject"});
                }
            }
            string openText = g.open ? "+ " : "- ";
            Raylib.DrawText(openText + g.name, (int)(rect.X + indent * Library.treeIndentSize + 10), (int)rect.Y, Library.fontSize, Color.White);
            if (g.open)
            {
                Draw(layout, g.children, indent+1);
            }
        }
    }

    public void Draw(Rectangle rect, List<GameObject> gameObjects)
    {
        HierarchyLayout layout = new(rect);
        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect))
        {
            MouseOver.last = this;
        }
        if (MouseOver.current == this && Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            selected = new GameObject{name = "GameObject"};
            Scene.gameObjects.Add(selected);
        }
        Raylib.BeginScissorMode((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        Raylib.ClearBackground(new Color(0.15f, 0.15f, 0.15f));
        Draw(layout, gameObjects, 0);
        Raylib.EndScissorMode();
        Raylib.DrawRectangleLinesEx(rect, 4, Color.Blue);
    }
}

static class Scene
{
    public static readonly List<GameObject> gameObjects = [];
}