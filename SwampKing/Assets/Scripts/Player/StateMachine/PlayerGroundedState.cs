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
        if (_ctx.PlayerManager.JumpChargeTime >= _ctx.PlayerManager.TapTreshold && InputController.instance.IsJumpPressed) SetSubState(_factory.ChargeJump());
        else if (InputController.instance.MoveAmount != 0) SetSubState(_factory.Walk());
        else SetSubState(_factory.Idle());
    }
    public override void CheckSwitchStates(){
        if (!InputController.instance.IsJumpPressed && !InputController.instance.RequireNewJumpPress 
            && _ctx.PlayerManager.JumpChargeTime >= 0.05f
            || !_ctx.PlayerMovement.CharacterController.isGrounded) 
                SwitchState(_factory.Airbone());
    }

}
