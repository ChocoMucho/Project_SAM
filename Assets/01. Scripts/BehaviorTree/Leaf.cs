using UnityEngine;

public class Leaf : Node
{
    IStrategy strategy;

    public Leaf(string name, IStrategy strategy, int priority = 0) : base(name)
    {
        this.strategy = strategy;
    }

    public override Status Process()
    {
        Debug.Log(name);
        return strategy.Process();
    }
    public override void Reset() => strategy.Reset();
}
