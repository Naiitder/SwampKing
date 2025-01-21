using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {
        _isRootState = true;
    }

    public override void EnterState(){
        InitializeSubState();
        _ctx.PlayerMovement.SetGravity();
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void ExitState(){}
    public override void InitializeSubState(){
        SetSubState(_factory.Walk());
    }
    public override void CheckSwitchStates(){
        if (InputController.instance.IsJumpPressed && !InputController.instance.RequireNewJumpPress) SwitchState(_factory.Jump());
        else if (!_ctx.PlayerMovement.CharacterController.isGrounded) SwitchState(_factory.Falling());
    }

}
