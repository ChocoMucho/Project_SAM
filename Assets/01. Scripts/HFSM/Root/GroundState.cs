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
        // 애니메이터 Ground 파라미터 false로
        StateMachine.Animator.SetBool(PlayerAnimatorHashes.Grounded, false);
    }

    public override void CheckSwitchState() 
    {
        // 플레이어 컨트롤러에서 IsGround 로 상태 업데이트
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
