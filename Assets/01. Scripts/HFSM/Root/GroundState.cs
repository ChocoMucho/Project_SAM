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
        StateMachine.Animator.SetBool(PlayerAnimatorHashes.Grounded, true);
    }

    public override void ExitState() 
    {
        // �ִϸ����� Ground �Ķ���� false��
        StateMachine.Animator.SetBool(PlayerAnimatorHashes.Grounded, false);
    }

    public override void CheckSwitchState() 
    {
        // �÷��̾� ��Ʈ�ѷ����� IsGround �� ���� ������Ʈ
        if(!StateMachine.Player.Controller.IsGround)
        {
            SwitchState(Provider.GetState(PlayerStates.Air));
        }        
    }

    public override void InitializeSubState() 
    {
        SetSubState(Provider.GetState(PlayerStates.Idle));
    }
}
