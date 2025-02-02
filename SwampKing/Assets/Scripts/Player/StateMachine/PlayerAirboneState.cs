
public class PlayerAirboneState : PlayerBaseState
{

    public PlayerAirboneState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }
    public override void CheckSwitchStates()
    {

    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {
        if (_ctx.PlayerManager.InAirTimer <= 0.1f
            && InputController.instance.CheckActions(InputController.InputActionType.Jump)) SetSubState(_factory.Jump());
        else if (_ctx.PlayerManager.InAirTimer > 0.1f 
            && InputController.instance.CheckActions(InputController.InputActionType.Jump)
            && _ctx.PlayerManager.CanDoubleJump) SetSubState(_factory.DoubleJump());
        else SetSubState(_factory.Falling());
    }

    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleGravity();
        CheckSwitchStates();
    }
}
