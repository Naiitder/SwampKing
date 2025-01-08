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
    float groundedGravity = -.05f;
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
        if (!playerManager.IsJumping && characterController.isGrounded && InputController.instance.IsJumpPressed)
        {
            playerAnimator.Animator.SetBool(playerAnimator.IsJumpingHash,true);
            playerAnimator.IsJumpAnimating = true;
            playerManager.IsJumping = true;
            moveDirection.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;
        }
        else if (InputController.instance.IsJumpPressed && characterController.isGrounded && playerManager.IsJumping) playerManager.IsJumping = false;
    }

    public void HandleMovement(float delta)
    {
        appliedMovement.x = moveDirection.x;
        appliedMovement.z = moveDirection.z;
        characterController.Move(appliedMovement*delta);
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

        playerAnimator.UpdateMovementAnimationValues(InputController.instance.MoveAmount,0);
        
    }

    public void HandleGravity(float delta)
    {
        bool isFalling = moveDirection.y <= 0.0f || !InputController.instance.IsJumpPressed;
        float fallMultiplier = 2.0f;
        if (characterController.isGrounded)
        {
            if (playerAnimator.IsJumpAnimating)
            {
                playerAnimator.Animator.SetBool(playerAnimator.IsJumpingHash, false);
                playerAnimator.IsJumpAnimating = false;
            }
            moveDirection.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = moveDirection.y;
            moveDirection.y = moveDirection.y + (gravity * fallMultiplier*delta);
            appliedMovement.y = Mathf.Max((previousYVelocity + moveDirection.y) * .5f, -20.0f);
        }
        else
        {
            float previousYVelocity = moveDirection.y;
            moveDirection.y = moveDirection.y + (gravity * delta);
            appliedMovement.y = (previousYVelocity + moveDirection.y) * .5f;
        }
    }

    public void HandleRotation(float delta)
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
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

        myTransform.rotation = targetRotation;
    }

    private void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
    }
}
