using UnityEngine;

public interface IState
{
    public void EnterState();
    public void UpdateState();
    public void ExitState();
    public void CheckSwitchState();
    public void InitializeSubState();
}
