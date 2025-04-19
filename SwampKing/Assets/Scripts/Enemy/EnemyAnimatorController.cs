using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    Animator animator;

    private int horizontalHash; 
    private int verticalHash; 
    
    private int simpleAttack1Hash; 
    private int attackFinishedHash; 
    
    public Animator Animator { get { return animator; } }
    public int HorizontalHash {get { return horizontalHash; }}
    public int VerticalHash {get { return verticalHash; }}
    public int SimpleAttack1Hash {get { return simpleAttack1Hash; }}
    public int AttackFinishedHash {get { return attackFinishedHash; }}


    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
        simpleAttack1Hash = Animator.StringToHash("simpleAttack1");
        attackFinishedHash = Animator.StringToHash("attackFinished");
    }
}
