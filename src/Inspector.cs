using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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

interface ISliderValue
{
    float GetValue01();
    void SetValue01(float value);
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

    public FieldByteValue GetByteField(string field)
    {
        return new FieldByteValue(this, fieldInfo.FieldType.GetField(field));
    }

    public FieldFloatValue GetFloatField(string field)
    {
        return new FieldFloatValue(this, fieldInfo.FieldType.GetField(field));
    }

    public FieldByteSliderValue GetFieldByteSliderValue(string field)
    {
        return new FieldByteSliderValue(this, fieldInfo.FieldType.GetField(field));
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

class FieldFloatValue(StructValue structValue, FieldInfo fieldInfo) : IFloatValue
{
    StructValue structValue = structValue;
    FieldInfo fieldInfo = fieldInfo;

    public float GetValue()
    {
        return (float)fieldInfo.GetValue(structValue.GetValue());   
    }

    public void SetValue(float value)
    {
        var obj = structValue.GetValue();
        fieldInfo.SetValue(obj, value);
        structValue.SetValue(obj);
    }
}

class FieldByteValue(StructValue structValue, FieldInfo fieldInfo) : IByteValue
{
    StructValue structValue = structValue;
    FieldInfo fieldInfo = fieldInfo;

    public void SetValue(byte value)
    {
        var obj = structValue.GetValue();
        fieldInfo.SetValue(obj, value);
        structValue.SetValue(obj);
    }

    public byte GetValue()
    {
        return (byte)fieldInfo.GetValue(structValue.GetValue());   
    }
}

class FieldByteSliderValue(StructValue structValue, FieldInfo fieldInfo) : ISliderValue
{
    StructValue structValue = structValue;
    FieldInfo fieldInfo = fieldInfo;

    public float GetValue01()
    {
        return (byte)fieldInfo.GetValue(structValue.GetValue()) / 255f;
    }

    public void SetValue01(float value)
    {
        var v = (byte)(float.Clamp(value, 0, 1) * 255);
        var obj = structValue.GetValue();
        fieldInfo.SetValue(obj, v);
        structValue.SetValue(obj);
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

abstract class CoreButton : IGUI, ISelectable
{
    public bool Selected{get;set;}
    Rectangle rect;

    public CoreButton(Rectangle rect)
    {
        this.rect = rect;
    }

    public abstract void OnClick(Vector2 position);

    public abstract void DrawContent(Rectangle rect, bool mouseOverOrSelected);

    public void Update(InspectorLayout layout)
    {
        var rect = layout.GetRect(this.rect);
        var mouseOverOrSelected = Selected || MouseOver.current == this;
        if(Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect))
        {
            MouseOver.last = this;
        }
        if (Program.contextMenu==null && MouseOver.current == this && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            OnClick(rect.Position);  
        }
        if (Program.contextMenu==null && Selected && Library.IsKeyPressed(KeyboardKey.Enter))
        {
            OnClick(rect.Position);
        }

        if ((MouseOver.current == this && Raylib.IsMouseButtonDown(MouseButton.Left)) 
            || (Selected && Raylib.IsKeyDown(KeyboardKey.Enter)))
        {
            Raylib.DrawRectangleRec(rect, new Color(0,1,0.4f));
        }
        Raylib.DrawRectangleLinesEx(rect, 3, mouseOverOrSelected ? Color.White:Color.Blue);
        DrawContent(rect, mouseOverOrSelected);
    }
}

class Button : CoreButton
{
    string text;
    Action<Vector2> action;

    public Button(Rectangle rect, string text, Action<Vector2> action) : base(rect)
    {
        this.text = text;
        this.action = action;
    }

    public override void DrawContent(Rectangle rect, bool mouseOverOrSelected)
    {
        Raylib.DrawText(text, (int)rect.X, (int)rect.Y, Library.fontSize, mouseOverOrSelected ? Color.White:Color.Blue);
    }

    public override void OnClick(Vector2 position)
    {
        action(position);
    }
}

interface IContextWindow
{
    void Update();
}

class ColorWindow : IContextWindow
{
    Rectangle rect;
    List<IGUI> guis = [];

    void AddField(float width, StructValue structValue, string fieldName, ref float y)
    {
        var sliderValue = structValue.GetFieldByteSliderValue(fieldName);
        var x = 0f;
        guis.Add(new Label(new Vector2(x+10,y), fieldName, Color.Blue));
        x += width * 0.3f;
        guis.Add(new Slider(new Rectangle(x,y,width * 0.7f,Library.fontSize).GrowX(-10), sliderValue));
        y += Library.lineSize;
    }

    public ColorWindow(StructValue structValue, Vector2 position)
    {
        rect = new Rectangle(position, Library.contextMenuWidth, Library.lineSize * 4);
        var width = rect.Width;
        var y = 0f;
        AddField(width, structValue, "R", ref y);
        AddField(width, structValue, "G", ref y);
        AddField(width, structValue, "B", ref y);
        AddField(width, structValue, "A", ref y);
    }

    public void Update()
    {
        bool mouseOver = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect);
        if(!mouseOver && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Program.contextMenu = null;
        }
        if(mouseOver)
        {
            MouseOver.last = this;
        }
        GUIWindow.DrawGUIWindow(rect, guis, true);
    }
}

class ColorPicker : CoreButton
{
    public readonly StructValue structValue;

    public ColorPicker(Rectangle rect, StructValue structValue) : base(rect)
    {
        this.structValue = structValue;
    }

    public override void DrawContent(Rectangle rect, bool mouseOverOrSelected)
    {
        rect.Grow(-5);
        Raylib.DrawRectangleRec(rect, (Color)structValue.GetValue());
    }

    public override void OnClick(Vector2 position)
    {
        Program.contextMenu = new ColorWindow(structValue, position);
    }
}

class Slider : IGUI, ISelectable
{
    public bool Selected{get;set;}
    Rectangle rect;
    ISliderValue sliderValue;
    bool dragging = false;

    public Slider(Rectangle rect, ISliderValue sliderValue)
    {
        this.rect = rect;
        this.sliderValue = sliderValue;
    }

    public void Update(InspectorLayout layout)
    {
        var rect = layout.GetRect(this.rect);
        float val = sliderValue.GetValue01();
        if (Selected)
        {
            if (Library.IsKeyPressed(KeyboardKey.Left))
            {
                val -= 0.01f;
            }
            if (Library.IsKeyPressed(KeyboardKey.Right))
            {
                val += 0.01f;
            }
            sliderValue.SetValue01(val);
        }
        if(Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect))
        {
            MouseOver.last = this;
        }
        if (MouseOver.current == this && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            dragging = true;
        }
        if (dragging)
        {
            sliderValue.SetValue01((Raylib.GetMousePosition().X - rect.X) / rect.Width);
        }
        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            dragging = false;
        }
        var color = Selected ? Color.White : Color.Blue;
        Raylib.DrawRectangleLinesEx(rect, 3, color);
        var r = rect;
        r.Grow(-5);
        Raylib.DrawRectangleRec(new Rectangle(r.X, r.Y, r.Width * val, r.Height), color);
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

static class GUIWindow
{
    static void SetSelectable(List<ISelectable> selectables, int index, bool selected)
    {
        if(index >= 0 && index < selectables.Count)
        {
            selectables[index].Selected = selected;
        }
    }

    public static void DrawGUIWindow(Rectangle rect, List<IGUI> guis, bool allowTab)
    {
        Raylib.BeginScissorMode((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        Raylib.ClearBackground(new Color(0.15f, 0.15f, 0.15f));
        InspectorLayout layout = new(rect.Position);
        if (allowTab && Library.IsKeyPressed(KeyboardKey.Tab))
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
                var xval = new FloatStringValue(vec2Value.GetFloatField("X"));
                guis.Add(new Textbox(new Rectangle(x,y,width*0.35f,Library.fontSize).GrowX(-10), xval));
                x += width * 0.35f;
                var yval = new FloatStringValue(vec2Value.GetFloatField("Y"));
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
                var colorValue = new StructValue(f, obj);
                guis.Add(new ColorPicker(new Rectangle(x,y,width * 0.7f,Library.fontSize).GrowX(-10), colorValue));
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

    public void Update(Rectangle rect, GameObject gameObject)
    {
        SetObject(gameObject, rect.Width);
        GUIWindow.DrawGUIWindow(rect, guis, Program.contextMenu == null);
    }
}