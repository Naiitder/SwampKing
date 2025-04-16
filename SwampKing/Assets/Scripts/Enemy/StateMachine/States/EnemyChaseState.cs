using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsChasingHash,true);
        _ctx.Agent.speed = _ctx.runningSpeed;
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
        
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsChasingHash,false);

    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange()) SwitchState(_factory.Idle());
        else if (_ctx.IsInStrafeRange()) SwitchState(_factory.Strafe());
        
    }
}
