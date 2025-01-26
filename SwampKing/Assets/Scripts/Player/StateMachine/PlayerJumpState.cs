using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    float multiplierJumpForce;
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {
    }
    public override void EnterState() {
        InitializeSubState();
        _ctx.PlayerMovement.HandleJump();
        _ctx.PlayerManager.IsJumping = true;
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void ExitState() {
        _ctx.PlayerMovement.CancelJumpAnimation();
    }
    public override void InitializeSubState() {

    }
    public override void CheckSwitchStates() {

    }

}
