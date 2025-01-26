using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    PlayerManager playerManager;
    PlayerAnimator playerAnimator;

    [SerializeField] LayerMask groundLayer;
    
    Transform cameraObject;

    [Header("CharacterMovementStats")]
    [SerializeField] float walkingSpeed = 2.5f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float sprintSpeed = 7f;
    Vector3 moveDirection;
    Vector3 appliedMovement;
    Transform myTransform;
    [SerializeField] float rotationSpeed = 10;

    [Header("JumpStats")]
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float groundCheckSphereRadius = .25f;
    [SerializeField] float initialJumpVelocity;
    [SerializeField] float maxJumpHeight = 4.0f;
    [SerializeField] float maxJumpTime = 0.75f;

    public CharacterController CharacterController { get { return characterController; } }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerManager = GetComponent<PlayerManager>();
        cameraObject = Camera.main.transform;
        myTransform = transform;

        SetupJumpVariables();
    }


    public void HandleJump()
    {
        if (characterController.isGrounded)
        {
            canDoubleJump = true;
            playerManager.IsJumping = false;
        }
        if (!characterController.isGrounded && InputController.instance.IsJumpPressed && canDoubleJump)
        {
            canDoubleJump = false;
            PerformJump(initialJumpVelocity * 1.25f);

            playerAnimator.Animator.SetBool(playerAnimator.IsDoubleJumpingHash, true);
        }
        else if (InputController.instance.IsJumpPressed && characterController.isGrounded)
        {
            jumpChargeTime += Time.deltaTime;
            if (jumpChargeTime >= tapThreshold)
            {
                playerAnimator.Animator.SetBool(playerAnimator.IsChargingJumpHash, true);
                playerManager.IsChargingJumping = true;
            }
        }
        else
        {
            if (jumpChargeTime <= tapThreshold && jumpChargeTime > 0 && !playerManager.IsJumping && characterController.isGrounded)
            {
                PerformJump(initialJumpVelocity);

                playerAnimator.Animator.SetBool(playerAnimator.IsJumpingHash, true);
            }
            else if (jumpChargeTime >= tapThreshold && !playerManager.IsJumping && characterController.isGrounded)
            {
                PerformJump(initialJumpVelocity * 1.5f);

                playerAnimator.Animator.SetBool(playerAnimator.IsJumpingHash, true);
            }
        }
       


    }

    private void PerformJump(float jumpVelocity)
    {
        moveDirection.y = jumpVelocity;
        appliedMovement.y = jumpVelocity;
        jumpChargeTime = 0;
        playerAnimator.Animator.SetBool(playerAnimator.IsChargingJumpHash, false);
        playerManager.IsJumping = true;
        InputController.instance.IsJumpPressed = false;
        playerManager.IsChargingJumping = false;
    }


    public void StopMovement()
    {
        moveDirection = Vector3.zero;
    }

    public void HandleMovement()
    {
        appliedMovement.x = moveDirection.x;
        appliedMovement.z = moveDirection.z;
        characterController.Move(appliedMovement*Time.deltaTime);
    }

    public void HandleGroundedMovement()
    {
        Vector3 moveDirectionAux;
        moveDirectionAux = cameraObject.transform.forward * InputController.instance.VerticalInput;
        moveDirectionAux = moveDirectionAux + cameraObject.transform.right * InputController.instance.HorizontalInput;
        moveDirectionAux.Normalize();
        moveDirection.x = moveDirectionAux.x;
        moveDirection.z = moveDirectionAux.z;

        if (InputController.instance.MoveAmount > 0.5f)
        {
            moveDirection.x = moveDirection.x * movementSpeed;
            moveDirection.z = moveDirection.z * movementSpeed;
        }
        else if (InputController.instance.MoveAmount <= 0.5f)
        {
            moveDirection.x = moveDirection.x * walkingSpeed;
            moveDirection.z = moveDirection.z * walkingSpeed;
        }

        playerAnimator.UpdateMovementAnimationValues(InputController.instance.MoveAmount, 0);

    }

    public void HandleGravity()
    {
        bool isFalling = moveDirection.y <= 0.0f || !InputController.instance.IsJumpPressed;
        float fallMultiplier = 2.0f;
        if (isFalling)
        {
            float previousYVelocity = moveDirection.y;
            moveDirection.y = moveDirection.y + (gravity * fallMultiplier*Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + moveDirection.y) * .5f, -20.0f);
        }
        else
        {
            float previousYVelocity = moveDirection.y;
            moveDirection.y = moveDirection.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + moveDirection.y) * .5f;
        }
    }

    public void SetGravity()
    {
        moveDirection.y = gravity;
        appliedMovement.y = gravity;
    }

    public void CancelJumpAnimation()
    {
        playerAnimator.Animator.SetBool(playerAnimator.IsJumpingHash, false);
        playerAnimator.Animator.SetBool(playerAnimator.IsDoubleJumpingHash, false);
        if (InputController.instance.IsJumpPressed) InputController.instance.RequireNewJumpPress = true;
    }

    public void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;
        float moveOverride = InputController.instance.MoveAmount;

        targetDir = cameraObject.transform.forward * InputController.instance.VerticalInput;
        targetDir += cameraObject.transform.right * InputController.instance.HorizontalInput;

        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero) targetDir = myTransform.forward;

        float rs = rotationSpeed;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * Time.deltaTime);

        myTransform.rotation = targetRotation;
    }

    private void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    public void FallingAnimationOn()
    {
        playerAnimator.Animator.SetBool("isFalling", true);
    }

    public void FallingAnimationOff()
    {
        playerAnimator.Animator.SetBool("isFalling", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
    }
}
