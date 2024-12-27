using UnityEngine;

public class Selector : Node // OR ����
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
                case Status.Success: // �����ϸ� ���� ����
                    Reset();
                    return Status.Success;
                case Status.Running:
                    return Status.Running;
                default: // �����ϸ� �ε��� �ö� => ���� �ڽ�
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
