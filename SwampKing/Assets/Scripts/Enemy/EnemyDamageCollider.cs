using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamageCollider : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager stats = other.GetComponent<PlayerManager>();
        if(stats != null && !enemyManager.HasHit)
        {
            enemyManager.HasHit = true;
            stats.TakeDamage(enemyManager.CharacterStats.Damage, true);
        }
    }
}
