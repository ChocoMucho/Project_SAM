using UnityEngine;

public class AimState : BaseState
{
    private Vector3 _targetedPosition;

    public AimState(StateMachine StateMachine, StateProvider Provider) : base(StateMachine, Provider)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        base.EnterState();
        // aim 세팅
        Player.AimCamera.SetActive(true);
    }

    public override void UpdateState()
    {
        AimLogic();

        // 리깅 가중치 변화
        Player.SetRigWeight("RigUpperBodyLayer");
        Player.SetRigWeight("RigHandLayer");
        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
        Player.AimCamera.SetActive(false);
    }

    public override void CheckSwitchState()
    {        
        if (Player.IsRunning) // Running?
            SwitchState(Provider.GetState(PlayerStates.Run));
        if (!Player.Input.Aim) // Not Aiming
            SwitchState(Provider.GetState(PlayerStates.Idle));
        if (Player.Input.Reload && Player.CurrentAmmo > 0) // Reload Key Pressed && Ammo > 0
            SwitchState(Provider.GetState(PlayerStates.Reload));
    }

    private void AimLogic()
    {
        float layerWeight = Player.Animator.GetLayerWeight(1);
        Player.Animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, 1, Time.deltaTime * 10f));

        Transform camera = Player.MainCamera.transform;
        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, 1000))
        {
            _targetedPosition = hit.point;
            Player.TargetObject.transform.position = hit.point;
        }

        Vector3 tempVector = _targetedPosition;
        tempVector.y = Player.transform.position.y;
        Vector3 playerDirection = (tempVector - Player.transform.position).normalized;

        Player.transform.forward = Vector3.Lerp(Player.transform.forward, playerDirection, Time.deltaTime * 20f);

        if (Player.Input.Fire)
        {
            Player.Weapon.Fire(_targetedPosition);
        }
    }

}
