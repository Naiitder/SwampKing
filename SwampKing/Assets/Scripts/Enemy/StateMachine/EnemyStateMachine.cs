using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    private EnemyBaseState _currentState;
    private EnemyStateFactory _states;

    public EnemyAnimatorController EnemyAnimatorController {get; private set;}
    public EnemyManager EnemyManager {get; private set;}

    public Transform PlayerTarget {get; private set;}
    public NavMeshAgent Agent {get; private set;}
    
    [Header("Dettection/Chase Stats")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float strafeRange = 4f;
    [SerializeField] private float attackRange = 2f;
    
    //Obtain via Database
    [Header("Movement Stats")]
    public float movementSpeed = 4f;
    public float rotationSpeed = 15f;
    
    public EnemyBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public EnemyStateFactory States { get { return _states; } set { _states = value; } }
    public float AttackRange { get { return attackRange; } set { attackRange = value; } }
    
    private void Start()
    {
        _states = new EnemyStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        PlayerTarget = InputController.instance.GetComponent<Transform>();
        Agent = GetComponent<NavMeshAgent>();
        EnemyManager = GetComponent<EnemyManager>();
        EnemyAnimatorController = GetComponent<EnemyAnimatorController>();
    }
    
    private void Update()
    {
        _currentState.UpdateStates();
    }

    public bool IsInChaseRange()
    {
        return Vector3.Distance(transform.position, PlayerTarget.position) <= chaseRange;
    }
    
    public bool IsInStrafeRange()
    {
        return Vector3.Distance(transform.position, PlayerTarget.position) <= strafeRange;
    }
    
    public bool IsInAttackRange()
    {
        return Vector3.Distance(transform.position, PlayerTarget.position) <= attackRange;
    }
}
