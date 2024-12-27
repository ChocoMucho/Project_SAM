using UnityEngine;

public class Selector : Node // OR 개념
{
    public Selector(string name, int priority = 0) : base(name, priority)
    {
    }

    public override Status Process()
    {
        if(currentChild < Children.Count)
        {
            switch (Children[currentChild].Process())
            {
                case Status.Success: // 성공하면 모든게 끝남
                    Reset();
                    return Status.Success;
                case Status.Running:
                    return Status.Running;
                default: // 실패하면 인덱스 올라감 => 다음 자식
                    ++currentChild;
                    return Status.Running;
            }
        }

        Reset();
        return Status.Failure;
    }

    public override void Reset()
    {
        base.Reset();
    }
}
