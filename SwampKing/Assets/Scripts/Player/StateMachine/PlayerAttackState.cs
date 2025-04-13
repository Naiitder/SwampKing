using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int attackCount = 0;
    private int currentAttackHash;

    private bool attackFinished = false;

    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
            if (attackFinished)
            {
                if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
                else if (InputController.instance.MoveAmount == 0) SwitchState(_factory.Idle());
                else if (InputController.instance.MoveAmount > 0) SwitchState(_factory.Walk());
                else if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
            }
    }

    public override void EnterState()
    {
        attackFinished = false;
        _ctx.PlayerManager.IsAttacking = true;
        _ctx.PlayerAnimator.Animator.applyRootMotion = true;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AttackFinishedHash, false);

        _ctx.PlayerMovement.StopMovement();

        if (attackCount == 0)
        {
            currentAttackHash = _ctx.PlayerAnimator.SimpleAttackHash1;
            attackCount++;
            _ctx.PlayerAnimator.Animator.SetBool(currentAttackHash, true);
        }
        else if (attackCount == 1)
        {
            attackCount++;
            _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash2, true);
        }
        else if (attackCount == 2)
        {
            attackCount = 0;
            _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash3, true);
        }

        InputController.instance.InputBuffer.Dequeue();
    }


    public override void ExitState()
    {
        _ctx.PlayerManager.IsAttacking = false;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash1, false);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash2, false);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash3, false);
        _ctx.PlayerAnimator.Animator.applyRootMotion = false;

    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {

        if (_ctx.PlayerAnimator.Animator.GetBool(_ctx.PlayerAnimator.AttackFinishedHash))
        {
            attackFinished = true;
        }

        CheckSwitchStates();
    }
}
