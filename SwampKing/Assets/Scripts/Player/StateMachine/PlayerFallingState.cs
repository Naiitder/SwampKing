
public class PlayerFallingState : PlayerBaseState
{

    private float fallThreshold = 0.1f;

    public PlayerFallingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
    }
    public override void EnterState()
    {
    }
    public override void UpdateState()
    {
        if (!_ctx.PlayerAnimator.Animator.GetBool("isFalling") && _ctx.PlayerManager.InAirTimer > fallThreshold) 
            _ctx.PlayerAnimator.Animator.SetBool("isFalling", true);
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.PlayerAnimator.Animator.SetBool("isFalling", false);
    }
    public override void InitializeSubState()
    {
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PlayerMovement.CharacterController.isGrounded) 
            SwitchState(_factory.Grounded());

        if (_ctx.PlayerManager.InAirTimer <= fallThreshold
            && InputController.instance.CheckActions(InputController.InputActionType.Jump)) 
            SwitchState(_factory.Jump());
        else if (_ctx.PlayerManager.InAirTimer > fallThreshold
            && InputController.instance.CheckActions(InputController.InputActionType.Jump)
            && _ctx.PlayerManager.CanDoubleJump) 
            SwitchState(_factory.DoubleJump());
    }


}
