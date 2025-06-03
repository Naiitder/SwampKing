
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
        _ctx.PlayerManager.InAirTimer = 0;
        _ctx.PlayerManager.CanDoubleJump = true;
        _ctx.PlayerManager.IsGrounded = true;

    } 
    public override void UpdateState(){
        HandleJumpCharge();

        _ctx.HandleFootSteepsSound();
        
        if(!_ctx.PlayerManager.IsAiming) _ctx.PlayerAnimator.UpdateMovementAnimationValues(InputController.instance.MoveAmount, 0);
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState()
    {
        
    }

    public override void ExitState()
    { }
    public override void InitializeSubState(){
        if (InputController.instance.IsAimingPressed) SetSubState(_factory.Aimning());
        else if (InputController.instance.MoveAmount != 0) SetSubState(_factory.Walk());
        else SetSubState(_factory.Idle());
    }
    public override void CheckSwitchStates(){
        if ((InputController.instance.CheckActions(InputController.InputActionType.Jump) && !_ctx.PlayerManager.IsDead)
            || !_ctx.PlayerMovement.isGrounded()) 
                SwitchState(_factory.Airbone());
    }
    
    
    private void HandleJumpCharge()
    {
        if (InputController.instance.IsJumpPressed 
            && _ctx.PlayerMovement.CharacterController.isGrounded 
            && !_ctx.PlayerManager.IsDead
            && !_ctx.PlayerManager.IsReacting
            && !_ctx.PlayerManager.IsAiming
            && !_ctx.PlayerManager.IsAttacking)
        {
            if (_ctx.PlayerManager.JumpChargeTime >= _ctx.PlayerManager.TapTreshold) _ctx.PlayerManager.IsChargingJumping = true;
            _ctx.PlayerManager.JumpChargeTime += Time.deltaTime;
        }
        else _ctx.PlayerManager.IsChargingJumping = false;
    }
    
}
