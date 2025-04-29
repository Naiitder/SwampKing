using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private bool attackFinished;
    private int currentAttackHash;
    
    public EnemyAttackState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    { }
    
    public override void EnterState()
    {
        _ctx.EnemyManager.IsAttacking = true;
        _ctx.EnemyAnimatorController.Animator.applyRootMotion = true;
        _ctx.Agent.SetDestination(_ctx.transform.position);
        _ctx.transform.LookAt(_ctx.PlayerTarget);
        attackFinished = false;
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.AttackFinishedHash, false);
        
        if (_ctx.EnemyManager.AttackCount == 0)
        {
            currentAttackHash = _ctx.EnemyAnimatorController.SimpleAttack1Hash;
            _ctx.EnemyManager.AttackCount++;
            _ctx.EnemyAnimatorController.Animator.SetBool(currentAttackHash, true);
        }
        else if (_ctx.EnemyManager.AttackCount == 1)
        {
            _ctx.EnemyManager.AttackCount++;
            _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.SimpleAttack2Hash, true);
        }
        else if (_ctx.EnemyManager.AttackCount == 2)
        {
            _ctx.EnemyManager.AttackCount = 0;
            _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.SimpleAttack3Hash, true);
        }

    } 

    public override void UpdateState()
    {
        if (_ctx.EnemyAnimatorController.Animator.GetBool(_ctx.EnemyAnimatorController.AttackFinishedHash))
        {
            attackFinished = true;
        }
        

        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.SimpleAttack1Hash,false);
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.SimpleAttack2Hash,false);
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.SimpleAttack3Hash,false);
        _ctx.EnemyManager.IsAttacking = false;
        _ctx.EnemyAnimatorController.Animator.applyRootMotion = false;

    }
    
    public override void InitializeSubState() {}

    public override void CheckSwitchStates()
    {
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting && _ctx.profile.canReact) SwitchState(_factory.Reaction());
        
        if (!attackFinished) return;
        
        bool canChainAttack = _ctx.profile.canChainAttacks && 
                              (
                                  (_ctx.IsInAttackRange() && _ctx.EnemyManager.AttackCount != 2) 
                                  || 
                                  (_ctx.PlayerManager != null && !_ctx.PlayerManager.IsAttacking)
                              );
        
        if (canChainAttack)
        {
            if (Random.value >= _ctx.profile.chanceToChainAttack)
            {
                _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsPreparingAttackHash,true);
                SwitchState(_factory.Attack()); 
                Debug.Log(_ctx.profile.canChainAttacks);
                return;
            }
        }
        
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsPreparingAttackHash,false);
        _ctx.EnemyManager.AttackCount = 0;
        
        if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange())
            SwitchState(_factory.Idle());
        
        else if (_ctx.profile.canRetreat && (_ctx.IsInAttackRange() || _ctx.IsInStrafeRange()))
            SwitchState(_factory.Backing());
        else if (_ctx.profile.canStrafe && (_ctx.IsInStrafeRange())) SwitchState(_factory.Strafe());
        
        else if (!_ctx.IsInStrafeRange() && _ctx.IsInChaseRange())
            SwitchState(_factory.Chase());
    }
    
}