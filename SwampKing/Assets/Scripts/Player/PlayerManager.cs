using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("PlayerFlags")]
    [SerializeField] bool isJumping;
    [SerializeField] bool isChargingJumping;
    [SerializeField] bool isGrounded;

    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsChargingJumping { get { return isChargingJumping; } set { isChargingJumping = value; } }

}
