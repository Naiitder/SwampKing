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
            if (_ctx.IsInStrafeRange()) SetSubState(_factory.Strafe());
            else if(_ctx.IsInChaseRange()) SetSubState(_factory.Chase());
            else SetSubState(_factory.Idle());
        }
        
        //Todo Idle or Patrol
        else SetSubState(_factory.Idle());
    }
    public override void CheckSwitchStates(){
        
    }

    public void UpdateAnimatorValues()
    {
        if (!_ctx.EnemyManager.IsChasing && !_ctx.EnemyManager.IsIdle)
        {
            Vector3 velocity = _ctx.Agent.velocity;
            Vector3 localVelocity = _ctx.transform.InverseTransformDirection(velocity);
    
            float vertical = Mathf.Clamp(localVelocity.z / _ctx.Agent.speed, -1f, 1f);
            float horizontal = Mathf.Clamp(localVelocity.x / _ctx.Agent.speed, -1f, 1f);

            _ctx.EnemyAnimatorController.Animator.SetFloat("Vertical", vertical);
            _ctx.EnemyAnimatorController.Animator.SetFloat("Horizontal", horizontal);
        }
        else if (_ctx.EnemyManager.IsIdle)
        {
            _ctx.EnemyAnimatorController.Animator.SetFloat("Vertical", 0);
            _ctx.EnemyAnimatorController.Animator.SetFloat("Horizontal", 0);
        }
        else if (_ctx.EnemyManager.IsChasing)
        {
            _ctx.EnemyAnimatorController.Animator.SetFloat("Vertical", 2);
            _ctx.EnemyAnimatorController.Animator.SetFloat("Horizontal", 0);
        }
        
    }

}
