
using System.Numerics;

class GameObject
{
    public bool open = true;
    public GameObject parent;
    public string name;
    public Vector2 position;
    public float degrees;
    public float scale = 1;
    public readonly List<GameObject> children = [];
    public readonly List<Component> components = [];

    GameObject(GameObject parent, string name)
    {
        this.name = name;
        this.parent = parent;
    }

    public static GameObject Create(GameObject parent, string name, Type[] componentTypes)
    {
        var g = new GameObject(parent,name);
        if(parent == null)
        {
            Scene.gameObjects.Add(g);
        }
        else
        {
            parent.children.Add(g);
        }
        foreach(var c in componentTypes)
        {
            g.AddComponent(c);
        }
        return g;
    }

    public void AddComponent(Type type)
    {
        var component = (Component)Activator.CreateInstance(type);
        component.gameObject = this;
        components.Add(component);
    }

    public void AddComponent<T>() where T:Component
    {
        AddComponent(typeof(T));
    }
}

abstract class Component
{
    public GameObject gameObject;

    public T GetComponent<T>() where T:Component
    {
        return gameObject.components.OfType<T>().First();
    }

    public virtual void Render(){}
}

static class Scene
{
    public static readonly List<GameObject> gameObjects = [];

    public static void Render(List<GameObject> gameObjects)
    {
        foreach(var g in gameObjects)
        {
            foreach(var c in g.components)
            {
                c.Render();
            }
            Render(g.children);
        }
    }
}