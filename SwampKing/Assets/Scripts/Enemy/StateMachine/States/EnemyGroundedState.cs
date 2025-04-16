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
        _ctx.EnemyManager.IsGrounded = true;
        InitializeSubState();
    } 
    public override void UpdateState(){
        
        CheckSwitchStates();
    }
    public override void ExitState(){}
    public override void InitializeSubState(){
        if (_ctx.PlayerTarget != null)
        {
            if (_ctx.IsInStrafeRange()) SetSubState(_factory.Strafe());
            else if(_ctx.IsInChaseRange()) SetSubState(_factory.Chase());
            else SetSubState(_factory.Idle());
        }
        
        //Todo Idle or Patrol
        else SetSubState(_factory.Idle());
    }
    public override void CheckSwitchStates(){
        
    }

}
