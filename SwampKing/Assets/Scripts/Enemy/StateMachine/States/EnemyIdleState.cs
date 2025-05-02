using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        
        _ctx.Agent.stoppingDistance = 0f;
        _ctx.Agent.SetDestination(_ctx.transform.position);
        _ctx.EnemyManager.IsIdle = true;
    } 
    public override void UpdateState(){
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.EnemyManager.IsIdle = false;

    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting && _ctx.profile.canReact) SwitchState(_factory.Reaction());
        
        if (_ctx.PlayerTarget != null)
        {
            if(_ctx.profile.attacksFromDistance && _ctx.IsInShootingRange())
                SwitchState(_factory.RangedAttack());
            else if(_ctx.profile.canMeleeAttack && _ctx.IsInAttackRange() && _ctx.profile.canMeleeAttack)
                SwitchState(_factory.Attack());
            else if (_ctx.IsInStrafeRange() && _ctx.profile.canStrafe) SwitchState(_factory.Strafe());
            else if (_ctx.IsInChaseRange()) SwitchState(_factory.Chase());
        }
    }
}
