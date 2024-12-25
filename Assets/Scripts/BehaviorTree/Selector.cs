using UnityEngine;

public class Selector : Node
{
    public Selector(string name, int priority = 0) : base(name, priority)
    {
    }

    public override Status Process()
    {
        return base.Process();
    }

    public override void Reset()
    {
        base.Reset();
    }
}
