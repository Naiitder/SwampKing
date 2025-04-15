using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private EnemyBaseState _currentState;
    private EnemyStateFactory _states;

    public EnemyAnimatorController EnemyAnimatorController {get; private set;}
    
    public EnemyBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public EnemyStateFactory States { get { return _states; } set { _states = value; } }
    
    private void Start()
    {
        _states = new EnemyStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        
    }
    
    private void Update()
    {
        _currentState.UpdateStates();
    }
}
