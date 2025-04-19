using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateFactory 
{
    enum EnemyStates
    {
        grounded,
        idle,
        patrol,
        chase,
        strafe,
        attack,
        reaction, 
        die,
        airbone,
        fall,
    }
    
    private EnemyStateMachine _context;
    Dictionary<EnemyStates, EnemyBaseState> _states = new Dictionary<EnemyStates, EnemyBaseState>();
    
    public EnemyStateFactory(EnemyStateMachine currentContext)
    {
        _context = currentContext;
        _states[EnemyStates.grounded] = new EnemyGroundedState(_context, this);
        _states[EnemyStates.idle] = new EnemyIdleState(_context, this);
        _states[EnemyStates.chase] = new EnemyChaseState(_context, this);
        _states[EnemyStates.strafe] = new EnemyStrafeState(_context, this);
        _states[EnemyStates.attack] = new EnemyAttackState(_context, this);
    }

    public EnemyBaseState Grounded() {
        return _states[EnemyStates.grounded];
    }
    
    public EnemyBaseState Idle() {
        return _states[EnemyStates.idle];
    }
    
    public EnemyBaseState Patrol() {
        return _states[EnemyStates.patrol];
    }
    
    public EnemyBaseState Chase() {
        return _states[EnemyStates.chase];
    }    
    
    public EnemyBaseState Strafe() {
        return _states[EnemyStates.strafe];
    }
    
    public EnemyBaseState Attack() {
        return _states[EnemyStates.attack];
    }
    
    public EnemyBaseState Reaction() {
        return _states[EnemyStates.reaction];
    }
    
    public EnemyBaseState Die() {
        return _states[EnemyStates.die];
    }
    
    public EnemyBaseState Airbone() {
        return _states[EnemyStates.airbone];
    }
    
    public EnemyBaseState Fall() {
        return _states[EnemyStates.fall];
    }
    
}
