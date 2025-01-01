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

        // ���� ����ġ ��ȭ
        Player.SetRigWeight("RigUpperBodyLayer", 0.0f);
        Player.SetRigWeight("RigHandLayer", 0.0f);

        if (Player.Input.Reload) // �޸��� �߿� ������ ������ ���߰� �ڵ����� ��ȯ. ����
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
