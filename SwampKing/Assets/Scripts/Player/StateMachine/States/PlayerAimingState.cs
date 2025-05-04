using System.Collections;
using UnityEngine;

public class PlayerAimingState  : PlayerBaseState
{

    private bool attackFinished = true;
    private float attackDelay; 
    private Collider[] enemyBuffer = new Collider[20]; 


    public PlayerAimingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if(_ctx.PlayerManager.IsDead) SwitchState(_factory.Dead());
        else if(_ctx.PlayerManager.IsReacting) SwitchState(_factory.Reaction());
        
        if (attackFinished && !InputController.instance.IsAimingPressed) 
        {
            if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
            else if (InputController.instance.MoveAmount == 0) SwitchState(_factory.Idle());
            else if (InputController.instance.MoveAmount > 0) SwitchState(_factory.Walk());
            else if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
        }
    }

    public override void EnterState()
    {
        attackFinished = true;
        
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AimingHash, true);
        
    }


    public override void ExitState()
    {
        _ctx.PlayerManager.IsAttacking = false;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AimingHash, false);

    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleGroundedMovement();
        AimAtNearestEnemy();

        if (attackFinished && InputController.instance.CheckActions(InputController.InputActionType.Attack))
        {
            Shoot();
        }


        CheckSwitchStates();
    }



    private void Shoot()
    {
        attackFinished = false;
        
        //TODO ANIM
        //_ctx.PlayerAnimator.Animator.SetTrigger("Shoot"); 
        
        _ctx.AudioSource.PlayOneShot(_ctx.ShootSound);
        

        Quaternion rotation = _ctx.transform.rotation;
        GameObject projectile = GameObject.Instantiate(_ctx.GunProjectilePrefab, _ctx.ShootPoint.position, rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Damage = _ctx.PlayerManager.CharacterStats.Damage;
        }
        _ctx.StartCoroutine(ResetAttackCooldown(0.2f));
    }

    private IEnumerator ResetAttackCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        attackFinished = true;
    }
    
    private void AimAtNearestEnemy()
    {
        Transform target = GetNearestVisibleEnemy(20f); 

        if (target != null)
        {
            Vector3 direction = (target.position - _ctx.transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, lookRotation, Time.deltaTime * 8f);
        }
    }
    
    private Transform GetNearestVisibleEnemy(float maxDistance)
    {
        int layerMask = LayerMask.GetMask("Enemy"); 
        int count = Physics.OverlapSphereNonAlloc(_ctx.transform.position, maxDistance, enemyBuffer, layerMask);

        Transform nearestEnemy = null;
        float shortestDistance = maxDistance;

        for (int i = 0; i < count; i++)
        {
            Collider col = enemyBuffer[i];
            if (col == null) continue;

            EnemyManager enemy = col.GetComponent<EnemyManager>(); 
            if (enemy == null) continue;

            Vector3 dirToEnemy = (col.transform.position - _ctx.transform.position).normalized;
            float distance = Vector3.Distance(_ctx.transform.position, col.transform.position);
            
            if (Physics.Raycast(_ctx.transform.position + Vector3.up * 1.5f, dirToEnemy, out RaycastHit hit, distance, ~LayerMask.GetMask("IgnoreRaycast", "Enemy")))
            {
                if (hit.transform != col.transform) continue;
            }
            
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = col.transform;
            }
        }

        return nearestEnemy;
    }


}
