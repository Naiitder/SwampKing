using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{

    private int currentAttackHash;

    private bool attackFinished = false;

    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if(_ctx.PlayerManager.IsDead) SwitchState(_factory.Dead());
        else if(_ctx.PlayerManager.IsReacting) SwitchState(_factory.Reaction());
        else if(_ctx.PlayerManager.IsDrowned) SwitchState(_factory.Drown());
        
        if (attackFinished) 
        {
            if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
            else if (InputController.instance.CheckActions(InputController.InputActionType.Attack) && _ctx.PlayerManager.AttackCount != 0 ) SwitchState(_factory.Attack());
            else if (InputController.instance.IsAimingPressed) SwitchState(_factory.Aimning());
            else if (InputController.instance.MoveAmount == 0) SwitchState(_factory.Idle());
            else if (InputController.instance.MoveAmount > 0) SwitchState(_factory.Walk());

        }
    }
    public override void FixedUpdateState()
    {
        
    }

    public override void EnterState()
    {
        attackFinished = false;
        _ctx.PlayerManager.IsAttacking = true;
        _ctx.PlayerAnimator.Animator.applyRootMotion = true;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AttackFinishedHash, false);

        _ctx.PlayerMovement.StopMovement();

        if (_ctx.PlayerManager.AttackCount == 0)
        {
            currentAttackHash = _ctx.PlayerAnimator.SimpleAttackHash1;
            _ctx.PlayerManager.AttackCount++;
            _ctx.PlayerAnimator.Animator.SetBool(currentAttackHash, true);
            _ctx.AudioSource.pitch = 1;
            _ctx.AudioSource.PlayOneShot(_ctx.SimpleAttackSound);

        }
        else if (_ctx.PlayerManager.AttackCount == 1)
        {
            _ctx.PlayerManager.AttackCount++;
            _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash2, true);
            _ctx.AudioSource.pitch = .9f;
            _ctx.AudioSource.PlayOneShot(_ctx.SimpleAttackSound);

        }
        else if (_ctx.PlayerManager.AttackCount == 2)
        {
            _ctx.PlayerManager.AttackCount = 0;
            _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash3, true);
            _ctx.AudioSource.pitch = .8f;
            _ctx.AudioSource.PlayOneShot(_ctx.SimpleAttackSound);
        }
        _ctx.AudioSource.volume = .9f;

        InputController.instance.InputBuffer.Dequeue();
        
        _ctx.PlayerAnimator.DrawSword();
    }


    public override void ExitState()
    {
        _ctx.PlayerManager.IsAttacking = false;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash1, false);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash2, false);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.SimpleAttackHash3, false);
        _ctx.PlayerAnimator.Animator.applyRootMotion = false;
        
        _ctx.PlayerAnimator.OnAttackAnimationFinished();

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
        
        if (InputController.instance.CheckActions(InputController.InputActionType.Attack))
            _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsPreparingAttackHash,true);
        else _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsPreparingAttackHash,false);


        CheckSwitchStates();
    }



}
