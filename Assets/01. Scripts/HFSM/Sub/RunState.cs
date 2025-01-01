using UnityEngine;

public class RunState : BaseState
{
    public RunState(StateMachine StateMachine, StateProvider Provider) : base(StateMachine, Provider)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        base.EnterState();
        Player.Animator.SetBool(PlayerAnimatorHashes.Run, true);        
    }

    public override void UpdateState()
    {
        base.UpdateState();

        float layerWeight = Player.Animator.GetLayerWeight(1);
        Player.Animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, 0, Time.deltaTime * 10f));

        // 리깅 가중치 변화
        Player.SetRigWeight("RigUpperBodyLayer", 0.0f);
        Player.SetRigWeight("RigHandLayer", 0.0f);

        if (Player.Input.Reload) // 달리기 중에 재장전 누르면 멈추고 자동으로 변환. 방지
            Player.Input.Reload = false;
    }

    public override void ExitState()
    {
        Player.Animator.SetBool(PlayerAnimatorHashes.Run, false);
        base.ExitState();
    }

    public override void CheckSwitchState()
    {
        if (!Player.IsRunning) SwitchState(Provider.GetState(PlayerStates.Idle));
    }
}
