using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
    } 
    public override void UpdateState(){
        
        CheckSwitchStates();
    }
    public override void ExitState(){}
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        
    }
}
