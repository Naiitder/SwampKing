using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Transform currentPlatform;
    public Vector3 lastPlatformPosition;
    
    [Header("PlayerFlags")]
    [SerializeField] bool isJumping;
    [SerializeField] bool isChargingJumping;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isAttacking; 
    [SerializeField] bool isDead;
    [SerializeField] bool isDrowned;
    [SerializeField] bool isReacting; 
    [SerializeField] bool isAiming;
    [SerializeField] private bool canDoubleJump;

    [SerializeField] private int attackCount = 0;

    public float TimeSinceLastAttack { get; set; }
    public bool PreviousIsAttacking { get; set; }

    [SerializeField] float maxChargeTime = 1.0f;
    [SerializeField] float tapThreshold = 0.2f;
    [SerializeField] private float jumpChargeTime = 0f;

    [SerializeField] private float inAirTimer = 0f;
    [SerializeField] private float coyoteTime = 0.2f;
    
    [Header("CharacterStats")]
    public CharacterStats CharacterStats { get; private set; }
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider easeHealthSlider;
    SkinnedMeshRenderer meshRenderer;
    private Coroutine damageFlashCoroutine;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;
    
    public float JumpChargeTime { get { return jumpChargeTime; } set { jumpChargeTime = value; } }
    public float TapTreshold { get { return tapThreshold; } set { tapThreshold = value; } }
    public float MaxChargeTime { get { return maxChargeTime; } set { maxChargeTime = value; } }
    public float InAirTimer { get { return inAirTimer; } set { inAirTimer = value; } }
    public float CoyoteTime { get { return coyoteTime; } set { coyoteTime = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }
    public bool IsDead { get { return isDead; } set { isDead = value; } }
    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
    public bool IsReacting { get { return isReacting; } set { isReacting = value; } }
    public bool IsAiming { get { return isAiming; } set { isAiming = value; } }
    public bool CanDoubleJump { get { return canDoubleJump; } set { canDoubleJump = value; } }
    public bool IsChargingJumping { get { return isChargingJumping; } set { isChargingJumping = value; } }
    public int AttackCount { get { return attackCount; } set { attackCount = value; } }
    
    public bool IsDrowned { get { return isDrowned; } set { isDrowned = value; } }

    private void Awake()
    {
        CharacterStats = GetComponent<CharacterStats>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void Initilize()
    {
        healthSlider.maxValue = CharacterStats.MaximumHealth;
        healthSlider.value = CharacterStats.CurrentHealth;
        
        easeHealthSlider.maxValue = CharacterStats.MaximumHealth;
        easeHealthSlider.value = CharacterStats.CurrentHealth;
    }

    public void TakeDamage(int amount, bool reacting = false)
    {
        if (isJumping || isDead) return;
        
        bool canReact = !isDead && !isJumping && isGrounded;
        
        CharacterStats.CurrentHealth -= amount;
        if (healthSlider != null)
        {
            healthSlider.maxValue = CharacterStats.MaximumHealth;
            healthSlider.value = CharacterStats.CurrentHealth;
        }
        if (CharacterStats.CurrentHealth <= 0)
        {
            CharacterStats.CurrentHealth = 0;
            Die();
        }
        else if (reacting && canReact) isReacting = true;
        
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
        meshRenderer.material.color = Color.red;

        yield return new WaitForSeconds(.1f);

        meshRenderer.material.color = Color.white;

        damageFlashCoroutine = null;
    }
}
