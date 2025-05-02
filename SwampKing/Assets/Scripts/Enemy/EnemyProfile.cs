using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyProfile", menuName = "Enemy/EnemyProfile")]
public class EnemyProfile : ScriptableObject
{
    public string enemyName;

    public bool isAggressive;
    public bool canMeleeAttack = true;
    public bool attacksFromDistance;
    public bool canChainAttacks;
    public bool canStrafe;
    public bool canRetreat;
    public bool canReact;
    
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float strafeRange = 4f;
    public float shootingRange = 8f;

    public float movementSpeed = 2f;
    public float runningSpeed = 4f;
    public float rotationSpeed = 15f;
    
    public float chanceToChainAttack = 0.4f;
}
