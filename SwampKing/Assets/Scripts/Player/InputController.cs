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
    [SerializeField] private bool isInteractPressed;
    [SerializeField] private bool isAimingPressed;
    
    public InputDevice LastUsedDevice { get; private set; }

    public Queue<InputActionType> InputBuffer = new Queue<InputActionType>();
    public enum InputActionType { Jump, Attack, Interact }

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
    
    public bool IsInteractPressed { get { return isInteractPressed; } set { isInteractPressed = value; } }
    public bool IsAimingPressed { get { return isAimingPressed; } set { isAimingPressed = value; } }
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
            playerControlls.Locomotion.Jump.started +=  onJumpInputStart;
            playerControlls.Locomotion.Jump.canceled += onJumpInputExit;
            playerControlls.Actions.Attack.started += onAttackInputStart;
            playerControlls.Actions.Attack.canceled += onAttackInputExit;
            playerControlls.UserActions.Pause.started +=  onPauseInputStart;
            playerControlls.Actions.Interact.started += onInteractStart;
            playerControlls.Actions.Interact.canceled +=  onInteractExit;
            playerControlls.Actions.Aiming.started += onAimingStart;
            playerControlls.Actions.Aiming.canceled +=  onAimingExit;

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
        LastUsedDevice = context.control.device;
        
        movementInput = context.ReadValue<Vector2>();
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
    }
    void onJumpInputStart(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;
        
        isJumpPressed = true;
    }

    void onJumpInputExit(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;
        
        isJumpPressed = false;
        InputBuffer.Enqueue(InputActionType.Jump);
    }
    
    void onAimingStart(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;
        
        isAimingPressed = true;
    }

    void onAimingExit(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;
        
        isAimingPressed = false;
    }

    void onAttackInputStart(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;

        isAttackPressed = true;
    }
    
    

    void onAttackInputExit(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;

        isAttackPressed = false;
        InputBuffer.Enqueue(InputActionType.Attack);
    }

    void onPauseInputStart(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;

        isPausePressed = !isPausePressed; 
    }
    
    void onInteractExit(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;

        isInteractPressed = true;
        InputBuffer.Enqueue(InputActionType.Interact);
    }

    void onInteractStart(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;

        isInteractPressed = false; 
    }

    void onCameraInput(InputAction.CallbackContext context)
    {
        LastUsedDevice = context.control.device;

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
