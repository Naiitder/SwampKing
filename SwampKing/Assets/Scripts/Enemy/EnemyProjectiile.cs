using System;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerManager>().TakeDamage(damage);
        }
    }
}
