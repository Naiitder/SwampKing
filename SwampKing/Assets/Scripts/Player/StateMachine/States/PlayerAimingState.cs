using System.Collections;
using UnityEngine;

public class PlayerAimingState  : PlayerBaseState
{

    private bool attackFinished = true;
    private float attackDelay = 0.4f;
    private float distance = 40f;
    private Collider[] enemyBuffer = new Collider[20]; 
    private Transform playerTransform;


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

    public override void FixedUpdateState()
    {
        
    }

    public override void EnterState()
    {
        attackFinished = true;
        _ctx.PlayerManager.IsAiming = true;

        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AimingHash, true);
        
    }


    public override void ExitState()
    {
        _ctx.PlayerManager.IsAiming = false;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AimingHash, false);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.ShotHash,false); 


    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleGroundedMovement();
        playerTransform = GetNearestVisibleEnemy(distance);
        AimAtNearestEnemy();

        if (attackFinished && InputController.instance.CheckActions(InputController.InputActionType.Attack))
        {
            Shoot();
            InputController.instance.InputBuffer.Dequeue();
        }


        CheckSwitchStates();
    }



    private void Shoot()
    {
        attackFinished = false;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.ShotHash,true); 
        
        _ctx.AudioSource.PlayOneShot(_ctx.ShootSound);
        

        Quaternion rotation = _ctx.transform.rotation;
        GameObject projectile = GameObject.Instantiate(_ctx.GunProjectilePrefab, _ctx.ShootPoint.position, rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Damage = _ctx.PlayerManager.CharacterStats.Damage;
            if(playerTransform != null) projectileScript.Target = playerTransform;
        }
        _ctx.StartCoroutine(ResetAttackCooldown(attackDelay));
    }

    private IEnumerator ResetAttackCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.ShotHash,false); 
        attackFinished = true;
    }
    
    private void AimAtNearestEnemy()
    {

        if (playerTransform != null)
        {
            _ctx.aimCamera.LookAt = playerTransform;
            
            Vector3 inputDir = new Vector3(InputController.instance.MovementInput.x, 0f, InputController.instance.MovementInput.y);

            Vector3 toEnemy = (playerTransform.position - _ctx.transform.position).normalized;
            toEnemy.y = 0f;
            
            Vector3 right = Vector3.Cross(Vector3.up, toEnemy).normalized;
            
            Vector3 moveWorldDir = (right * inputDir.x + toEnemy * inputDir.z).normalized;
            
            Vector3 localInputDir = _ctx.transform.InverseTransformDirection(moveWorldDir);

            float vertical = Mathf.Clamp(localInputDir.z, -1f, 1f);
            float horizontal = Mathf.Clamp(localInputDir.x, -1f, 1f);

            _ctx.PlayerAnimator.UpdateMovementAnimationValues(vertical, horizontal);
        }
        else
        {
            _ctx.PlayerAnimator.UpdateMovementAnimationValues(InputController.instance.MoveAmount, 0);
            _ctx.PlayerMovement.HandleRotation();
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
