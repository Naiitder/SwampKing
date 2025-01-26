
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
    } 
    public override void UpdateState(){
        _ctx.PlayerAnimator.UpdateMovementAnimationValues(InputController.instance.MoveAmount, 0);
        CheckSwitchStates();
    }
    public override void ExitState(){}
    public override void InitializeSubState(){
        if (_ctx.PlayerManager.JumpChargeTime >= _ctx.PlayerManager.TapTreshold && InputController.instance.IsJumpPressed) SetSubState(_factory.ChargeJump());
        else if (InputController.instance.MoveAmount != 0) SetSubState(_factory.Walk());
        else SetSubState(_factory.Idle());
    }
    public override void CheckSwitchStates(){
        if (!InputController.instance.IsJumpPressed 
            && _ctx.PlayerManager.JumpChargeTime >= 0.05f
            || !_ctx.PlayerMovement.CharacterController.isGrounded) 
                SwitchState(_factory.Airbone());
    }

}
