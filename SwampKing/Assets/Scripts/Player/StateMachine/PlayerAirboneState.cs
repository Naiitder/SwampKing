
public class PlayerAirboneState : PlayerBaseState
{
    private bool _hasLeftGround;

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
        _hasLeftGround = false;
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleGravity();

        if (!_hasLeftGround && !_ctx.PlayerMovement.CharacterController.isGrounded)
        {
            _hasLeftGround = true;
        }
    }
}
