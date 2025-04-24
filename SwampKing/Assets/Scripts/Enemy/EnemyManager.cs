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
    
    public int AttackCount {get; set;}
    public float TimeSinceLastAttack { get; set; }
    public bool PreviousIsAttacking { get; set; }
    
    [Header("CharacterStats")]
    public CharacterStats CharacterStats { get; private set; }
    [SerializeField] private Slider slider;

    
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsIdle { get => isIdle; set => isIdle = value; }
    public bool IsChasing { get => isChasing; set => isChasing = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsReacting { get => isReacting; set => isReacting = value; }
    
    private void Awake()
    {
        CharacterStats = GetComponent<CharacterStats>();
    }
    
    public void TakeDamage(int amount, bool reacting = false)
    {
        bool canReact = !isDead && !isJumping && isGrounded;
        
        CharacterStats.CurrentHealth -= amount;
        if (slider != null)
        {
            slider.maxValue = CharacterStats.MaximumHealth;
            slider.value = CharacterStats.CurrentHealth;
        }
        if (CharacterStats.CurrentHealth <= 0)
        {
            CharacterStats.CurrentHealth = 0;
            Die();
        }
        else if (reacting && canReact) isReacting = true;
    }

    public void Die()
    {
        isDead = true;
    }
    
}
