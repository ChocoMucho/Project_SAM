using UnityEngine;

public class Sequence : Node
{
    public Sequence(string name, int priority = 0) : base(name, priority){}

    public override Status Process()
    {
        if(currentChild < Children.Count)
        {
            switch (Children[currentChild].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    Reset();
                    return Status.Failure;
                case Status.Success: // 이번 자식 성공 -> 자식 안남았으면? 성공 리턴 : 진행 리턴
                    ++currentChild;
                    return currentChild == Children.Count ? Status.Success : Status.Running;
            }
        }
        Reset();
        return Status.Success;
    }

    public override void Reset()
    {
        base.Reset();
    }
}
