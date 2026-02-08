using System.Numerics;
using Raylib_cs;

class MenuItem(string text, Action action)
{
    public readonly string text = text;
    public readonly Action action = action;
}

class ContextMenu : IContextWindow
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
        if (!firstFrame && Library.IsKeyPressed(KeyboardKey.Enter))
        {
            items[id].action();
            Program.contextMenu = null;
        }
        
        var x = (int)rect.X;
        var y = (int)rect.Y;
        Raylib.DrawRectangleRec(rect, new Color(0.25f,0.25f,0.25f));
        bool leftClick = Raylib.IsMouseButtonPressed(MouseButton.Left);
        bool mouseOver = false;
        for(var i = 0; i < items.Length; i++)
        {
            var itemRect = new Rectangle(x,y,Library.contextMenuWidth, Library.lineSize);
            bool mouseOverItem = MouseOver.current == items[i];
            if(mouseOverItem) mouseOver = true;
            MouseOver.TrySetMouseOver(itemRect, items[i]);
            if(mouseOverItem && leftClick)
            {
                items[i].action();
                Program.contextMenu = null;
            }
            if (mouseOverItem && Raylib.GetMouseDelta().Length() > 0)
            {
                id = i;
            }
            Color color = Color.Blue;
            if(id == i)
            {
                Raylib.DrawRectangleRec(itemRect, new Color(0, 1, 0.4f));
                color = Color.White;
            }
            Raylib.DrawText(items[i].text, x, y, Library.fontSize, color);
            y += Library.lineSize;
        }
        if(!mouseOver && leftClick)
        {
            Program.contextMenu = null;
        }
        Raylib.DrawRectangleLinesEx(rect, 4, Color.Blue);
        firstFrame = false;
    }
}