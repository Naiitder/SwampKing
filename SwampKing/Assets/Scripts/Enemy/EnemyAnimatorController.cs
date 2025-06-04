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
    private int shootingHash;
    
    private int isReactingHash;
    private int isDeadHash;
    private int reactionFinishedHash;

    [SerializeField] private Collider weaponCollider;
    [SerializeField] private GameObject weaponTrail;
    [SerializeField] private ParticleSystem weaponVFX;
    
    public Animator Animator { get { return animator; } }
    public int HorizontalHash {get { return horizontalHash; }}
    public int VerticalHash {get { return verticalHash; }}
    public int SimpleAttack1Hash {get { return simpleAttack1Hash; }}
    public int SimpleAttack2Hash {get { return simpleAttack2Hash; }}
    public int SimpleAttack3Hash {get { return simpleAttack3Hash; }}
    public int AttackFinishedHash {get { return attackFinishedHash; }}
    public int IsPreparingAttackHash {get { return isPreparingAttackHash; }}
    public int ShootingHash {get { return shootingHash; }}
    public int IsReactingHash {get { return isReactingHash; }}
    public int IsDeadHash {get { return isDeadHash; }}
    public int ReactionFinishedHash {get { return reactionFinishedHash; }}


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
        isReactingHash = Animator.StringToHash("isReacting");
        isDeadHash = Animator.StringToHash("isDead");
        reactionFinishedHash = Animator.StringToHash("reactionFinished");
        shootingHash = Animator.StringToHash("isShooting");
        
        CloseWeaponCollider();
    }
    
    public void OnAttackAnimationFinished()
    {
        Animator.SetBool(attackFinishedHash, true);
        CloseWeaponCollider();
    }
    
    public void OnReactingAnimationFinished()
    {
        Animator.SetBool(reactionFinishedHash, true);
    }

    
    public void CloseWeaponCollider()
    {
        if(weaponCollider != null) weaponCollider.enabled = false;
        if(weaponTrail != null) weaponTrail.SetActive(false);
    }

    public void OpenWeaponCollider()
    {
        if(weaponCollider != null) weaponCollider.enabled = true;
        if(weaponTrail != null) weaponTrail.SetActive(true);
        if(weaponVFX != null) weaponVFX.Play();

    }

}
