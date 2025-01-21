using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{

    public PlayerFallingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();
        _ctx.PlayerMovement.FallingAnimationOn();
    }
    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleGravity();
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.PlayerMovement.FallingAnimationOff();
    }
    public override void InitializeSubState()
    {
        SetSubState(_factory.Walk());
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PlayerMovement.CharacterController.isGrounded) SwitchState(_factory.Grounded());
    }


}
