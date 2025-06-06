using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamageCollider : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] public int damage;
    
    private void OnTriggerEnter(Collider other)
    {
        if (enemyManager != null)
        {
            PlayerManager stats = other.GetComponent<PlayerManager>();
            if(stats != null && !enemyManager.HasHit)
            {
                enemyManager.HasHit = true;
                stats.TakeDamage(enemyManager.CharacterStats.Damage, true);
            }
        }
        else
        {
            PlayerManager stats = other.GetComponent<PlayerManager>();
            if(stats != null)
            {
                stats.TakeDamage(damage, true);
            }
        }
    }
}
