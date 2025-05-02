using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRangedAttackState : EnemyBaseState
{
    
    private float attackInterval = 2f;
    private Coroutine attackCoroutine;

    public EnemyRangedAttackState(EnemyStateMachine currentContext, EnemyStateFactory factory)
        : base(currentContext, factory) {}

    public override void EnterState()
    {
        _ctx.Agent.isStopped = true; 
        _ctx.EnemyManager.IsShooting = true;
        
        
        attackCoroutine = _ctx.StartCoroutine(AttackLoop());
    }

    public override void UpdateState()
    {

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.Agent.isStopped = false;
        _ctx.EnemyManager.IsShooting = false;
        
        if (attackCoroutine != null)
        {
            _ctx.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    public override void InitializeSubState() {}

    public override void CheckSwitchStates()
    {
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting && _ctx.profile.canReact) SwitchState(_factory.Reaction());
        
        if(_ctx.PlayerTarget == null || (!_ctx.IsInShootingRange() && !_ctx.IsInChaseRange())) SwitchState(_factory.Idle());
        else if (!_ctx.IsInShootingRange())
        {
            if (_ctx.IsInChaseRange()) 
                SwitchState(_factory.Chase()); 
        }
        else if (_ctx.IsInAttackRange() && _ctx.profile.canMeleeAttack) SwitchState(_factory.Attack());
        
        //TODO condicion para usar strafe y back state
    }

    private bool AnimationFinished(int animationHash)
    {
        AnimatorStateInfo stateInfo = _ctx.EnemyAnimatorController.Animator.GetCurrentAnimatorStateInfo(1);
        return stateInfo.shortNameHash == animationHash && stateInfo.normalizedTime >= 1f;
    }
    
    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return AttackRoutine();
        }
    }
    
    private IEnumerator AttackRoutine()
    {
        if (_ctx.PlayerTarget != null)
        {
            _ctx.transform.LookAt(_ctx.PlayerTarget); 
            
            yield return new WaitForSeconds(attackInterval);

            _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.ShootingHash, true); 
            _ctx.transform.LookAt(_ctx.PlayerTarget); 

            yield return new WaitUntil(() => AnimationFinished(_ctx.EnemyAnimatorController.ShootingHash));

            _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.ShootingHash, false);
          
            Quaternion rotation = _ctx.transform.rotation;
            GameObject projectile = Object.Instantiate(_ctx.projectilePrefab, _ctx.projectileSpawnPoint.position, rotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Damage = _ctx.EnemyManager.CharacterStats.Damage;
            }
            
            _ctx.AudioSource.PlayOneShot(_ctx.shootSound);
        }
           
    }
}