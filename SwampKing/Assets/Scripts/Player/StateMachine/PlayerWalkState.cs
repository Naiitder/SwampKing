using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext, playerStateFactory) { }

    public override void EnterState() { }
    public override void UpdateState(){
        _ctx.PlayerMovement.HandleGroundedMovement();
        _ctx.PlayerMovement.HandleRotation();
        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.Idle());
    }
}
