using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    private List<Coins> coins = new();
    private static float[] sinTable = new float[360];
    private float angle;
    
    public Transform player; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        for (int i = 0; i < 360; i++)
            sinTable[i] = Mathf.Sin(i * Mathf.Deg2Rad) * 0.1f; 
    }
    
    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        angle += Time.deltaTime * 60f;
        if (angle >= 360f) angle -= 360f;

        foreach (var coin in coins)
        {
            int idx = (int)((angle + coin.offset) % 360);
            coin.UpdatePosition(sinTable[idx]);
            coin.player = player;
            coin.UpdateAttraction();
        }
    }

    public void RegisterCoin(Coins coin)
    {
        if (!coins.Contains(coin)) coins.Add(coin);
    }

    public void UnregisterCoin(Coins coin)
    {
        coins.Remove(coin);
    }
}