using System.Numerics;
using Raylib_cs;

record MenuItem(string Text, Action Action);

class ContextMenu
{
    Rectangle rect;
    MenuItem[] items;
    int id;
    bool firstFrame = true;

    public ContextMenu(Vector2 position, MenuItem[] items)
    {
        rect = new (position, Library.contextMenuWidth, items.Length * Library.lineSize);
        id = 0;
        this.items = items;
    }

    public void Update()
    {
        if (Library.IsKeyPressed(KeyboardKey.Up))
        {
            id--;
            if(id < 0)
            {
                id = items.Length-1;
            }
        }
        if (Library.IsKeyPressed(KeyboardKey.Down))
        {
            id++;
            if(id >= items.Length)
            {
                id = 0;
            }
        }
        if (!firstFrame && (Library.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsMouseButtonPressed(MouseButton.Left)))
        {
            items[id].Action();
            Program.contextMenu = null;
        }
        
        var x = (int)rect.X;
        var y = (int)rect.Y;
        Raylib.DrawRectangleRec(rect, new Color(0.25f,0.25f,0.25f));
        for(var i = 0; i < items.Length; i++)
        {
            var itemRect = new Rectangle(x,y,Library.contextMenuWidth, Library.lineSize);
            bool mouseOver = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), itemRect);
            if (mouseOver && Raylib.GetMouseDelta().Length() > 0)
            {
                id = i;
            }
            Color color = Color.Blue;
            if(id == i)
            {
                Raylib.DrawRectangleRec(itemRect, new Color(0, 1, 0.4f));
                color = Color.White;
            }
            Raylib.DrawText(items[i].Text, x, y, Library.fontSize, color);
            y+=Library.lineSize;
        }
        Raylib.DrawRectangleLinesEx(rect, 4, Color.Blue);
        firstFrame = false;
    }
}