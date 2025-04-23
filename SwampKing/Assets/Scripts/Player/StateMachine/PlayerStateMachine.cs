using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerBaseState currentState;
    private PlayerStateFactory states;
    
    public AudioSource AudioSource;
    public AudioClip SimpleAttack;
    
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerManager PlayerManager { get; private set; }
    public PlayerAnimator PlayerAnimator {get; private set;}
    
    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    public PlayerStateFactory States { get { return states; } set { states = value; } }

    private void Start()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerManager = GetComponent<PlayerManager>();
        PlayerAnimator = GetComponent<PlayerAnimator>();
        AudioSource = GetComponent<AudioSource>();
        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();
    }

    private void Update()
    {
        currentState.UpdateStates();
        PlayerMovement.HandleMovement();
        HandleJumpCharge();
        HandleAirTimer();

        HandleAttackCounter();
    }

    private void HandleJumpCharge()
    {
        if (InputController.instance.IsJumpPressed && PlayerMovement.CharacterController.isGrounded)
        {
            if (PlayerManager.JumpChargeTime >= PlayerManager.TapTreshold) PlayerManager.IsChargingJumping = true;
            PlayerManager.JumpChargeTime += Time.deltaTime;
        }
        else PlayerManager.IsChargingJumping = false;


    }


    private void HandleAirTimer()
    {
        if (!PlayerMovement.CharacterController.isGrounded && !PlayerManager.IsJumping) PlayerManager.InAirTimer += Time.deltaTime;
    }

    private void HandleAttackCounter()
    {
        if (!PlayerManager.IsAttacking)
        {
            if (PlayerManager.PreviousIsAttacking)
            {
                PlayerManager.TimeSinceLastAttack = 0f;
                PlayerManager.PreviousIsAttacking = false;
            }
            else
            {
                PlayerManager.TimeSinceLastAttack += Time.deltaTime;
                if (PlayerManager.TimeSinceLastAttack > .5f)
                {
                    PlayerManager.AttackCount = 0;
                }
            }
        }
        else
        {
            PlayerManager.PreviousIsAttacking = true;
        }
    }
    
    private void OnAnimatorMove()
    {
        if (PlayerManager.IsAttacking)
        {
            Vector3 rootPosition = PlayerAnimator.Animator.rootPosition;
            transform.position = rootPosition;
            
            Quaternion rootRotation = PlayerAnimator.Animator.rootRotation;
            transform.rotation = rootRotation;
        }
    }
}
