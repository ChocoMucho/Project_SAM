using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Status status;
    public readonly string name;
    public readonly List<Node> Children;
    protected int currentChild; // ¿Œµ¶Ω∫

    public Node(string name, int priority = 0)
    {
        this.name = name;
        Children = new List<Node>();
        currentChild = 0;
    }

    public void AddChild(Node child) => Children.Add(child);

    public virtual Status Process() => Children[currentChild].Process();

    public virtual void Reset()
    {
        currentChild = 0;
        foreach (Node child in Children)
        {
            child.Reset();
        }
    }
    
}
