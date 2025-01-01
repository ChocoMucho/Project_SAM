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
        // 애니메이터 Fall 파라미터 false로
        StateMachine.Animator.SetBool(PlayerAnimatorHashes.Fall, false);
    }

    public override void CheckSwitchState()
    {
        // 플레이어 컨트롤러에서 IsGround 로 상태 업데이트
        if (StateMachine.Controller.IsGround)
            SwitchState(Provider.GetState(PlayerStates.Ground));
    }
}
