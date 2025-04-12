using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttackState : PlayerBaseState
{
    public PlayerJumpAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {

    }

    public override void EnterState()
    {
        _ctx.PlayerManager.IsAttacking = true;
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
