

public class PlayerDoubleJumpState : PlayerBaseState
{
    public PlayerDoubleJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    { }

    public override void CheckSwitchStates()
    {
        if (_ctx.PlayerMovement.CharacterController.isGrounded) SwitchState(_factory.Grounded());
    }

    public override void EnterState()
    {
        _ctx.PlayerMovement.PerformJump(1.25f);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsDoubleJumpingHash, true);
        _ctx.PlayerManager.IsJumping = true;
        _ctx.PlayerManager.CanDoubleJump = false;
        InputController.instance.InputBuffer.Dequeue();
    }

    public override void ExitState()
    {
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsDoubleJumpingHash, false);
        _ctx.PlayerManager.JumpChargeTime = 0;
        _ctx.PlayerManager.IsJumping = false;
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleGroundedMovement();
        _ctx.PlayerMovement.HandleRotation();
        CheckSwitchStates();
    }
}
