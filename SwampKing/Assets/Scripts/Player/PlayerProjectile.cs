using UnityEngine;

public class PlayerProjectile : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        EnemyManager stats = other.GetComponent<EnemyManager>();
        if(stats != null)
        {
            if(stats.tank)stats.TakeDamage(damage/4);
            else stats.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
