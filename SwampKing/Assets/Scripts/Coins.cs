using System;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public Vector3 initialPosition;
    public float offset;
    private bool isAttracted = false;
    public int coins;
    public Transform player; 
    public float attractionDistance = 3f;
    private float currentAttractionSpeed = 0f;
    private float attractionTimer = 0f;
    public float maxAttractionSpeed = 10f;
    public float accelerationDuration = 0.5f; 
    

    private void Start()
    {
        initialPosition = transform.position;
        offset = UnityEngine.Random.Range(0f, 360f);
        CoinManager.Instance.RegisterCoin(this);
    }

    public void UpdatePosition(float sinValue)
    {
        if (isAttracted) return;

        Vector3 newPos = initialPosition;
        newPos.y += sinValue;
        transform.position = newPos;
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }

    private void OnDestroy()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.UnregisterCoin(this);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.instance.UpdateCoins(coins);
            Destroy(gameObject);
        }
    }
    
    public void UpdateAttraction()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < attractionDistance || isAttracted)
        {
            isAttracted = true;
            
            attractionTimer += Time.deltaTime;
            
            float t = Mathf.Clamp01(attractionTimer / accelerationDuration);
            currentAttractionSpeed = Mathf.Lerp(0f, maxAttractionSpeed, t);
            
            transform.position = Vector3.MoveTowards(transform.position,
                player.position + Vector3.up, currentAttractionSpeed * Time.deltaTime);
        }
    }
}
