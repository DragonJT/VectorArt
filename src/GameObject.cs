
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

    public void AddComponent<T>() where T:Component
    {
        var component = Activator.CreateInstance<T>();
        component.gameObject = this;
        components.Add(component);
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