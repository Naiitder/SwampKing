using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundedState : EnemyBaseState
{
    public EnemyGroundedState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
        _isRootState = true;
    }

    public override void EnterState(){
        _ctx.EnemyManager.IsGrounded = true;
        InitializeSubState();
    } 
    public override void UpdateState()
    {

        UpdateAnimatorValues();
        
        CheckSwitchStates();
    }
    public override void ExitState(){}
    public override void InitializeSubState(){
        if (_ctx.PlayerTarget != null)
        {
            if (_ctx.IsInStrafeRange() && _ctx.profile.canStrafe) SetSubState(_factory.Strafe());
            else if(_ctx.IsInChaseRange()) SetSubState(_factory.Chase());
            else if(_ctx.profile.canPatrol) SetSubState(_factory.Patrol());
            else SetSubState(_factory.Idle());
        }
        
        else if(_ctx.profile.canPatrol) SetSubState(_factory.Patrol());
        else SetSubState(_factory.Idle());
    }
    public override void CheckSwitchStates(){
        
    }

    public void UpdateAnimatorValues()
    {
        Animator animator = _ctx.EnemyAnimatorController.Animator;
        float dampTime = 0.1f; 

        if (!_ctx.EnemyManager.IsChasing && !_ctx.EnemyManager.IsIdle)
        {
            Vector3 velocity = _ctx.Agent.velocity;
            Vector3 localVelocity = _ctx.transform.InverseTransformDirection(velocity);

            float vertical = Mathf.Clamp(localVelocity.z / _ctx.Agent.speed, -1f, 1f);
            float horizontal = Mathf.Clamp(localVelocity.x / _ctx.Agent.speed, -1f, 1f);

            animator.SetFloat("Vertical", vertical, dampTime, Time.deltaTime);
            animator.SetFloat("Horizontal", horizontal, dampTime, Time.deltaTime);
        }
        else if (_ctx.EnemyManager.IsIdle)
        {
            animator.SetFloat("Vertical", 0f, dampTime, Time.deltaTime);
            animator.SetFloat("Horizontal", 0f, dampTime, Time.deltaTime);
        }
        else if (_ctx.EnemyManager.IsChasing)
        {
            animator.SetFloat("Vertical", 2f, dampTime, Time.deltaTime);
            animator.SetFloat("Horizontal", 0f, dampTime, Time.deltaTime);
        }
    }


}
