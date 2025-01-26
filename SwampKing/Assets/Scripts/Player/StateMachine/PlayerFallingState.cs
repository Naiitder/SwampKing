
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
    }
    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleGravity();
        if (!_ctx.PlayerAnimator.Animator.GetBool("isFalling") && _ctx.PlayerManager.InAirTimer > 0.125f) _ctx.PlayerAnimator.Animator.SetBool("isFalling", true);
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.PlayerAnimator.Animator.SetBool("isFalling", false);
    }
    public override void InitializeSubState()
    {
        SetSubState(_factory.Walk());
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PlayerMovement.CharacterController.isGrounded) SwitchState(_factory.Grounded());

        if (_ctx.PlayerManager.InAirTimer <= 0.125f && InputController.instance.IsJumpPressed && !InputController.instance.RequireNewJumpPress) SwitchState(_factory.Jump());
        else if (_ctx.PlayerManager.InAirTimer > 0.125f && InputController.instance.IsJumpPressed
            && !InputController.instance.RequireNewJumpPress && _ctx.PlayerManager.CanDoubleJump) SwitchState(_factory.DoubleJump());
    }


}
