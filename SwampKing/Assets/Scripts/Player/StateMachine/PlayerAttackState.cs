using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int attackCount = 0;

    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
        else if (InputController.instance.MoveAmount == 0) SwitchState(_factory.Idle());
        else if (InputController.instance.MoveAmount > 0) SwitchState(_factory.Walk());
        else if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
    }

    public override void EnterState()
    {
        _ctx.PlayerManager.IsAttacking = true;
        InputController.instance.InputBuffer.Dequeue();
    }

    public override void ExitState()
    {
        _ctx.PlayerManager.IsAttacking = false;
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {

        CheckSwitchStates();
    }
}
