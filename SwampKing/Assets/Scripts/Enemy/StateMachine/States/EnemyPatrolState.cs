using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState
{
    public EnemyPatrolState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        _ctx.Agent.speed = _ctx.movementSpeed;
        _ctx.EnemyManager.IsChasing = true;

    } 
    public override void UpdateState(){
        
        HandlePatrol();
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.EnemyManager.IsChasing = false;

    }
    
    public override void InitializeSubState(){
        
    }
    public override void CheckSwitchStates(){
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting && _ctx.profile.canReact) SwitchState(_factory.Reaction());
        
        if(_ctx.PlayerTarget != null && _ctx.IsInChaseRange()) SwitchState(_factory.Chase());
      //  if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange()) SwitchState(_factory.Idle());
        else if(_ctx.profile.attacksFromDistance && _ctx.IsInShootingRange())
            SwitchState(_factory.RangedAttack());
        else if (_ctx.IsInStrafeRange() && _ctx.profile.canStrafe) SwitchState(_factory.Strafe());
        else if (_ctx.IsInAttackRange() && _ctx.profile.canMeleeAttack) SwitchState(_factory.Attack());
        
    }
    
    private void HandlePatrol(){
        if (!_ctx.hasPatrolTarget || ReachedPatrolPoint()) {
            _ctx.currentPatrolTarget = GetRandomPatrolPoint(_ctx.SpawnPoint, _ctx.PatrolRadius);
            _ctx.hasPatrolTarget = true;
            _ctx.Agent.SetDestination(_ctx.currentPatrolTarget);
        }
    }
    
    private bool ReachedPatrolPoint() {
        return !_ctx.Agent.pathPending && _ctx.Agent.remainingDistance <= _ctx.PatrolPointTolerance;
    }

    private Vector3 GetRandomPatrolPoint(Vector3 origin, float radius) {
        for (int i = 0; i < 30; i++) { 
            Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
            randomPoint.y = origin.y; 
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas)) {
                return hit.position;
            }
        }
        return origin; 
    }


}
