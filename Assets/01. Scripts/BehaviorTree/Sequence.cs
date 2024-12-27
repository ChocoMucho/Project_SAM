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
                case Status.Success: // �̹� �ڽ� ���� -> �ڽ� �ȳ�������? ���� ���� : ���� ����
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
