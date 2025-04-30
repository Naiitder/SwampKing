using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        _ctx.Agent.speed = _ctx.runningSpeed;
        _ctx.EnemyManager.IsChasing = true;

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
        _ctx.EnemyManager.IsChasing = false;

    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting && _ctx.profile.canReact) SwitchState(_factory.Reaction());
        
        if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange()) SwitchState(_factory.Idle());
        else if (_ctx.IsInStrafeRange() && _ctx.profile.canStrafe) SwitchState(_factory.Strafe());
        else if (_ctx.IsInAttackRange()) SwitchState(_factory.Attack());
        
    }
}
