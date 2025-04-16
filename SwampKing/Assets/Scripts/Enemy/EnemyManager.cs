using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Flags")] 
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isDead;
    
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    
}
