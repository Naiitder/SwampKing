using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    Animator animator;
    
    public Animator Animator { get { return animator; } }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
