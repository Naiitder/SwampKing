using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        
        _ctx.Agent.stoppingDistance = 0f;
        _ctx.Agent.SetDestination(_ctx.transform.position);
        _ctx.EnemyManager.IsIdle = true;
    } 
    public override void UpdateState(){
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.EnemyManager.IsIdle = false;

    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting) SwitchState(_factory.Reaction());
        
        if (_ctx.PlayerTarget != null)
        {
            if (_ctx.IsInStrafeRange()) SwitchState(_factory.Strafe());
            else if (_ctx.IsInChaseRange()) SwitchState(_factory.Chase());
        }
    }
}
