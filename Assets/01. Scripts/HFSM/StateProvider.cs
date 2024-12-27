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

        // TODO: 상태들 세팅하기
/*        SetState(PlayerStates.Ground, new PlayerState_Ground(stateMachine, this));
        SetState(PlayerStates.Air, new PlayerState_Air(stateMachine, this));

        SetState(PlayerStates.Move, new PlayerState_Move(stateMachine, this));
        SetState(PlayerStates.Dodge, new PlayerState_Dodge(stateMachine, this));

        SetState(PlayerStates.NonCombat, new PlayerState_NonCombat(stateMachine, this));
        SetState(PlayerStates.Shoot, new PlayerState_Shoot(stateMachine, this));*/
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
