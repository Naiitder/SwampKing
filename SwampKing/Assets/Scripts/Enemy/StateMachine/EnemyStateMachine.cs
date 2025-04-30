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
    public EnemyProfile profile;

    public PlayerManager PlayerManager {get; private set;}
    public Transform PlayerTarget {get; private set;}
    public NavMeshAgent Agent {get; private set;}
    
    
    [Header("Dettection/Chase Stats")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float strafeRange = 4f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float shottingRange = 6f;
    
    //Obtain via Database
    [Header("Movement Stats")]
    public float movementSpeed = 1.5f;
    public float runningSpeed = 5f;
    public float rotationSpeed = 15f;
    
    public EnemyBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public EnemyStateFactory States { get { return _states; } set { _states = value; } }
    public float AttackRange { get { return attackRange; } set { attackRange = value; } }
    public float StrafeRange { get { return strafeRange; } set { strafeRange = value; } }
    
    private void Start()
    {
        PlayerManager = InputController.instance.GetComponent<PlayerManager>();
        PlayerTarget = PlayerManager.GetComponent<Transform>();
        Agent = GetComponent<NavMeshAgent>();
        EnemyManager = GetComponent<EnemyManager>();
        EnemyAnimatorController = GetComponent<EnemyAnimatorController>();
        
        ApplyProfile();
        
        _states = new EnemyStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        
    }
    
    private void Update()
    {
        _currentState.UpdateStates();
        HandleAttackCounter();
    }
    
    private void ApplyProfile()
    {
        if (profile != null)
        {
            chaseRange = profile.chaseRange;
            strafeRange = profile.strafeRange;
            attackRange = profile.attackRange;
            movementSpeed = profile.movementSpeed;
            runningSpeed = profile.runningSpeed;
            rotationSpeed = profile.rotationSpeed;
            shottingRange = profile.shootingRange;
        }
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
    
    
    public bool IsInShootingRange()
    {
        return Vector3.Distance(transform.position, PlayerTarget.position) <= shottingRange;
    }
    private void HandleAttackCounter()
    {
        if (!EnemyManager.IsAttacking)
        {
            if (EnemyManager.PreviousIsAttacking)
            {
                EnemyManager.TimeSinceLastAttack = 0f;
                EnemyManager.PreviousIsAttacking = false;
            }
            else
            {
                EnemyManager.TimeSinceLastAttack += Time.deltaTime;
                if (EnemyManager.TimeSinceLastAttack > .5f)
                {
                    EnemyManager.AttackCount = 0;
                }
            }
        }
        else
        {
            EnemyManager.PreviousIsAttacking = true;
        }
    }
    
    private void OnAnimatorMove()
    {
        if (EnemyManager.IsAttacking)
        {
            Vector3 rootPosition = EnemyAnimatorController.Animator.rootPosition;
            transform.position = rootPosition;
            
        }
    }
}
