using UnityEngine;

public class EnemyBackState : EnemyBaseState
{
    public EnemyBackState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    { }

    private float currentAngle = 0f;
    private float backDuration = 1f;
    private float backTimer = 0f;

    public float retreatSpeed = 60f;
    public float retreatSpeedVariation = 20f;
    public float retreatRangeVariation = 0.5f;
    public float modifierUpdateInterval = 1f;

    private float randomSpeedModifier = 0f;
    private float randomRangeModifier = 0f;
    private float lastModifierUpdateTime = 0f;

    public override void EnterState()
    {
        _ctx.Agent.speed = _ctx.movementSpeed;
        backTimer = backDuration;
    }

    public override void UpdateState()
    {
        if (_ctx.PlayerTarget != null)
        {
            RetreatFromPlayer();
        }

        backTimer -= Time.deltaTime;

        if (backTimer <= 0f)
        {
            CheckSwitchStates();
        }
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState() {}

    public override void CheckSwitchStates()
    {
        if (_ctx.EnemyManager.IsDead) SwitchState(_factory.Die());
        else if (_ctx.EnemyManager.IsReacting) SwitchState(_factory.Reaction());
        
        if (_ctx.PlayerTarget == null || !_ctx.IsInChaseRange())
            SwitchState(_factory.Idle());
        else if (_ctx.IsInAttackRange())
            SwitchState(_factory.Attack());
        else if (_ctx.IsInStrafeRange())
            SwitchState(_factory.Strafe());
        else if (_ctx.IsInChaseRange())
            SwitchState(_factory.Chase());
    }

    void RetreatFromPlayer()
    {
        if (Time.time - lastModifierUpdateTime > modifierUpdateInterval)
        {
            randomSpeedModifier = Random.Range(-retreatSpeedVariation, retreatSpeedVariation);
            randomRangeModifier = Random.Range(-retreatRangeVariation, retreatRangeVariation);
            lastModifierUpdateTime = Time.time;
        }

        float effectiveRetreatSpeed = retreatSpeed + randomSpeedModifier;
        currentAngle += effectiveRetreatSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;
        if (currentAngle < 0f) currentAngle += 360f;

        float effectiveRetreatRange = _ctx.StrafeRange + randomRangeModifier;
        
        Vector3 directionToPlayer = (_ctx.PlayerTarget.position - _ctx.transform.position).normalized;
        Vector3 retreatDirection = -directionToPlayer;
        
        Vector3 lateral = Vector3.Cross(Vector3.up, retreatDirection).normalized;
        float lateralOffset = Random.Range(-1f, 1f) * _ctx.StrafeRange * 0.5f;


        Vector3 offset = (retreatDirection * effectiveRetreatRange) + (lateral * lateralOffset);
        Vector3 retreatTarget = _ctx.transform.position + offset;


        if (CanMoveTo(retreatTarget))
        {
            _ctx.Agent.SetDestination(retreatTarget);
        }
        else
        {
            _ctx.Agent.SetDestination(_ctx.transform.position); 
        }

        FaceTarget(_ctx.PlayerTarget.position);
    }

    bool CanMoveTo(Vector3 target)
    {
        Vector3 dir = (target - _ctx.transform.position).normalized;
        float distance = Vector3.Distance(_ctx.transform.position, target);
        Ray ray = new Ray(_ctx.transform.position + Vector3.up * 0.5f, dir);
        return !Physics.Raycast(ray, distance, LayerMask.GetMask("Default", "Environment"));
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - _ctx.transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }
}
