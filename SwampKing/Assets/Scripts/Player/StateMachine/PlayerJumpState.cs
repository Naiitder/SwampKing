using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private bool _hasLeftGround;

    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {
        _isRootState = true;
        InitializeSubState();
    }
    public override void EnterState() {
        _ctx.PlayerManager.IsJumping = true;
        _hasLeftGround = false;
    }
    public override void UpdateState(){
        CheckSwitchStates();
        _ctx.PlayerMovement.HandleJump();
        _ctx.PlayerMovement.HandleGravity();

        if (!_hasLeftGround && !_ctx.PlayerMovement.CharacterController.isGrounded)
        {
            _hasLeftGround = true;
        }
    }
    public override void ExitState() {
        _ctx.PlayerMovement.CancelJumpAnimation();
    }
    public override void InitializeSubState() {
        if (!_ctx.PlayerManager.IsChargingJumping) SetSubState(_factory.Walk());
        else SetSubState(_factory.Idle());
    }
    public override void CheckSwitchStates() {
        if (_hasLeftGround && _ctx.PlayerMovement.CharacterController.isGrounded) SwitchState(_factory.Grounded());
    }

}
