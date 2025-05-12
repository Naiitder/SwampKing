using UnityEngine;

public class PlayerDamageCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EnemyManager stats = other.GetComponent<EnemyManager>();
        if(stats != null)
        {
            stats.TakeDamage(25, true);
            Debug.Log(stats);
        }
    }
}
