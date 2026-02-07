using System.Numerics;
using System.Reflection;
using Raylib_cs;

class InspectorLayout
{
    Vector2 position;

    public InspectorLayout(Vector2 position)
    {
        this.position = position;
    }

    public Rectangle GetRect(Rectangle rect)
    {
        return new Rectangle(rect.X + position.X, rect.Y + position.Y, rect.Width, rect.Height);
    }

    public Vector2 GetPosition(Vector2 position)
    {
        return this.position + position;
    }
}

interface IFloatValue
{
    float GetValue();
    void SetValue(float value);
}

interface IStringValue
{
    string GetValue();
    void SetValue(string value);
}

interface IByteValue
{
    byte GetValue();
    void SetValue(byte value);
}

interface ICall
{
    void Call(Vector2 position);
}

class StructValue
{
    public FieldInfo fieldInfo;
    public object obj;

    public StructValue(FieldInfo fieldInfo, object obj)
    {
        this.fieldInfo = fieldInfo;
        this.obj = obj;
    }

    public FieldValue GetField(string field)
    {
        return new FieldValue(this, fieldInfo.FieldType.GetField(field));
    }

    public object GetValue()
    {
        return fieldInfo.GetValue(obj);
    }

    public void SetValue(object value)
    {
        fieldInfo.SetValue(obj, value);
    }
}

class FieldValue : IFloatValue, IByteValue
{
    StructValue structValue;
    FieldInfo fieldInfo;

    public FieldValue(StructValue structValue, FieldInfo fieldInfo)
    {
        this.structValue = structValue;
        this.fieldInfo = fieldInfo;  
    }

    void IFloatValue.SetValue(float value)
    {
        var obj = structValue.GetValue();
        fieldInfo.SetValue(obj, value);
        structValue.SetValue(obj);
    }

    float IFloatValue.GetValue()
    {
        return (float)fieldInfo.GetValue(structValue.GetValue());   
    }

    void IByteValue.SetValue(byte value)
    {
        var obj = structValue.GetValue();
        fieldInfo.SetValue(obj, value);
        structValue.SetValue(obj);
    }

    byte IByteValue.GetValue()
    {
        return (byte)fieldInfo.GetValue(structValue.GetValue());   
    }
}


class ByteValue : IByteValue
{
    FieldInfo fieldInfo;
    object obj;

    public ByteValue(FieldInfo fieldInfo, object obj)
    {
        this.fieldInfo = fieldInfo;
        this.obj = obj;
    }

    public byte GetValue()
    {
        return (byte)fieldInfo.GetValue(obj);
    }

    public void SetValue(byte value)
    {
        fieldInfo.SetValue(obj, value);
    }
}

class ByteStringValue : IStringValue 
{
    IByteValue byteValue;
    string lastString;
    float lastByte;

    public ByteStringValue(IByteValue byteValue)
    {
        this.byteValue = byteValue;
        lastByte = byteValue.GetValue();
        lastString = lastByte.ToString();
    }

    public string GetValue()
    {
        var f = byteValue.GetValue();
        if(f == lastByte)
        {
            return lastString;
        }
        return f.ToString();
    }

    static bool TryParse(string text, out byte b)
    {
        if(byte.TryParse(text, out b))
        {
            return true;
        }
        else if(text == "")
        {
            b = 0;
            return true;
        }
        return false;
    }

    public void SetValue(string value)
    {
        if(TryParse(value, out byte b))
        {
            lastByte = byteValue.GetValue();
            lastString = value;
            byteValue.SetValue(b);
        }
    }
}

class FloatValue : IFloatValue
{
    FieldInfo fieldInfo;
    object obj;

    public FloatValue(FieldInfo fieldInfo, object obj)
    {
        this.fieldInfo = fieldInfo;
        this.obj = obj;
    }

    public float GetValue()
    {
        return (float)fieldInfo.GetValue(obj);
    }

    public void SetValue(float value)
    {
        fieldInfo.SetValue(obj, value);
    }
}

class StringValue : IStringValue
{
    FieldInfo fieldInfo;
    object obj;

    public StringValue(FieldInfo fieldInfo, object obj)
    {
        this.fieldInfo = fieldInfo;
        this.obj = obj;
    }

    public string GetValue()
    {
        return (string)fieldInfo.GetValue(obj);
    }

    public void SetValue(string value)
    {
        fieldInfo.SetValue(obj, value);
    }
}

class FloatStringValue : IStringValue 
{
    IFloatValue floatValue;
    string lastString;
    float lastFloat;

    public FloatStringValue(IFloatValue floatValue)
    {
        this.floatValue = floatValue;
        lastFloat = floatValue.GetValue();
        lastString = lastFloat.ToString();
    }

    public string GetValue()
    {
        var f = floatValue.GetValue();
        if(f == lastFloat)
        {
            return lastString;
        }
        return f.ToString();
    }

    static bool TryParse(string text, out float f)
    {
        if(float.TryParse(text, out f))
        {
            return true;
        }
        else if(text == "")
        {
            f = 0;
            return true;
        }
        else  if(text == "-")
        {
            f = 0;
            return true;
        }
        return false;
    }

    public void SetValue(string value)
    {
        if(TryParse(value, out float f))
        {
            lastFloat = floatValue.GetValue();
            lastString = value;
            floatValue.SetValue(f);
        }
    }
}

class CallAction(Action<Vector2> action) : ICall
{
    readonly Action<Vector2> action = action;

    public void Call(Vector2 position)
    {
        action(position);
    }
}

class Button : IGUI, ISelectable
{
    public bool Selected{get;set;}
    Rectangle rect;
    string text;
    Action<Vector2> action;

    public Button(Rectangle rect, string text, Action<Vector2> action)
    {
        this.rect = rect;
        this.text = text;
        this.action = action;
    }

    public void Update(InspectorLayout layout)
    {
        var rect = layout.GetRect(this.rect);
        var color = Selected ? Color.White : Color.Blue;
        if(MouseOver.current == this)
        {
            color = Color.White;
        }
        if(Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect))
        {
            MouseOver.last = this;
        }
        if (Program.contextMenu==null && MouseOver.current == this && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            action(rect.Position);   
        }
        if (Program.contextMenu==null && Selected && Library.IsKeyPressed(KeyboardKey.Enter))
        {
            action(rect.Position);
        }

        if ((MouseOver.current == this && Raylib.IsMouseButtonDown(MouseButton.Left)) 
            || (Selected && Raylib.IsKeyDown(KeyboardKey.Enter)))
        {
            Raylib.DrawRectangleRec(rect, new Color(0,1,0.4f));
        }
        Raylib.DrawRectangleLinesEx(rect, 3, color);
        Raylib.DrawText(text, (int)rect.X, (int)rect.Y, Library.fontSize, color);
    }
}

class Textbox : IGUI, ISelectable
{
    public bool Selected{get;set;}
    Rectangle rect;
    IStringValue value;

    public Textbox(Rectangle rect, IStringValue value)
    {
        this.rect = rect;
        this.value = value;
    }

    public void Update(InspectorLayout layout)
    {
        var rect = layout.GetRect(this.rect);
        var text = value.GetValue();
        if (Selected)
        {
            Library.UpdateText(ref text);
            value.SetValue(text);
        }
        if(Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect))
        {
            MouseOver.last = this;
        }
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Selected = MouseOver.current == this;
        }
        var color = Selected ? Color.White : Color.Blue;
        Raylib.DrawRectangleLinesEx(rect, 3, color);
        Raylib.DrawText(text, (int)rect.X, (int)rect.Y, Library.fontSize, color);
    }
}

class Label:IGUI
{
    Vector2 position;
    string text;
    Color color;

    public Label(Vector2 position, string text, Color color)
    {
        this.position = position;
        this.text = text;
        this.color = color;
    }

    public void Update(InspectorLayout layout)
    {
        var position = layout.GetPosition(this.position);
        Raylib.DrawText(text, (int)position.X, (int)position.Y, Library.fontSize, color);
    }
}

interface IGUI
{
    void Update(InspectorLayout layout);
}

interface ISelectable
{
    bool Selected{get;set;}
}

class Inspector
{
    object gameObject;
    List<IGUI> guis = [];
    bool refresh;

    void AddObject(object obj, ref int y, float width)
    {
        foreach(var f in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var x = 0f;
            if(f.FieldType == typeof(Vector2))
            {
                guis.Add(new Label(new Vector2(x+10,y), f.Name, Color.Blue));
                x += width * 0.3f;
                var vec2Value = new StructValue(f, obj);
                var xval = new FloatStringValue(vec2Value.GetField("X"));
                guis.Add(new Textbox(new Rectangle(x,y,width*0.35f,Library.fontSize).GrowX(-10), xval));
                x += width * 0.35f;
                var yval = new FloatStringValue(vec2Value.GetField("Y"));
                guis.Add(new Textbox(new Rectangle(x,y,width*0.35f,Library.fontSize).GrowX(-10), yval));
                y += Library.lineSize;
            }
            else if(f.FieldType == typeof(float))
            {
                guis.Add(new Label(new Vector2(x+10,y), f.Name, Color.Blue));
                x += width * 0.3f;
                var fval = new FloatStringValue(new FloatValue(f, obj));
                guis.Add(new Textbox(new Rectangle(x,y,width * 0.7f,Library.fontSize).GrowX(-10), fval));
                y += Library.lineSize;
            }
            else if(f.FieldType == typeof(byte))
            {
                guis.Add(new Label(new Vector2(x+10,y), f.Name, Color.Blue));
                x += width * 0.3f;
                var bval = new ByteStringValue(new ByteValue(f, obj));
                guis.Add(new Textbox(new Rectangle(x,y,width * 0.7f,Library.fontSize).GrowX(-10), bval));
                y += Library.lineSize;
            }
            else if(f.FieldType == typeof(Color))
            {
                guis.Add(new Label(new Vector2(x+10,y), f.Name, Color.Blue));
                x += width * 0.3f;
                var iwidth = 0.7f / 4f;
                var colorValue = new StructValue(f, obj);
                var rval = new ByteStringValue(colorValue.GetField("R"));
                guis.Add(new Textbox(new Rectangle(x,y,width*iwidth,Library.fontSize).GrowX(-10), rval));
                x += width * iwidth;
                var gval = new ByteStringValue(colorValue.GetField("G"));
                guis.Add(new Textbox(new Rectangle(x,y,width*iwidth,Library.fontSize).GrowX(-10), gval));
                x += width * iwidth;
                var bval = new ByteStringValue(colorValue.GetField("B"));
                guis.Add(new Textbox(new Rectangle(x,y,width*iwidth,Library.fontSize).GrowX(-10), bval));
                x += width * iwidth;
                var aval = new ByteStringValue(colorValue.GetField("A"));
                guis.Add(new Textbox(new Rectangle(x,y,width*iwidth,Library.fontSize).GrowX(-10), aval));
                y += Library.lineSize;
            }
        }
    }

    void SetObject(GameObject gameObject, float width)
    {
        if(this.gameObject != gameObject || refresh)
        {
            refresh = false;
            guis.Clear();
            if(gameObject == null)
            {
                this.gameObject = null;
                return;
            }
            this.gameObject = gameObject;
            var y = 10;
            guis.Add(new Label(new Vector2(10, y), "GameObject", Color.White));
            y += Library.lineSize;
            AddObject(gameObject, ref y, width);
            foreach(var c in gameObject.components)
            {
                guis.Add(new Label(new Vector2(10, y), c.GetType().Name, Color.White));
                y += Library.lineSize;
                AddObject(c, ref y, width);
            }
            Action<Vector2> action = p =>
            {
                var items = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t=>t.IsAssignableTo(typeof(Component)))
                    .Where(t=>!t.IsAbstract)
                    .Select(t=>new MenuItem(t.Name, ()=>{
                        gameObject.AddComponent(t);
                        refresh = true;
                    }))
                    .ToArray();
                Program.contextMenu = new ContextMenu(p, items);
            };
            guis.Add(new Button(new Rectangle(10, y, width-20, Library.fontSize), "Add Component", action));
        }
    }

    static void SetSelectable(List<ISelectable> selectables, int index, bool selected)
    {
        if(index >= 0 && index < selectables.Count)
        {
            selectables[index].Selected = selected;
        }
    }

    public void Update(Rectangle rect, GameObject gameObject)
    {
        SetObject(gameObject, rect.Width);
        Raylib.BeginScissorMode((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        Raylib.ClearBackground(new Color(0.15f, 0.15f, 0.15f));
        InspectorLayout layout = new(rect.Position);
        if (Library.IsKeyPressed(KeyboardKey.Tab))
        {
            var selectables = guis.OfType<ISelectable>().ToList();
            var index = selectables.FindIndex(s=>s.Selected);
            SetSelectable(selectables, index, false);
            SetSelectable(selectables, index+1, true);
        }
        foreach(var g in guis)
        {
            g.Update(layout);
        }
        Raylib.EndScissorMode();
        Raylib.DrawRectangleLinesEx(rect, 4, Color.Blue);
    }
}