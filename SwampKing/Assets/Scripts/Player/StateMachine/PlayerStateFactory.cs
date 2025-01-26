using System.Collections.Generic;

public class PlayerStateFactory 
{
    enum PlayerStates
    {
        grounded,
        idle,
        walk,
        chargeJump,
        airbone,
        doubleJump,
        jump,
        fall
    }

    private PlayerStateMachine _context;
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.idle] = new PlayerIdleState(_context,this);
        _states[PlayerStates.walk] = new PlayerWalkState(_context,this);
        _states[PlayerStates.chargeJump] = new PlayerChargeJumpState(_context,this);
        _states[PlayerStates.airbone] = new PlayerAirboneState(_context,this);
        _states[PlayerStates.jump] = new PlayerJumpState(_context,this);
        _states[PlayerStates.doubleJump] = new PlayerDoubleJumpState(_context,this);
        _states[PlayerStates.fall] = new PlayerFallingState(_context,this);
    }

    public PlayerBaseState Idle() {
        return _states[PlayerStates.idle];
    }
    public PlayerBaseState Walk() {
        return _states[PlayerStates.walk];
    }
    public PlayerBaseState Jump() {
        return _states[PlayerStates.jump];
    }
    public PlayerBaseState ChargeJump()
    {
        return _states[PlayerStates.chargeJump];
    }
    public PlayerBaseState DoubleJump()
    {
        return _states[PlayerStates.doubleJump];
    }
    public PlayerBaseState Grounded() {
        return _states[PlayerStates.grounded];
    }

    public PlayerBaseState Airbone()
    {
        return _states[PlayerStates.airbone];
    }

    public PlayerBaseState Falling()
    {
        return _states[PlayerStates.fall];
    }
}
