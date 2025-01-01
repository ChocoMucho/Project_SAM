using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(StateMachine StateMachine, StateProvider Provider) : base(StateMachine, Provider)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        base.EnterState();        
    }

    public override void UpdateState()
    {
        base.UpdateState();

        float layerWeight = Player.Animator.GetLayerWeight(1);
        Player.Animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, 0, Time.deltaTime * 10f));

        // 리깅 가중치 변화
        Player.SetRigWeight("RigUpperBodyLayer", 0.0f);
        Player.SetRigWeight("RigHandLayer", 0.0f);
    }

    public override void CheckSwitchState()
    {
        if(Player.IsRunning) // Running?
            SwitchState(Provider.GetState(PlayerStates.Run));
        
        if(Player.Input.Reload && Player.CurrentAmmo > 0) // Reload Key Pressed && Ammo > 0
            SwitchState(Provider.GetState(PlayerStates.Reload));

        if(Player.Input.Aim) // Aim Key Pressed
            SwitchState(Provider.GetState(PlayerStates.Aim));

    }

    // 서브 스테이트 없음
}
