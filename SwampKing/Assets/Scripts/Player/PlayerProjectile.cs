using UnityEngine;

public class PlayerProjectile : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        EnemyManager stats = other.GetComponent<EnemyManager>();
        if(stats != null)
        {
            stats.TakeDamage(damage);
        }
    }
}
