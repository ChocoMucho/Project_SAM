using UnityEngine;

public abstract class BaseState : IState
{
    protected bool IsRootState { get; set; } = false;
    protected StateMachine StateMachine { get; private set; }
    protected StateProvider Provider { get; private set; }
    protected Player Player { get; private set; }
    protected BaseState _currentSuperState;
    protected BaseState _currentSubState;

    public BaseState(StateMachine StateMachine, StateProvider Provider)
    {
        this.StateMachine = StateMachine;
        this.Provider = Provider; 
    }

    public virtual void EnterState() { InitializeSubState(); }
    public virtual void UpdateState() { CheckSwitchState(); }
    public virtual void ExitState() { }
    public virtual void CheckSwitchState() { }
    public virtual void InitializeSubState() { }

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
            _currentSubState.UpdateStates();
    }

    protected void SwitchState(BaseState newState)
    {
        ExitState();
        if (IsRootState || null != _currentSubState)
        {
            _currentSubState?.ExitState();
            _currentSubState?._currentSubState?.ExitState(); 
        }

        newState.EnterState();

        if (IsRootState) 
        {
            StateMachine.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
    }

    public void SetSuperState(BaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    public void SetSubState(BaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
