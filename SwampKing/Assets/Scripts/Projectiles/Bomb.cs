using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float flightDuration = 1.5f;
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] protected Collider collider;
    [SerializeField] private AudioClip bombSound;
    
    private AudioSource audioSource;

    protected Transform target;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float timer;
    private bool hasLanded = false;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (collider != null)
            collider.enabled = false;

        if (explosionEffect != null)
            explosionEffect.Stop();
        
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        if (hasLanded) return;

        if (target != null)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / flightDuration);
            
            Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, progress);
            float height = 4 * maxHeight * progress * (1 - progress); 
            currentPos.y += height;

            transform.position = currentPos;

            if (progress >= 1f)
            {
                OnImpact();
            }
        }
    }
        
    private void OnImpact()
    {
        hasLanded = true;

        if (explosionEffect != null)
            explosionEffect.Play();

        if (collider != null)
            collider.enabled = true;

        Destroy(gameObject, .5f);
        audioSource.PlayOneShot(bombSound);
    }
    
    public void Initialize(Transform targetTransform, int damage)
    {
        target = targetTransform;
        startPoint = transform.position;

        Vector3 predictedOffset = Vector3.zero;
        
        if (targetTransform.TryGetComponent(out CharacterController cc))
        {
            Vector3 velocity = cc.velocity;
            Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

            if (flatVelocity.magnitude > 0.2f) 
            {
                predictedOffset = flatVelocity * flightDuration;
            }
        }

        endPoint = targetTransform.position + predictedOffset + Vector3.up * 1.5f;

        EnemyDamageCollider edc = collider.GetComponent<EnemyDamageCollider>();
        if (edc != null)
            edc.damage = damage * 2;
    }


}