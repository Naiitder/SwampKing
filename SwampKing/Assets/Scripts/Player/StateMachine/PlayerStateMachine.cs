using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerBaseState currentState;
    private PlayerStateFactory states;
    
    public AudioSource AudioSource;
    public AudioClip SimpleAttack;

    public NPCDialogueTrigger npc;
    public GameObject InteractionPrompt; 
    
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TMP_SpriteAsset gamepadSpriteAsset;

    
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
        
        promptText.spriteAsset = gamepadSpriteAsset;
    }

    private void Update()
    {
        currentState.UpdateStates();
        PlayerMovement.HandleMovement();
        HandleJumpCharge();
        HandleAirTimer();

        npc = FindNPC();
        
        UpdateInteractionPrompt();

        HandleAttackCounter();
    }
    
    void UpdateInteractionPrompt()
    {
        if (npc != null && !DialogueManager.instance.dialogueBox.activeSelf)
        {
            InteractionPrompt.SetActive(true);

            var device = InputController.instance.LastUsedDevice;

            if (device is Gamepad)
            {
                promptText.text = "Presiona <sprite name=\"WestButton_Gamepad\"> para interactuar";   
            }
            //Todo Cambiarlo por un sprite
            else if (device is Keyboard)
            {
                promptText.text = "Presiona 'E' para interactuar";
            }
        }
        else
        {
            InteractionPrompt.SetActive(false);
        }
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
    
    private NPCDialogueTrigger FindNPC()
    {
        float interactionRadius = 2f;
        Vector3 center = transform.position + Vector3.up;

        Collider[] colliders = Physics.OverlapSphere(center, interactionRadius);
        foreach (Collider collider in colliders)
        {
            NPCDialogueTrigger npc = collider.GetComponent<NPCDialogueTrigger>();
            if (npc != null)
            {
                return npc;
            }
        }

        return null;
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
