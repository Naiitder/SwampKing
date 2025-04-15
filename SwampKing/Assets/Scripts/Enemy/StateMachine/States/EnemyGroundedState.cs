using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundedState : EnemyBaseState
{
    public EnemyGroundedState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
        _isRootState = true;
    }

    public override void EnterState(){
        InitializeSubState();
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
