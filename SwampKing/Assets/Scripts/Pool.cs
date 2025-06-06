using System;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().IsDrowned = true;
        }
    }
}
