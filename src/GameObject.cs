
using System.Numerics;

class GameObject
{
    public bool open = true;
    public GameObject parent;
    public string name = "";
    public Vector2 position;
    public float rotation;
    public Vector2 scale;
    public readonly List<GameObject> children = [];
    public readonly List<Component> components = [];

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