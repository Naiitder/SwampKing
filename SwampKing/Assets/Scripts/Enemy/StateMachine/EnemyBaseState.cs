using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState
{
    protected bool _isRootState = false;
    protected EnemyStateMachine _ctx;
    protected EnemyStateFactory _factory;
    protected EnemyBaseState _currentSubState;
    protected EnemyBaseState _currentSuperState;
    
    public EnemyBaseState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory) {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }
    
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates() {
        UpdateState();
        if (_currentSubState != null) _currentSubState.UpdateState();
    }
    protected void SwitchState(EnemyBaseState newState) {
        ExitState();
        if (_currentSubState != null) _currentSubState.ExitState();
        if (newState._isRootState)
        {
            newState.EnterState();
            _ctx.CurrentState = newState;
        }
        else if (_currentSuperState != null) _currentSuperState.SetSubState(newState);
    }
    protected void SetSuperState(EnemyBaseState newSuperState) {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(EnemyBaseState newSubState) {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        _currentSubState.EnterState();
    }
}
