using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;  
    [SerializeField] protected float lifetime = 2f;
    float rotationSpeed = 15f;
    protected Transform target;

    protected int damage = 0;
    public int Damage { get { return damage; } set { damage = value; } }
    public Transform Target { get { return target; } set { target = value; } }
    
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + Vector3.up * 1.5f;

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction); 
            }
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }
    
}
