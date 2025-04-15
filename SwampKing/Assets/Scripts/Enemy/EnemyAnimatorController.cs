using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    Animator animator;

    private int isIdleHash; 
    private int isChasingHash; 
    
    public Animator Animator { get { return animator; } }
    public int IsIdleHash {get { return isIdleHash; }}
    public int IsChasingHash {get { return isChasingHash; }}


    private void Awake()
    {
        animator = GetComponent<Animator>();
        isIdleHash = Animator.StringToHash("isIdle");
        isChasingHash = Animator.StringToHash("isChasing");
    }
}
