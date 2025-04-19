using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Flags")] 
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isIdle;
    [SerializeField] private bool isChasing;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isDead;
    
    public int AttackCount {get; set;}
    
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsIdle { get => isIdle; set => isIdle = value; }
    public bool IsChasing { get => isChasing; set => isChasing = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    
}
