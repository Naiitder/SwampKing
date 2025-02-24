using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PlayerMovement playerMovement;

    [Header("PlayerFlags")]
    [SerializeField] bool isJumping;
    [SerializeField] bool isGrounded;

    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }


    private void Update()
    {
        float delta = Time.deltaTime;
        playerMovement.HandleGroundedMovement();
        playerMovement.HandleRotation(delta);
        playerMovement.HandleGravity(delta);
        playerMovement.HandleJump();
        playerMovement.HandleMovement(delta);



    }


}
