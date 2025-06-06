using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Flags")] 
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isIdle;
    [SerializeField] private bool isChasing;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isDead;
    [SerializeField] private bool isReacting;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isShooting; 
    
    [SerializeField] private bool hasHit;
    [SerializeField] public bool tank = false;
    
    public int AttackCount {get; set;}
    public float TimeSinceLastAttack { get; set; }
    public bool PreviousIsAttacking { get; set; }
    
    [Header("CharacterStats")]
    public CharacterStats CharacterStats { get; private set; }
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider easeHealthSlider;
    SkinnedMeshRenderer[] meshRenderer;
    private Coroutine damageFlashCoroutine;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;
    
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsIdle { get => isIdle; set => isIdle = value; }
    public bool IsChasing { get => isChasing; set => isChasing = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsShooting { get => isShooting; set => isShooting = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsReacting { get => isReacting; set => isReacting = value; }
    public bool HasHit { get => hasHit; set => hasHit = value; }
    
    private void Awake()
    {
        CharacterStats = GetComponent<CharacterStats>();
        meshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
    }
    
    public void TakeDamage(int amount, bool reacting = false)
    {
        if(isDead) return;
        
        bool canReact = !isDead && !isJumping && isGrounded;
        
        CharacterStats.CurrentHealth -= amount;
        if (healthSlider != null && easeHealthSlider != null)
        {
            healthSlider.maxValue = CharacterStats.MaximumHealth;
            healthSlider.value = CharacterStats.CurrentHealth;
        }
        if (CharacterStats.CurrentHealth <= 0)
        {
            CharacterStats.CurrentHealth = 0;
            Die();
        }
        else if (reacting && canReact && !isAttacking && !isReacting) isReacting = true;
        
        audioSource.PlayOneShot(damageSound);
        
        if (damageFlashCoroutine != null)
            StopCoroutine(nameof(DamageFlashRoutine));

        damageFlashCoroutine = StartCoroutine(nameof(DamageFlashRoutine));
    }

    public void Die()
    {
        isDead = true;
    }
    
    private IEnumerator DamageFlashRoutine()
    {
        foreach (var meshRenderer in meshRenderer)
        {
            meshRenderer.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var meshRenderer in meshRenderer)
        {
            meshRenderer.material.color = Color.white;

            damageFlashCoroutine = null;
        }
      
    }
    
}
