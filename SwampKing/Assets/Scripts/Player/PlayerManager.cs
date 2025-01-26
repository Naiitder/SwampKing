using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("PlayerFlags")]
    [SerializeField] bool isJumping;
    [SerializeField] bool isChargingJumping;
    [SerializeField] bool isGrounded;

    [SerializeField] private bool canDoubleJump;

    [SerializeField] float maxChargeTime = 1.0f;
    [SerializeField] float tapThreshold = 0.2f;
    [SerializeField] private float jumpChargeTime = 0f;

    public float JumpChargeTime { get { return jumpChargeTime; } set { jumpChargeTime = value; } }
    public float TapTreshold { get { return tapThreshold; } set { tapThreshold = value; } }
    public float MaxChargeTime { get { return maxChargeTime; } set { maxChargeTime = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsChargingJumping { get { return isChargingJumping; } set { isChargingJumping = value; } }

}
