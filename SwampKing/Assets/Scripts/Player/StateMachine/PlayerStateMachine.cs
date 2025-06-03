using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerBaseState currentState;
    private PlayerStateFactory states;
    
    public GameObject JumpTrail;
    
    [Header ("Audios")]
    public AudioSource AudioSource;
    public AudioClip SimpleAttackSound;
    public AudioClip ShootSound;
    public AudioClip JumpSound;

    [Header ("FootSteeps")]
    [SerializeField] private AudioSource footAudioSource;
    float stepInterval = 0.5f;
    float stepTimer;
    [SerializeField] private LayerMask groundLayer;
    Terrain terrain;
    TerrainData terrainData;
    int alphamapWidth;
    int alphamapHeight;
    public AudioClip[] mudSounds;
    public AudioClip[] grassSounds;
    public AudioClip[] waterSounds;
    public AudioClip[] woodSounds;

    
    public NPCDialogueTrigger npc;
    public GameObject InteractionPrompt; 
    
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TMP_SpriteAsset gamepadSpriteAsset;

    [Header("GunStats")] 
    public GameObject GunProjectilePrefab;
    public ParticleSystem GunShootParticles;
    public Transform ShootPoint;
    
    private Collider[] npcBuffer = new Collider[10];
    
    [Header("Cinemachine")]
    public CinemachineCamera aimCamera;
    public GameObject cameraObject;

    
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
        
        
        terrain = Terrain.activeTerrain;
        terrainData = terrain.terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;
        
        JumpTrail.SetActive(false);
    }

    private void Update()
    {
        currentState.UpdateStates();
        PlayerMovement.HandleMovement();
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
    
    public void HandleFootSteepsSound()
    {
        if (PlayerMovement.CharacterController.isGrounded && PlayerMovement.CharacterController.velocity.magnitude > 0.2f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayFootstepSound();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }
    
    void PlayFootstepSound()
    {
        Vector3 footPosition = transform.position;
        
        if (Physics.Raycast(footPosition + Vector3.up, Vector3.down, out RaycastHit hit, 3f, groundLayer))
        {
            if (hit.collider.CompareTag("Water"))
            {
                PlayRandomClip(waterSounds);
                footAudioSource.volume = 0.2f;
                return;
            }
            else if (hit.collider.CompareTag("Wood"))
            {
                PlayRandomClip(woodSounds);
                footAudioSource.volume = 0.4f;
                return;
            }
            else if (hit.collider.CompareTag("Lilypad"))
            {
                PlayRandomClip(grassSounds);
                footAudioSource.volume = 0.2f;
                return;
            }
            footAudioSource.volume = 0.2f;

        }
        
        int textureIndex = GetMainTexture(transform.position);

        switch (textureIndex)
        {
            case 0:
                PlayRandomClip(mudSounds);
                break;
            case 1:
                PlayRandomClip(grassSounds);
                break;
            default:
                PlayRandomClip(mudSounds);
                break;
        }
    }
    
    int GetMainTexture(Vector3 worldPos)
    {
        Vector3 terrainPos = terrain.transform.InverseTransformPoint(worldPos);

        int mapX = Mathf.Clamp((int)((terrainPos.x / terrainData.size.x) * alphamapWidth), 0, alphamapWidth - 1);
        int mapZ = Mathf.Clamp((int)((terrainPos.z / terrainData.size.z) * alphamapHeight), 0, alphamapHeight - 1);

        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        int maxIndex = 0;
        float maxMix = 0;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxMix)
            {
                maxIndex = i;
                maxMix = splatmapData[0, 0, i];
            }
        }

        return maxIndex;
    }
    
    void PlayRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        float pitch = Random.Range(0.9f, 1.1f);

        footAudioSource.pitch = pitch;
        footAudioSource.PlayOneShot(clips[index]);
    }

}
