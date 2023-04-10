using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerControllerManager player;
    [Header("Player Attributes")]
    public string playerName = "";
    public int xpLevel = 1;
    public int xp = 0;

    [Space]
    [Header("Player Data")]
    [Range(1f, 200f)] public float health = 100f;
    public float stress = 0f;
    public float armor = 0f;
    [Range(1f, 200f)] public float healthLimit = 100f;
    [Range(50f, 100f)] public float stressLimit = 100f;
    [Range(0f, 150f)] public float armorLimit = 150f;
    public float speed = 4f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float slideSpeed = 9.5f;
    [Range(0f, 5f)] public float jumpForce = 2f;
    [Range(-10f, -5f)] public float gravity = -9.81f;
    public float groundDist = 0.4f;
    public float playerNametagDistance = 20f;
    public float crouchYScale;

    [Space]
    [Header("Player Options")]
    public bool toggleAiming = false;
    public bool invertedMouse = false;
    public bool enableNightVision = false;

    [Space]
    [Header("Player States")]
    public bool isSprinting = false;
    public bool isAiming = false;
    public bool isWalking = false;
    public bool isJumping = false;
    public bool isCrouching = false;
    public bool isSliding = false;
    public bool isBreathing = false;
    public bool isDowned = false;
    public bool isDead = false;
    public bool isFiring = false;
    public bool isShooting = false;
    public bool onGround = false;
    public bool enableGravity = true;

    [Space]
    [Header("Player Generic Controls")]
    public bool mouseMovementEnabled = true;
    public bool playerMovementEnabled = true;
    public bool gunInteractionEnabled = true;
    public bool playerCameraBobEnabled = true;

    [Space]
    [Header("Player Camera Settings")]
    [Range(60f, 120f)] public float cameraFieldOfView = 70f;
    [Range(0f, 200f)] public float mouseSensitivity = 70f;
    [Range(-5f, -90f)] public float ClampCamRotX = -75f;
    [Range(5f, 90f)] public float ClampCamRotZ = 75f;
    [Range(1f, 1.5f)] public float sprintFOVMultiplier = 1.2f;
    [Range(2f, 20f)] public float sprintFOVChangeDuration = 12;

    [Space]
    [Header("Player SFX Controls")]
    public float baseStepSpeed = 0.3f;
    public float crouchStepMultiplier = 1.5f;
    public float sprintStepMultiplier = 0.6f;
    public AudioSource footstepAS = default;
    public AudioSource playerInternalAS = default;

    [Space]
    [Header("Local Game Stats")]
    public int totalKilledEnemies = 0;
    public int totalDealtDamage = 0;
    public float totalAbsorbedDamage = 0;
    public int totalGainedXP = 0;

    [HideInInspector] public Vector3 originalCenter;
    [HideInInspector] public float originalRadius;
    [HideInInspector] public float originalHeight;
    [HideInInspector] public Vector3 crouchCenter = new Vector3(0f, 0.91f, 0.04f);
    [HideInInspector] public float crouchRadius = 0.32f;
    [HideInInspector] public float crouchHeight = 1.05f;

    private void Awake()
    {
        originalCenter = player.body.center;
        originalRadius = player.body.radius;
        originalHeight = player.body.height;

        player.capsuleCollider.center = originalCenter;
        player.capsuleCollider.radius = originalRadius;
        player.capsuleCollider.height = originalHeight;

        crouchCenter = new Vector3(0f, 0.91f, 0.04f);
        crouchRadius = 0.32f;
        crouchHeight = 1.05f;
    }
    private void Update()
    {
        if (!player.pv.IsMine) return;
        if (health < 0) health = 0f;
        if (armor < 0) armor = 0f;
    }
    private void Start()
    {
        //SetPlayerSensitivity(PlayerPrefs.GetFloat("Mouse Sensitivity"));
    }
    public void SetPlayerSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        player.fpsCam.mouseSensitivityValve = sensitivity;
        player.fpsCam.ResetAimingSensitivity(sensitivity);
    }
    public void SetPlayerFOV(float fov)
    {
        player.fpsCam.SetPlayerFOV(fov);
        player.controls.SetNewFOV(fov);
    }
}
