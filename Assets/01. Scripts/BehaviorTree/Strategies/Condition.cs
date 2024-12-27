using System;
using UnityEngine;

public class Condition : IStrategy
{
    Func<bool> _predicate; // bool���� �����ϴ� �޼���(������Ƽ)�� �޾Ƽ� ���� Ȯ�� ����

    public Condition (Func<bool> predicate)
    {
        _predicate = predicate;
    }

    public Status Process() => _predicate() ? Status.Success : Status.Failure;
}
