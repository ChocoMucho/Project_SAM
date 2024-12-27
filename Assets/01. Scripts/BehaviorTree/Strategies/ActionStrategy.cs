using System;
public class ActionStrategy : IStrategy // �Ǵ� ���� �ൿ�� �ϴ� �κ�
{
    private Action action; // �Ű����� �ִ� �޼��忩�� ���ٽ����� ���� �Լ� ���� ����

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
