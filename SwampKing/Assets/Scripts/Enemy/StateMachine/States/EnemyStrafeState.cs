using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStrafeState : EnemyBaseState
{
    public EnemyStrafeState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        
        //Enable Strafe Anim
    } 
    public override void UpdateState(){

        if (_ctx.PlayerTarget != null)
        {
            _ctx.Agent.stoppingDistance = _ctx.AttackRange;
            _ctx.Agent.SetDestination(_ctx.PlayerTarget.position);
        }
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        //Disable Enemy Strafe Anim
    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange()) SwitchState(_factory.Idle());
        else if (!_ctx.IsInStrafeRange() && _ctx.IsInChaseRange()) SwitchState(_factory.Chase());
        
    }
}
