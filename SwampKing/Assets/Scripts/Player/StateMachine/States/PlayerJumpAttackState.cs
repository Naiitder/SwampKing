using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttackState : PlayerBaseState
{
    bool attackFinished = false;
    public PlayerJumpAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_ctx.PlayerMovement.isGrounded() && attackFinished) 
            SwitchState(_factory.Grounded());
    }
    
    public override void FixedUpdateState()
    {
        
    }

    public override void EnterState()
    {
        attackFinished = false;
        _ctx.PlayerManager.IsAttacking = true;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.JumpAttackHash, true);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AttackFinishedHash, false);
        _ctx.PlayerMovement.StopMovement();
   //     _ctx.PlayerAnimator.Animator.applyRootMotion = true;
        
        InputController.instance.InputBuffer.Dequeue();
    }

    public override void ExitState()
    {
        _ctx.PlayerManager.IsAttacking = false;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.JumpAttackHash, false);
     //   _ctx.PlayerAnimator.Animator.applyRootMotion = false;
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
