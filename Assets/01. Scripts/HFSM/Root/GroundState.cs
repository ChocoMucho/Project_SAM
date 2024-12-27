using UnityEngine;

public class GroundState : BaseState
{
    public GroundState(StateMachine StateMachine, StateProvider Provider) : base(StateMachine, Provider)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        base.EnterState();
        // �ִϸ����� Ground üũ
    }

    public override void UpdateState() { CheckSwitchState(); }

    public override void ExitState() 
    {
        // �ִϸ����� Ground �Ķ���� false��
    }

    public override void CheckSwitchState() 
    {
        // �÷��̾� ��Ʈ�ѷ����� IsGround �� ���� ������Ʈ
    }

    public override void InitializeSubState() 
    {
        //Idle �ο�
    }
}
