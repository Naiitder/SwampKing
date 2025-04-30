using UnityEngine;

public class EnemyRangedAttackState : EnemyBaseState
{
    private float shootCooldown = 2f;
    private float lastShootTime;

    public EnemyRangedAttackState(EnemyStateMachine currentContext, EnemyStateFactory factory)
        : base(currentContext, factory) {}

    public override void EnterState()
    {
        _ctx.Agent.isStopped = true; 
        _ctx.EnemyManager.IsShooting = true;
        lastShootTime = -shootCooldown; 
    }

    public override void UpdateState()
    {
        if (_ctx.PlayerTarget != null)
        {
            _ctx.transform.LookAt(_ctx.PlayerTarget); 

            if (Time.time >= lastShootTime + shootCooldown)
            {
                ShootProjectile();
                lastShootTime = Time.time;
            }
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.Agent.isStopped = false;
        _ctx.EnemyManager.IsShooting = false;
    }

    public override void InitializeSubState() {}

    public override void CheckSwitchStates()
    {
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting && _ctx.profile.canReact) SwitchState(_factory.Reaction());
        
        if (_ctx.PlayerTarget == null || !_ctx.IsInShootingRange() || !_ctx.profile.attacksFromDistance)
        {
            SwitchState(_factory.Chase()); 
        }
    }

    private void ShootProjectile()
    {
        // Lógica de disparo (ejemplo simple)
        Debug.Log("Enemy shoots!");
        // Puedes instanciar un proyectil aquí
        // GameObject projectile = GameObject.Instantiate(_ctx.projectilePrefab, _ctx.firePoint.position, _ctx.firePoint.rotation);
    }
}