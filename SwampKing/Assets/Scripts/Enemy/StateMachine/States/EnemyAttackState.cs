using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private bool attackFinished;
    
    public EnemyAttackState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    { }
    
    public override void EnterState()
    {
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.SimpleAttack1Hash,true);
        _ctx.EnemyManager.IsAttacking = true;
        _ctx.Agent.SetDestination(_ctx.transform.position);
    } 

    public override void UpdateState()
    {
        if (_ctx.EnemyAnimatorController.Animator.GetBool(_ctx.EnemyAnimatorController.AttackFinishedHash))
        {
            attackFinished = true;
        }
        
        /* Todo hacer una manera de que el enemigo pueda hacer una cadena de ataques
         if ()
            _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsPreparingAttackHash,true);
        else _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsPreparingAttackHash,false);
        */
        
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.SimpleAttack1Hash,true);
        _ctx.EnemyManager.IsAttacking = false;
    }
    
    public override void InitializeSubState() {}

    public override void CheckSwitchStates()
    {
        if (attackFinished)
        {
            if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange())
                SwitchState(_factory.Idle());
            else if(_ctx.IsInStrafeRange())
                SwitchState(_factory.Strafe());
            else if (!_ctx.IsInStrafeRange() && _ctx.IsInChaseRange())
                SwitchState(_factory.Chase());
            /* Seguir el ataque 
            else if ()
                SwitchState(_factory.Attack());
             */
        }
    }
    
}