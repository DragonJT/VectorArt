
using System.Numerics;
using Raylib_cs;
using System.Reflection;

static class HierarchyMenuItems
{
    public static void Empty(Hierarchy hierarchy, GameObject gameObject)
    {
        hierarchy.selected = GameObject.Create(gameObject, "GameObject", []);
    }

    public static void AddRect(Hierarchy hierarchy, GameObject gameObject)
    {
        hierarchy.selected = GameObject.Create(gameObject, "Rect", [typeof(RectangleRenderer)]);
    }

    public static void AddEllipse(Hierarchy hierarchy, GameObject gameObject)
    {
        hierarchy.selected = GameObject.Create(gameObject, "Ellipse", [typeof(EllipseRenderer)]);
    }

    public static void Duplicate(Hierarchy hierarchy, GameObject gameObject)
    {
        var parent = gameObject.parent;
        var serializer = new Serializer();
        serializer.Write(gameObject);
        var deserializer = new DeSerializer([..serializer.bytes]);
        hierarchy.selected = deserializer.ReadGameObject(parent);
    }

    public static void Delete(Hierarchy hierarchy, GameObject gameObject)
    {
        gameObject?.Delete();
        hierarchy.selected = null;
    }
}

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

    void CallMenu(GameObject gameObject, Vector2 position)
    {   
        var menuItems = typeof(HierarchyMenuItems)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(m=>new MenuItem(m.Name, ()=>m.Invoke(null, [this, gameObject])))
            .ToArray();
        Program.contextMenu = new ContextMenu(position, menuItems);
    }

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
            MouseOver.TrySetMouseOver(rect, g);
            if(MouseOver.current == g)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    selected = g;
                    g.open = !g.open;
                }
                else if (Raylib.IsMouseButtonPressed(MouseButton.Right))
                {
                    CallMenu(g, Raylib.GetMousePosition());
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
        MouseOver.TrySetMouseOver(rect, this);
        if (MouseOver.current == this && Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            CallMenu(null, Raylib.GetMousePosition());
        }
        Raylib.BeginScissorMode((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        Raylib.ClearBackground(new Color(0.15f, 0.15f, 0.15f));
        Draw(layout, gameObjects, 0);
        Raylib.EndScissorMode();
        Raylib.DrawRectangleLinesEx(rect, 4, Color.Blue);
    }
}
