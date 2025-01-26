
public class PlayerJumpState : PlayerBaseState
{
    float multiplierJumpForce;
    bool _hasLeftGround;
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {
    }
    public override void EnterState() {
        multiplierJumpForce = _ctx.PlayerManager.JumpChargeTime > _ctx.PlayerManager.TapTreshold ? 1.5f : 1f;
        _ctx.PlayerMovement.PerformJump(multiplierJumpForce);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsJumpingHash, true);
        if (InputController.instance.IsJumpPressed) InputController.instance.RequireNewJumpPress = true;
        _ctx.PlayerManager.IsJumping = true;
        _hasLeftGround = false;

    }
    public override void UpdateState(){
        if (!_hasLeftGround && !_ctx.PlayerMovement.CharacterController.isGrounded) _hasLeftGround = true;
        _ctx.PlayerMovement.HandleGroundedMovement();
        _ctx.PlayerMovement.HandleRotation();
        CheckSwitchStates();
    }
    public override void ExitState() {
        _ctx.PlayerMovement.CancelJumpAnimation();
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsJumpingHash, false);
        _ctx.PlayerManager.JumpChargeTime = 0;
        _ctx.PlayerManager.IsJumping = false;
    }
    public override void InitializeSubState() {

    }
    public override void CheckSwitchStates() {
        if (_ctx.PlayerManager.CanDoubleJump && InputController.instance.IsJumpPressed && !InputController.instance.RequireNewJumpPress
            && !_ctx.PlayerMovement.CharacterController.isGrounded) SwitchState(_factory.DoubleJump());
        else if (_ctx.PlayerMovement.CharacterController.isGrounded && _hasLeftGround) SwitchState(_factory.Grounded());
    }

}
