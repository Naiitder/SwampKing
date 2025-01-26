
public class PlayerChargeJumpState : PlayerBaseState
{
    public PlayerChargeJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {}

    public override void CheckSwitchStates()
    {
    }

    public override void EnterState()
    {
        _ctx.PlayerMovement.StopMovement();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {

    }

}
