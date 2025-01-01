using UnityEngine;

public class ReloadState : BaseState
{
    public ReloadState(StateMachine StateMachine, StateProvider Provider) : base(StateMachine, Provider)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        base.EnterState();

        Player.Input.Reload = false;
        Player.Animator.SetTrigger(PlayerAnimatorHashes.Reload);
        Player.IsReloading = true;        
    }

    public override void UpdateState()
    {
        float layerWeight = Player.Animator.GetLayerWeight(1);
        Player.Animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, 1, Time.deltaTime * 10f));

        // 리깅 가중치 변화
        Player.SetRigWeight("RigUpperBodyLayer", 0f);
        Player.SetRigWeight("RigHandLayer", 0f);

        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
        Player.IsReloading = false;
    }

    public override void CheckSwitchState()
    {
        if (Player.IsRunning) // IsRunning? 
            SwitchState(Provider.GetState(PlayerStates.Run));
        if (!Player.IsReloading) // Reloading Ended?
            SwitchState(Provider.GetState(PlayerStates.Idle));
    }
}
