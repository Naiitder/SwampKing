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
        PlayerAnimator = GetComponent<PlayerAnimator>();
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
        PlayerMovement.HandleMovement();
        HandleJumpChargeTime();
        HandleAirTimer();

    }

    private void HandleJumpChargeTime()
    {
        if (InputController.instance.IsJumpPressed && PlayerMovement.CharacterController.isGrounded) PlayerManager.JumpChargeTime += Time.deltaTime;
    }

    private void HandleAirTimer()
    {
        if (!PlayerMovement.CharacterController.isGrounded) PlayerManager.InAirTimer += Time.deltaTime;
    }
}
