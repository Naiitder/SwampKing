using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    private int verticalHash;
    private int horizontalHash;
    
    private int isJumpingHash;
    private int isFallingHash;
    private int isDoubleJumpingHash;
    private int isChargingJumpHash;
    
    private int simpleAttackHash1;
    private int simpleAttackHash2;
    private int simpleAttackHash3;

    private int attackFinishedHash;
    private int isPreparingAttackHash;

    private int jumpAttackHash;
    private int aimingHash;
    private int shotHash;
    
    private int isDeadHash;
    private int isReactingHash;
    
    private int reactionFinishedHash;

    [Header ("Sword")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private GameObject weaponTrail;
    [SerializeField] private GameObject sword;
    [SerializeField] private Transform swordHolderSlot;
    [SerializeField] private Transform swordHandSlot;
    

    [Header("Gun")]
    [SerializeField] private GameObject pistol;
    [SerializeField] private Transform pistolHolderSlot;
    [SerializeField] private Transform pistolHandSlot;

    public Animator Animator { get { return animator; } }

    public int IsJumpingHash { get { return isJumpingHash; } }
    public int IsDoubleJumpingHash { get { return isDoubleJumpingHash; } }
    public int IsChargingJumpHash { get { return isChargingJumpHash; } }
    public int SimpleAttackHash1 { get { return simpleAttackHash1; } }
    public int SimpleAttackHash2 { get { return simpleAttackHash2; } }
    public int SimpleAttackHash3 { get { return simpleAttackHash3; } }
    public int AttackFinishedHash { get { return attackFinishedHash; } }
    public int IsPreparingAttackHash { get { return isPreparingAttackHash; } }
    public int ShotHash { get { return shotHash; } }
    
    public int JumpAttackHash { get { return jumpAttackHash; } }
    public int IsDeadHash { get { return isDeadHash; } }
    public int IsReactingHash { get { return isReactingHash; } }
    public int ReactionFinishedHash { get { return reactionFinishedHash; } }
    public int IsFallingHash { get { return isFallingHash; } }
    public int AimingHash { get { return aimingHash; } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        verticalHash = Animator.StringToHash("Vertical");
        horizontalHash = Animator.StringToHash("Horizontal");
        isJumpingHash = Animator.StringToHash("isJumping");
        isDoubleJumpingHash = Animator.StringToHash("isDoubleJumping");
        isChargingJumpHash = Animator.StringToHash("isChargingJump");
        simpleAttackHash1 = Animator.StringToHash("simpleAttack1");
        simpleAttackHash2 = Animator.StringToHash("simpleAttack2");
        simpleAttackHash3 = Animator.StringToHash("simpleAttack3");
        attackFinishedHash = Animator.StringToHash("attackFinished");
        isPreparingAttackHash = Animator.StringToHash("isPreparingAttack");
        jumpAttackHash = Animator.StringToHash("jumpAttack");
        isDeadHash = Animator.StringToHash("isDead");
        isReactingHash = Animator.StringToHash("isReacting");
        reactionFinishedHash = Animator.StringToHash("reactionFinished");
        isFallingHash = Animator.StringToHash("isFalling");
        aimingHash = Animator.StringToHash("isAiming");
        shotHash = Animator.StringToHash("Shot");
        
        weaponCollider.enabled = false;
        weaponTrail.SetActive(false);
    }

    public void UpdateMovementAnimationValues(float verticalMovement, float horizontalMovement)
    {
        float v = 0;

        if (verticalMovement > 0 && verticalMovement < 0.55f) v = 0.5f;
        else if (verticalMovement > 0.55f) v = 1;
        else if (verticalMovement < 0 && verticalMovement > -0.55f) v = -0.5f;
        else if (verticalMovement < -0.55f) v = -1;
        else v = 0;

        float h = 0;
        if (horizontalMovement > 0 && horizontalMovement < 0.55f) h = 0.5f;
        else if (horizontalMovement > 0.55f) h = 1;
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f) h = -0.5f;
        else if (horizontalMovement < -0.55f) h = -1;
        else h = 0;


        animator.SetFloat(verticalHash, v, 0.1f, Time.deltaTime);
        animator.SetFloat(horizontalHash, h, 0.1f, Time.deltaTime);
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
        weaponCollider.enabled = false;
        weaponTrail.SetActive(false);

    }

    public void OpenWeaponCollider()
    {
        weaponCollider.enabled = true;
        weaponTrail.SetActive(true);
    }

    public void DrawGun()
    {
        pistol.transform.SetParent(pistolHandSlot);
        pistol.transform.localPosition = Vector3.zero;
        pistol.transform.localScale = new Vector3(4,4,4);
        pistol.transform.localRotation = Quaternion.identity;
    }

    public void HideGun()
    {
        pistol.transform.SetParent(pistolHolderSlot);
        pistol.transform.localPosition = Vector3.zero;
        pistol.transform.localScale = new Vector3(3,3,3);
        pistol.transform.localRotation = Quaternion.identity;
    }

    
    public void DrawSword()
    {
        sword.transform.SetParent(swordHandSlot);
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localScale = new Vector3(30,30,30);
        sword.transform.localRotation = Quaternion.identity;
    }

    public void HideSword()
    {
        sword.transform.SetParent(swordHolderSlot);
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localScale = new Vector3(25,25,25);
        sword.transform.localRotation = Quaternion.identity;
    }


}
