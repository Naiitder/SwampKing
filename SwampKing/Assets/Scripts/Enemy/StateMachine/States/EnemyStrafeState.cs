using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStrafeState : EnemyBaseState
{
    public EnemyStrafeState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    private float currentAngle = 0f;
    public float strafeDistance = 4f;
    public float orbitSpeed = 60f;
    
    public override void EnterState(){
        
        //Enable Strafe Anim
        _ctx.Agent.speed = _ctx.movementSpeed;
    } 
    public override void UpdateState(){

        if (_ctx.PlayerTarget != null)
        {
            _ctx.Agent.stoppingDistance = _ctx.AttackRange;
            StrafeAroundPlayer();
        }
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        //Disable Enemy Strafe Anim
    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange()) SwitchState(_factory.Idle());
        else if (!_ctx.IsInStrafeRange() && _ctx.IsInChaseRange()) SwitchState(_factory.Chase());
        
    }
    
    void StrafeAroundPlayer()
    {
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        // Orbit target point
        Vector3 offset = new Vector3(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * strafeDistance;

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
