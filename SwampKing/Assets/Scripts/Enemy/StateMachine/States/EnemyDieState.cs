using UnityEngine;

public class EnemyDieState : EnemyBaseState
{
    public EnemyDieState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        
        _ctx.Agent.stoppingDistance = 0f;
        _ctx.Agent.SetDestination(_ctx.transform.position);
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsDeadHash, true);
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    { }
    
    public override void InitializeSubState(){
        
    }

    public override void CheckSwitchStates()
    {}
}