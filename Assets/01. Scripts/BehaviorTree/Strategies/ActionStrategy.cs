using System;
public class ActionStrategy : IStrategy // 판단 없이 행동만 하는 부분
{
    private Action action; // 매개변수 있는 메서드여도 람다식으로 무명 함수 전달 가능

    public ActionStrategy(Action action)
    {
        this.action = action;
    }

    public Status Process()
    {
        action?.Invoke();
        return Status.Success;
    }
}
