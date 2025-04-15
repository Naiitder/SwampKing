using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        _ctx.EnemyAnimatorController.Animator.SetBool(,true);
    } 
    public override void UpdateState(){
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
        _ctx.EnemyAnimatorController.Animator.SetBool(,false);

    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        
    }
}
