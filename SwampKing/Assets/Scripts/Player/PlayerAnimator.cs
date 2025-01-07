using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    private int verticalHash;
    private int horizontalHash;
    private int isJumpingHash;
    bool isJumpAnimating;

    public Animator Animator { get { return animator; } }

    public bool IsJumpAnimating { get { return isJumpAnimating; } set { isJumpAnimating = value; } }
    public int IsJumpingHash { get { return isJumpingHash; } }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        verticalHash = Animator.StringToHash("Vertical");
        horizontalHash = Animator.StringToHash("Horizontal");
        isJumpingHash = Animator.StringToHash("isJumping");
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
}
