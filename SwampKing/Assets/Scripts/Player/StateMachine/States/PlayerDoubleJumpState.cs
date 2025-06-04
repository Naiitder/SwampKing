

using System.Collections;
using UnityEngine;

public class PlayerDoubleJumpState : PlayerBaseState
{
        private bool attackFinished = true;
    private float attackDelay = 0.4f;
    private float distance = 40f;
    private Collider[] enemyBuffer = new Collider[20]; 
    private Transform enemyTransform;
    
    public PlayerDoubleJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    { }

    public override void CheckSwitchStates()
    {
        if (_ctx.PlayerMovement.isGrounded()) 
            SwitchState(_factory.Grounded());
        
        if (!InputController.instance.IsAimingPressed && InputController.instance.CheckActions(InputController.InputActionType.Attack)) 
            SwitchState(_factory.JumpAttack());

    }

    public override void EnterState()
    {
        attackFinished = true;
        
        _ctx.PlayerMovement.PerformJump(1.25f);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsDoubleJumpingHash, true);
        _ctx.PlayerManager.JumpChargeTime = 0;
        _ctx.PlayerManager.IsJumping = true;
        _ctx.PlayerManager.CanDoubleJump = false;
        InputController.instance.InputBuffer.Dequeue();
        
        _ctx.AudioSource.pitch = .9f;
        _ctx.AudioSource.volume = .5f;
        _ctx.AudioSource.PlayOneShot(_ctx.JumpSound);
        
        _ctx.JumpTrailLF.SetActive(true);
        _ctx.JumpTrailRF.SetActive(true);

    }
    
    public override void FixedUpdateState()
    {
        
    }

    public override void ExitState()
    {
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsDoubleJumpingHash, false);
        _ctx.PlayerManager.IsJumping = false;
        ResetAiming();
        _ctx.JumpTrailLF.SetActive(false);
        _ctx.JumpTrailRF.SetActive(false);

        
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleGroundedMovement();
        
        if (InputController.instance.IsAimingPressed)
        {
            if(!_ctx.PlayerManager.IsAiming) EnableAiming();
            
            enemyTransform = GetNearestVisibleEnemy(distance);
            AimAtNearestEnemy();

            if (attackFinished && InputController.instance.CheckActions(InputController.InputActionType.Attack))
            {
                Shoot();
                InputController.instance.InputBuffer.Dequeue();
            }
        }else if (attackFinished && !InputController.instance.IsAimingPressed)
        {
            ResetAiming();
            _ctx.PlayerMovement.HandleRotation();
        }
        CheckSwitchStates();
    }
    
        private void EnableAiming()
    {
        attackFinished = true;
        _ctx.PlayerManager.IsAiming = true;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AimingHash, true);
    }

    private void ResetAiming()
    {
        if (!InputController.instance.IsAimingPressed)
        {
            _ctx.PlayerManager.IsAiming = false;
            _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.AimingHash, false);
            _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.ShotHash,false); 
            _ctx.cameraObject.SetActive(false);
        }
    }
      private void Shoot()
    {
        attackFinished = false;
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.ShotHash,true); 
        
        _ctx.AudioSource.PlayOneShot(_ctx.ShootSound);
        _ctx.AudioSource.pitch = 1.5f;
        _ctx.AudioSource.volume = 0.6f;
        

        Quaternion rotation = _ctx.transform.rotation;
        GameObject projectile = GameObject.Instantiate(_ctx.GunProjectilePrefab, _ctx.ShootPoint.position, rotation);
        _ctx.GunShootParticles.Play();
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Damage = _ctx.PlayerManager.CharacterStats.Damage;
            if(enemyTransform != null) projectileScript.Target = enemyTransform;
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

        if (enemyTransform != null)
        { 
            _ctx.cameraObject.SetActive(true);
            _ctx.aimCamera.Target.LookAtTarget = enemyTransform;
            _ctx.transform.LookAt(enemyTransform);
            Vector3 inputDir = new Vector3(InputController.instance.MovementInput.x, 0f, InputController.instance.MovementInput.y);

            Vector3 toEnemy = (enemyTransform.position - _ctx.transform.position).normalized;
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
            _ctx.cameraObject.SetActive(false);
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
            
            if (Physics.Raycast(_ctx.transform.position + Vector3.up * 1.5f, dirToEnemy, out RaycastHit hit, distance, 
                    ~LayerMask.GetMask("Player", "Enemy")))
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
