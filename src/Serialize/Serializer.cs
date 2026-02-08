
using System.Numerics;
using System.Reflection;
using Raylib_cs;

[AttributeUsage(AttributeTargets.Field)]
class DoNotSerializeAttribute : Attribute
{
}

class Serializer
{
    public readonly List<byte> bytes = [];

    void Write(byte b)
    {
        bytes.Add(b);
    }

    void Write(int i)
    {
        bytes.AddRange(BitConverter.GetBytes(i));
    }

    void Write(float f)
    {
        bytes.AddRange(BitConverter.GetBytes(f));
    }

    void Write(bool b)
    {
        bytes.AddRange(BitConverter.GetBytes(b));
    }

    void Write(char c)
    {
        bytes.AddRange((byte)c);
    }

    void Write(Vector2 p)
    {
        Write(p.X);
        Write(p.Y);
    }

    void Write(Color c)
    {
        Write(c.R);
        Write(c.G);
        Write(c.B);
        Write(c.A);
    }

    void Write(string s)
    {
        Write(s.Length);
        for(var i = 0; i < s.Length; i++)
        {
            Write(s[i]);
        }
    }

    public void Write(GameObject g)
    {
        Write(g.open);
        Write(g.name);
        Write(g.position);
        Write(g.degrees);
        Write(g.scale);
        Write(g.components);
        Write(g.children);
    }

    void Write(Component c)
    {
        var type = c.GetType();
        Write(type.Name);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach(var f in fields)
        {
            if (f.GetCustomAttributes<DoNotSerializeAttribute>().Any())
            {
                continue;
            }
            if(f.FieldType == typeof(Vector2))
            {
                Write((Vector2)f.GetValue(c));
            }
            else if(f.FieldType == typeof(Color))
            {
                Write((Color)f.GetValue(c));
            }
            else
            {
                Console.WriteLine("========>"+f.FieldType.Name);
                throw new NotImplementedException();
            }
        }
    }

    void Write(List<Component> cs)
    {
        Write(cs.Count);
        for(var i = 0; i < cs.Count; i++)
        {
            Write(cs[i]);
        }
    }

    public void Write(List<GameObject> gs)
    {
        Write(gs.Count);
        for(var i = 0; i < gs.Count; i++)
        {
            Write(gs[i]);
        }
    }
}