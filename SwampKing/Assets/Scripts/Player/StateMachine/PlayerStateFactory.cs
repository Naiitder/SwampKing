
public class PlayerStateFactory 
{
    private PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle() {
        return new PlayerIdleState();
    }
    public PlayerBaseState Walk() {
        return new PlayerWalkState();
    }
    public PlayerBaseState Jump() {
        return new PlayerJumpState();
    }
    public PlayerBaseState Grounded() {
        return new PlayerGroundedState();
    }
}
