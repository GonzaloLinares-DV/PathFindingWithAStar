using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentStatesEnum
{
    Idle,
    Star,
    Patrol
}

public class StateMachine
{
    IState _currentState = new BlankState();
    Dictionary<AgentStatesEnum, IState> _allStates = new Dictionary<AgentStatesEnum, IState>();


    public void OnUpdate()
    {
        _currentState.OnUpdate();
    }

    public void AddState(AgentStatesEnum AgentID, IState state)
    {
        if (_allStates.ContainsKey(AgentID)) return;

        _allStates.Add(AgentID, state);
    }

    public void ChangeState(AgentStatesEnum AgentId)
    {
        if (!_allStates.ContainsKey(AgentId)) return;
        _currentState.OnExit();
        _currentState = _allStates[AgentId]; 
        _currentState.OnStart();
    }
}
