
public class PlayerChargeJumpState : PlayerBaseState
{
    public PlayerChargeJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {}

    public override void CheckSwitchStates()
    {
        if (!InputController.instance.IsJumpPressed) SwitchState(_factory.Airbone());
    }

    public override void EnterState()
    {
        _ctx.PlayerMovement.StopMovement();
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsChargingJumpHash, true);
    }

    public override void ExitState()
    {
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsChargingJumpHash, false);
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

}
