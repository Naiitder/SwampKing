using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerManager PlayerManager { get; private set; }
    public PlayerAnimator PlayerAnimator {get; private set;}

    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerStateFactory States { get { return _states; } set { _states = value; } }

    private void Awake()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerManager = GetComponent<PlayerManager>();
        PlayerAnimator = GetComponentInChildren<PlayerAnimator>();
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
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
                // Acaba de dejar de atacar
                PlayerManager.TimeSinceLastAttack = 0f;
                PlayerManager.PreviousIsAttacking = false;
            }
            else
            {
                PlayerManager.TimeSinceLastAttack += Time.deltaTime;
                if (PlayerManager.TimeSinceLastAttack > 1f)
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

            // Opcional: mantener rotación estable
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(0, currentRotation.eulerAngles.y, 0);
        }
    }
}
