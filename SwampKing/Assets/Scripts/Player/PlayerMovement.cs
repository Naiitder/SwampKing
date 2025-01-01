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
    [SerializeField] Vector3 moveDirection;
    Transform myTransform;
    [SerializeField] float rotationSpeed = 10;

    [Header("JumpStats")]
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float groundCheckSphereRadius = .25f;
    float groundedGravity = -.05f;
    [SerializeField] float initialJumpVelocity;
    [SerializeField] float maxJumpHeight = 1.0f;
    [SerializeField] float maxJumpTime = 0.5f;

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
            playerManager.IsJumping = true;
            moveDirection.y = initialJumpVelocity * .5f;
        }
        else if (InputController.instance.IsJumpPressed && characterController.isGrounded && playerManager.IsJumping) playerManager.IsJumping = false;
    }

    public void HandleMovement(float delta)
    {
        characterController.Move(moveDirection*delta);
    }

    public void HandleGroundedMovement()
    {
        if (!characterController.isGrounded) return;

        moveDirection = cameraObject.transform.forward * InputController.instance.VerticalInput;
        moveDirection = moveDirection + cameraObject.transform.right * InputController.instance.HorizontalInput;
        moveDirection.Normalize();
        moveDirection.y = groundedGravity;

       if (InputController.instance.MoveAmount > 0.5f) moveDirection = moveDirection*movementSpeed;
       else if (InputController.instance.MoveAmount <= 0.5f) moveDirection = moveDirection * walkingSpeed;

        playerAnimator.UpdateMovementAnimationValues(InputController.instance.MoveAmount,0);
        
    }

    public void HandleGravity(float delta)
    {
        playerManager.IsGrounded = Physics.CheckSphere(transform.position, groundCheckSphereRadius, groundLayer);
        if (!characterController.isGrounded)
        {
            float previousYVelocity = moveDirection.y;
            float newYVelocity = moveDirection.y + (gravity * delta);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            moveDirection.y = nextYVelocity;
            print(nextYVelocity);
            print("NOT Grounded");
        }
        else
        {
            moveDirection.y = groundedGravity;
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
