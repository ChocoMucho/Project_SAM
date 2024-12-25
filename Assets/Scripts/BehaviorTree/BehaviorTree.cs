using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree(string name, int priority = 0) : base(name, priority)
    {
    }

    public override Status Process()
    {
        Status status = Children[currentChild].Process();
        if(status != Status.Success) // 성공 리턴만 아니면 계속 똑같은 자식의 process 호출함.
        {
            return status;
        }
        currentChild = (currentChild + 1) % Children.Count;

        return Status.Success;
    }
}
