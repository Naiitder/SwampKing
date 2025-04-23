using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("PlayerFlags")]
    [SerializeField] bool isJumping;
    [SerializeField] bool isChargingJumping;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isAttacking; 
    [SerializeField] bool isDead; 
    [SerializeField] bool isReacting; 

    [SerializeField] private bool canDoubleJump;

    [SerializeField] private int attackCount = 0;

    public float TimeSinceLastAttack { get; set; }
    public bool PreviousIsAttacking { get; set; }

    [SerializeField] float maxChargeTime = 1.0f;
    [SerializeField] float tapThreshold = 0.2f;
    [SerializeField] private float jumpChargeTime = 0f;

    [SerializeField] private float inAirTimer = 0f;
    
    public CharacterStats CharacterStats { get; private set; }

    public float JumpChargeTime { get { return jumpChargeTime; } set { jumpChargeTime = value; } }
    public float TapTreshold { get { return tapThreshold; } set { tapThreshold = value; } }
    public float MaxChargeTime { get { return maxChargeTime; } set { maxChargeTime = value; } }
    public float InAirTimer { get { return inAirTimer; } set { inAirTimer = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }
    public bool IsDead { get { return isDead; } set { isDead = value; } }
    public bool IsReacting { get { return isReacting; } set { isReacting = value; } }
    public bool CanDoubleJump { get { return canDoubleJump; } set { canDoubleJump = value; } }
    public bool IsChargingJumping { get { return isChargingJumping; } set { isChargingJumping = value; } }
    public int AttackCount { get { return attackCount; } set { attackCount = value; } }

    private void Awake()
    {
        CharacterStats = GetComponent<CharacterStats>();
    }
    
    public void TakeDamage(int amount, bool reacting = false)
    {
        CharacterStats.CurrentHealth -= amount;
        if (CharacterStats.CurrentHealth <= 0)
        {
            CharacterStats.CurrentHealth = 0;
            Die();
        }
        else if (reacting) isReacting = true;
    }

    public void Die()
    {
        isDead = true;
    }
}
