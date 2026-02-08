
using System.Numerics;
using System.Reflection;
using Raylib_cs;

class DeSerializer
{
    readonly byte[] bytes;
    int index = 0;

    public DeSerializer(byte[] bytes)
    {
        this.bytes = bytes;
    }

    byte ReadByte()
    {
        var b = bytes[index];
        index++;
        return b;
    }

    int ReadInt()
    {
        var i = BitConverter.ToInt32(bytes, index);
        index += 4;
        return i;
    }

    float ReadFloat()
    {
        var f = BitConverter.ToSingle(bytes, index);
        index += 4;
        return f;
    }

    bool ReadBool()
    {
        var b = BitConverter.ToBoolean(bytes, index);
        index++;
        return b;
    }

    char ReadChar()
    {
        var c = (char)bytes[index];
        index++;
        return c;
    }

    Vector2 ReadVector2()
    {
        return new (ReadFloat(), ReadFloat());
    }

    Color ReadColor()
    {
        return new(ReadByte(), ReadByte(), ReadByte(), ReadByte());
    }

    string ReadString()
    {
        var length = ReadInt();
        var chars = new char[length];
        for(var i = 0; i < length; i++)
        {
            chars[i] = ReadChar();
        }
        return new string(chars);
    }

    public GameObject ReadGameObject(GameObject parent)
    {
        var g = GameObject.Create(parent, null, []);
        g.open = ReadBool();
        g.name = ReadString();
        g.position = ReadVector2();
        g.degrees = ReadFloat();
        g.scale = ReadFloat();
        g.components.AddRange(ReadComponents(g));
        ReadGameObjects(g);
        return g;
    }

    Component ReadComponent(GameObject g)
    {
        var name = ReadString();
        var type = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(Component)))
            .FirstOrDefault(t => t.Name == name);
        if (type == null)
        {
            Console.WriteLine("-->"+name+"<--");
            throw new Exception();
        }
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var c = (Component)Activator.CreateInstance(type);
        c.gameObject = g;
        foreach(var f in fields)
        {
            if (f.GetCustomAttributes<DoNotSerializeAttribute>().Any())
            {
                continue;
            }
            if(f.FieldType == typeof(Vector2))
            {
                f.SetValue(c, ReadVector2());
            }
            else if(f.FieldType == typeof(Color))
            {
                f.SetValue(c, ReadColor());
            }
            else
            {
                Console.WriteLine("========>"+f.FieldType.Name);
                throw new NotImplementedException();
            }
        }
        return c;
    }

    Component[] ReadComponents(GameObject parent)
    {
        var length = ReadInt();
        Component[] components = new Component[length];
        for(var i = 0; i < length; i++)
        {
            components[i] = ReadComponent(parent);
        }
        return components;
    }

    void ReadGameObjects(GameObject parent)
    {
        var length = ReadInt();
        for(var i = 0; i < length; i++)
        {
            ReadGameObject(parent);
        }
    }
}