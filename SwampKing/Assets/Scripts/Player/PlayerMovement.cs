using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    PlayerManager playerManager;

    [SerializeField] LayerMask groundLayer;
    
    Transform cameraObject;

    [Header("CharacterMovementStats")]
    [SerializeField] float walkingSpeed = 2.5f;
    [SerializeField] float movementSpeed = 5f;
    Vector3 moveDirection; 
    Vector3 appliedMovement;
    Transform myTransform;
    [SerializeField] float rotationSpeed = 10;

    [Header("JumpStats")]
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float initialJumpVelocity;
    [SerializeField] float maxJumpHeight = 4.0f;
    [SerializeField] float maxJumpTime = 0.75f;
    [SerializeField] float groundCheckSphereRadius = 0.2f;

    public CharacterController CharacterController { get { return characterController; } }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerManager = GetComponent<PlayerManager>();
        cameraObject = Camera.main.transform;
        myTransform = transform;

        SetupJumpVariables();
    }


    public void PerformJump(float multiplier = 1)
    {
        moveDirection.y = initialJumpVelocity*multiplier;
        appliedMovement.y = initialJumpVelocity*multiplier;
        playerManager.IsJumping = true;
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
        Vector3 forward = cameraObject.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = cameraObject.right;
        right.y = 0;
        right.Normalize();

        Vector3 inputDir = forward * InputController.instance.VerticalInput
                           + right   * InputController.instance.HorizontalInput;
        inputDir.Normalize();
        
        float speed = InputController.instance.MoveAmount > 0.5f
            ? movementSpeed
            : walkingSpeed;

        moveDirection.x = inputDir.x * speed;
        moveDirection.z = inputDir.z * speed;
    }

    public bool isGrounded()
    {
        return Physics.CheckSphere(transform.position,
            groundCheckSphereRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    public void HandleGravity()
    {
        bool isGrounded = Physics.CheckSphere(transform.position,
            groundCheckSphereRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
        float fallMultiplier = 2.0f;
        if (!isGrounded && moveDirection.y < 0)
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
        moveDirection.y = -10f;
        appliedMovement.y = -10f;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
    }
}
