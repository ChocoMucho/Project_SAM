using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree(string name, int priority = 0) : base(name, priority)
    {
    }

    public override Status Process()
    {
        Status status = Children[currentChild].Process();
        if(status != Status.Success) // ���� ���ϸ� �ƴϸ� ��� �Ȱ��� �ڽ��� process ȣ����.
        {
            return status;
        }
        currentChild = (currentChild + 1) % Children.Count;

        return Status.Success;
    }
}
