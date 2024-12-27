using System;
using UnityEngine;

public class Condition : IStrategy
{
    Func<bool> _predicate; // bool값만 리턴하는 메서드(프로퍼티)를 받아서 조건 확인 가능

    public Condition (Func<bool> predicate)
    {
        _predicate = predicate;
    }

    public Status Process() => _predicate() ? Status.Success : Status.Failure;
}
