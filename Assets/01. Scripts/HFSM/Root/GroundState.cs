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
        // 애니메이터 Ground 체크
    }

    public override void UpdateState() { CheckSwitchState(); }

    public override void ExitState() 
    {
        // 애니메이터 Ground 파라미터 false로
    }

    public override void CheckSwitchState() 
    {
        // 플레이어 컨트롤러에서 IsGround 로 상태 업데이트
    }

    public override void InitializeSubState() 
    {
        //Idle 부여
    }
}
