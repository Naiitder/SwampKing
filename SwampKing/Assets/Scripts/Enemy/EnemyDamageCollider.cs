using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamageCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager stats = other.GetComponent<PlayerManager>();
        if(stats != null) stats.TakeDamage(25, true);
    }
}
