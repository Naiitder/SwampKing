using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    Animator animator;

    private int horizontalHash; 
    private int verticalHash; 
    
    private int simpleAttack1Hash; 
    private int simpleAttack2Hash; 
    private int simpleAttack3Hash; 
    private int attackFinishedHash; 
    private int isPreparingAttackHash;

    [SerializeField] private Collider weaponCollider;
    
    public Animator Animator { get { return animator; } }
    public int HorizontalHash {get { return horizontalHash; }}
    public int VerticalHash {get { return verticalHash; }}
    public int SimpleAttack1Hash {get { return simpleAttack1Hash; }}
    public int SimpleAttack2Hash {get { return simpleAttack2Hash; }}
    public int SimpleAttack3Hash {get { return simpleAttack3Hash; }}
    public int AttackFinishedHash {get { return attackFinishedHash; }}
    public int IsPreparingAttackHash {get { return isPreparingAttackHash; }}


    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
        simpleAttack1Hash = Animator.StringToHash("simpleAttack1");
        simpleAttack2Hash = Animator.StringToHash("simpleAttack2");
        simpleAttack3Hash = Animator.StringToHash("simpleAttack3");
        attackFinishedHash = Animator.StringToHash("attackFinished");
        isPreparingAttackHash = Animator.StringToHash("isPreparingAttack");
    }
    
    public void OnAttackAnimationFinished()
    {
        Animator.SetBool(attackFinishedHash, true);
        CloseWeaponCollider();
    }
    
    public void CloseWeaponCollider()
    {
        if(weaponCollider != null) weaponCollider.enabled = false;
    }

    public void OpenWeaponCollider()
    {
        if(weaponCollider != null) weaponCollider.enabled = true;
    }

}
