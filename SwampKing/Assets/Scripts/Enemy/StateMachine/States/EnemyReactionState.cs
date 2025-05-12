using UnityEngine;

public class EnemyReactionState : EnemyBaseState
{
    private bool hasReacted = false;
    public EnemyReactionState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        hasReacted = false;
        
        _ctx.Agent.stoppingDistance = 0f;
        _ctx.Agent.SetDestination(_ctx.transform.position);
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsReactingHash, true);
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.ReactionFinishedHash, false);
    } 
    public override void UpdateState(){
        
        if (_ctx.EnemyAnimatorController.Animator.GetBool(_ctx.EnemyAnimatorController.ReactionFinishedHash))
        {
            hasReacted = true;
        }
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.EnemyManager.IsReacting = false;
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsReactingHash, false);


    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        
        if (!hasReacted) return;
        
        if (_ctx.PlayerTarget != null)
        {
            if(_ctx.IsInAttackRange() && _ctx.profile.canMeleeAttack) SwitchState(_factory.Attack());
            else if (_ctx.IsInShootingRange() && _ctx.profile.attacksFromDistance) SwitchState(_factory.RangedAttack());
            else if (_ctx.profile.canRetreat && (_ctx.IsInStrafeRange() && _ctx.PlayerManager.IsAttacking)) SwitchState(_factory.Backing());
            else if (_ctx.IsInStrafeRange() && _ctx.profile.canStrafe) SwitchState(_factory.Strafe());
            else if (_ctx.IsInChaseRange()) SwitchState(_factory.Chase());
            else SwitchState(_factory.Idle());
        }
        else SwitchState(_factory.Idle());
    }
}
