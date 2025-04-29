using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStrafeState : EnemyBaseState
{
    public EnemyStrafeState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    { }

    private float currentAngle = 0f;
    public float orbitSpeed = 60f;                
    public float orbitSpeedVariation = 20f;         
    public float strafeRangeVariation = 0.5f;         
    public float modifierUpdateInterval = 1f;       

    private float randomSpeedModifier = 0f;         
    private float randomRangeModifier = 0f;         
    private float lastModifierUpdateTime = 0f;
    private int orbitDirection = 1;                 

    public override void EnterState()
    {
        _ctx.Agent.speed = _ctx.movementSpeed;

    } 

    public override void UpdateState()
    {
        if (_ctx.PlayerTarget != null)
        {
            _ctx.Agent.stoppingDistance = _ctx.AttackRange;
            StrafeAroundPlayer();
        }
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }
    
    public override void InitializeSubState() {}

    public override void CheckSwitchStates()
    {       
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting && _ctx.profile.canReact) SwitchState(_factory.Reaction());
        
        if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange())
            SwitchState(_factory.Idle());
        else if (_ctx.IsInAttackRange())
            SwitchState(_factory.Attack());
        else if (!_ctx.IsInStrafeRange() && _ctx.IsInChaseRange())
            SwitchState(_factory.Chase());
    }
    
    void StrafeAroundPlayer()
    {
        if (Time.time - lastModifierUpdateTime > modifierUpdateInterval)
        {
            randomSpeedModifier = Random.Range(-orbitSpeedVariation, orbitSpeedVariation);
            randomRangeModifier = Random.Range(-strafeRangeVariation, strafeRangeVariation);
            
            if (Random.value < 0.2f)
            {
                orbitDirection *= -1;
            }
            lastModifierUpdateTime = Time.time;
        }
        
        float effectiveOrbitSpeed = orbitSpeed + randomSpeedModifier;
        currentAngle += orbitDirection * effectiveOrbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;
        if (currentAngle < 0f) currentAngle += 360f;
        
        float effectiveStrafeRange = _ctx.StrafeRange + randomRangeModifier;
        Vector3 offset = new Vector3(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * effectiveStrafeRange;

        Vector3 strafeTarget = _ctx.PlayerTarget.position + offset;
        _ctx.Agent.SetDestination(strafeTarget);

        FaceTarget(_ctx.PlayerTarget.position);
    }
    
    void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - _ctx.transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }
}