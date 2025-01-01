using UnityEngine;

public class AirState : BaseState
{
    public AirState(StateMachine StateMachine, StateProvider Provider) : base(StateMachine, Provider)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        base.EnterState();        
    }

    public override void UpdateState()
    {
        if(StateMachine.Controller.IsFalling)
            StateMachine.Animator.SetBool(PlayerAnimatorHashes.Fall, true);
        base.UpdateState();
    }

    public override void ExitState()
    {
        // �ִϸ����� Fall �Ķ���� false��
        StateMachine.Animator.SetBool(PlayerAnimatorHashes.Fall, false);
    }

    public override void CheckSwitchState()
    {
        // �÷��̾� ��Ʈ�ѷ����� IsGround �� ���� ������Ʈ
        if (StateMachine.Controller.IsGround)
            SwitchState(Provider.GetState(PlayerStates.Ground));
    }
}
