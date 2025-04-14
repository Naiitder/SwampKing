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
    [SerializeField] private bool isJumpPressed;
    [SerializeField] private bool isAttackPressed;
    [SerializeField] private bool isPausePressed;

    public Queue<InputActionType> InputBuffer = new Queue<InputActionType>();
    public enum InputActionType { Jump, Attack, Aim }

    #region GettersAndSetters
    public float VerticalInput { get { return verticalInput; } }
    public float HorizontalInput { get { return horizontalInput; } }
    public float MoveAmount { get { return moveAmount; } }
    public float CameraVerticalInput { get { return cameraVerticalInput; } }
    public float CameraHorizontalInput { get { return cameraHorizontalInput; } }
    public Vector2 MovementInput { get { return movementInput; } }
    public Vector2 CameraInput { get { return cameraInput; } }
    public bool IsJumpPressed { get { return isJumpPressed; } set { isJumpPressed = value; } }
    public bool IsAttackPressed { get { return isAttackPressed; } set { isAttackPressed = value; } }
    public bool IsPausePressed { get { return isPausePressed; } set { isPausePressed = value; } }
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
            playerControlls.Locomotion.Jump.started += ctx => onJumpInputStart();
            playerControlls.Locomotion.Jump.canceled += ctx => onJumpInputExit();
            playerControlls.Actions.Attack.started += ctx => onAttackInputStart();
            playerControlls.Actions.Attack.canceled += ctx => onAttackInputExit();
            playerControlls.UserActions.Pause.started += ctx => onPauseInputStart();

        }
        playerControlls.Enable();
        StartCoroutine(ClearInputBufferRoutine());
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
    void onJumpInputStart()
    {
        isJumpPressed = true;
    }

    void onJumpInputExit()
    {
        isJumpPressed = false;
        InputBuffer.Enqueue(InputActionType.Jump);
    }

    void onAttackInputStart()
    {
        isAttackPressed = true;
    }

    void onAttackInputExit()
    {
        isAttackPressed = false;
        InputBuffer.Enqueue(InputActionType.Attack);
    }

    void onPauseInputStart()
    {
        isPausePressed = !isPausePressed; 
    }

    void onCameraInput(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
        cameraHorizontalInput = cameraInput.x;
        cameraVerticalInput = cameraInput.y;
    }

    public bool CheckActions(InputActionType action)
    {
        if (InputBuffer.Count > 0)
        {
            if (InputBuffer.Peek() == action)
            {
                //InputBuffer.Dequeue(); 
                return true;
            }
        }
        return false;

    }

    private IEnumerator ClearInputBufferRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (InputBuffer.Count > 0)
            {
                InputBuffer.Clear();
            }
        }
    }

}
