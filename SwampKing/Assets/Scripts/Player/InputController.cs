using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController instance;

    PlayerControlls playerControlls;

    private float verticalInput;
    private float horizontalInput;
    private float moveAmount;

    private float cameraVerticalInput;
    private float cameraHorizontalInput;

    private Vector2 movementInput;
    private Vector2 cameraInput;
    private bool requireNewJumpPress;
    private bool isJumpPressed;

    #region GettersAndSetters
    public float VerticalInput { get { return verticalInput; } }
    public float HorizontalInput { get { return horizontalInput; } }
    public float MoveAmount { get { return moveAmount; } }
    public float CameraVerticalInput { get { return cameraVerticalInput; } }
    public float CameraHorizontalInput { get { return cameraHorizontalInput; } }
    public Vector2 MovementInput { get { return movementInput; } }
    public Vector2 CameraInput { get { return cameraInput; } }
    public bool IsJumpPressed { get { return isJumpPressed; } set { isJumpPressed = value; } }
    public bool RequireNewJumpPress { get { return requireNewJumpPress; } set { requireNewJumpPress = value; } }
    #endregion

    //public delegate void MovementInputEvent(float horizontal, float vertical, float delta);
    //public event MovementInputEvent OnMovementInputEvent;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

    }

    private void OnEnable()
    {
        if (playerControlls == null)
        {
            playerControlls = new PlayerControlls();

            playerControlls.Locomotion.Movement.started += onMovementInput;
            playerControlls.Locomotion.Movement.canceled += onMovementInput;
            playerControlls.Locomotion.Movement.performed += onMovementInput;
            playerControlls.Locomotion.Camera.performed += onCameraInput;
            playerControlls.Locomotion.Jump.started += onJumpInput;
            playerControlls.Locomotion.Jump.canceled += onJumpInput;

        }
        playerControlls.Enable();
    }

    private void OnDisable()
    {
        playerControlls.Disable();
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
    }
    void onJumpInput(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        requireNewJumpPress = false;
    }

    void onCameraInput(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
        cameraHorizontalInput = cameraInput.x;
        cameraVerticalInput = cameraInput.y;
    }


}
