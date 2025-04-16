using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        
        _ctx.EnemyAnimatorController.Animator.SetBool(
            _ctx.EnemyAnimatorController.IsIdleHash,true);
        //Enable Idle Flag
        _ctx.Agent.stoppingDistance = 0f;
        _ctx.Agent.SetDestination(_ctx.transform.position);
    } 
    public override void UpdateState(){
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.EnemyAnimatorController.Animator.SetBool(
            _ctx.EnemyAnimatorController.IsIdleHash,false);
        //Diable Idle Flag EnemyManager
    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.PlayerTarget != null)
        {
            if (_ctx.IsInStrafeRange()) SwitchState(_factory.Strafe());
            else if (_ctx.IsInChaseRange()) SwitchState(_factory.Chase());
        }
    }
}
