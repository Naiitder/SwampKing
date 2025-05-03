using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;  
    [SerializeField] protected float lifetime = 2f;
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
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    
}
