
public class PlayerChargeJumpState : PlayerBaseState
{
    public PlayerChargeJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {}

    public override void CheckSwitchStates()
    {
        if(_ctx.PlayerManager.IsDead) SwitchState(_factory.Dead());
        else if(_ctx.PlayerManager.IsReacting) SwitchState(_factory.Reaction());
        else if(_ctx.PlayerManager.IsDrowned) SwitchState(_factory.Drown());
        
        if (InputController.instance.CheckActions(InputController.InputActionType.Jump)) SwitchState(_factory.Airbone());
    }
    
    public override void FixedUpdateState()
    {
        
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
