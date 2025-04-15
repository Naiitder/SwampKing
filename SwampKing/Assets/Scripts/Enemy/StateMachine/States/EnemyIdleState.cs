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
        //Trigger Idle Flag EnemyManager
        //Disable Movement
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
        
    }
}
