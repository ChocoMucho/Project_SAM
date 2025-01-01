using System.Collections.Generic;
using System;
using UnityEngine;

public class StateProvider
{
    private StateMachine stateMachine;

    private Dictionary<Enum, BaseState> _states = new Dictionary<Enum, BaseState>();

    public StateProvider(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        SetStates();
    }

    private void SetStates()
    {
        // TODO: 상태들 세팅하기
        SetState(PlayerStates.Ground, new GroundState(stateMachine, this));
        SetState(PlayerStates.Air, new AirState(stateMachine, this));

        SetState(PlayerStates.Idle, new IdleState(stateMachine, this));
        SetState(PlayerStates.Run, new RunState(stateMachine, this));
        SetState(PlayerStates.Aim, new AimState(stateMachine, this));
        SetState(PlayerStates.Reload, new ReloadState(stateMachine, this));
    }

    public BaseState GetState<PlayerStates>(PlayerStates stateEnum) where PlayerStates : Enum
    {
        if (_states.ContainsKey(stateEnum))
            return _states[stateEnum];
        else
            throw new InvalidOperationException("잘못된 상태 요구");
    }

    public void SetState<PlayerStates>(PlayerStates stateEnum, BaseState state) where PlayerStates : Enum
    {
        if (_states.ContainsKey(stateEnum))
            return;

        _states[stateEnum] = state;
    }
}
