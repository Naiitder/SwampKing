using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerBaseState currentState;
    private PlayerStateFactory states;
    
    public AudioSource AudioSource;
    public AudioClip SimpleAttackSound;
    public AudioClip ShootSound;

    public NPCDialogueTrigger npc;
    public GameObject InteractionPrompt; 
    
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TMP_SpriteAsset gamepadSpriteAsset;

    [Header("GunStats")] 
    public GameObject GunProjectilePrefab;
    public Transform ShootPoint;
    
    private Collider[] npcBuffer = new Collider[10];

    public CameraController CameraController;
    
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
        
        if (PlayerManager.easeHealthSlider.value != PlayerManager.healthSlider.value)
            PlayerManager.easeHealthSlider.value = Mathf.Lerp(PlayerManager.easeHealthSlider.value, PlayerManager.healthSlider.value, 0.05f);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    void UpdateInteractionPrompt()
    {
        if (npc != null && !DialogueManager.instance.dialogueBox.activeSelf)
        {
            InteractionPrompt.SetActive(true);

            var device = InputController.instance.LastUsedDevice;

            if (device is Gamepad)
            {
                promptText.text = "<sprite name=\"WestButton_Gamepad\">: Hablar";   
            }
            else if (device is Keyboard)
            {
                promptText.text = "<sprite name=\"KeyboardButtons_E\">: Hablar";
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
        Vector3 center = transform.position + Vector3.up * 1.5f; 

        int count = Physics.OverlapSphereNonAlloc(center, interactionRadius, npcBuffer, LayerMask.GetMask("NPC"));

        for (int i = 0; i < count; i++)
        {
            Collider col = npcBuffer[i];
            if (col == null) continue;

            NPCDialogueTrigger npc = col.GetComponent<NPCDialogueTrigger>();
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
