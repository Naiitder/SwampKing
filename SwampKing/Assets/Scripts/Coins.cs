using System;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public int coins;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.instance.UpdateCoins(coins);
            Destroy(gameObject);
        }
    }
}
