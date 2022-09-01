using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerControls : MonoBehaviour
{

    [SerializeField] PlayerControllerManager player;
    private float speedValve = 0f;
    [HideInInspector] public Vector3 playerInput;
    private float normalFOV;
    private float sprintFOV;
    private float startYScale;
    private float capsuleColliderInitHeight;
    private float capsuleColliderCrouchHeight;
    public Vector3 cameraInitPos;
    public Vector3 cameraCrouchPos;
    public float aimingMouseSensitivity;
    private MouseLookScript mouseLook;
    [SerializeField] Vector3 velocity;
    InteractionIndicatorScript interactionIndicator;

    [Space]
    [Header("Multiplayer")]
    [SerializeField] PhotonView pv;

    float x,z;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        normalFOV = player.stats.cameraFieldOfView;
        sprintFOV = player.stats.sprintFOVMultiplier * player.stats.cameraFieldOfView;
    }
    private void Start()
    {
        if (!pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        mouseLook = player.fpsCam.GetComponent<MouseLookScript>();
        capsuleColliderInitHeight = player.capsuleCollider.height;
        capsuleColliderCrouchHeight = player.capsuleCollider.height / 2;
        cameraInitPos = player.fpsCam.transform.position;
        cameraCrouchPos = player.fpsCam.transform.position;
        cameraCrouchPos = new Vector3(cameraCrouchPos.x, 0, cameraCrouchPos.z);
        startYScale = transform.localScale.y;
        interactionIndicator = player.ui.interactionIndicator.GetComponent<InteractionIndicatorScript>();
        aimingMouseSensitivity = player.stats.mouseSensitivity * 0.8f;
    }
    void Update()
    {
        if (!pv.IsMine) return;
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        Logics();

        GroundCheck();
        Movement();
        CameraFOV();
        Gravity();
        InteractIndicatorCheck();
        KeybindedActions();
    }
    public void InteractIndicatorCheck()
    {
        RaycastHit detectRay;
        if (Physics.Raycast(player.fpsCam.transform.position, player.fpsCam.transform.forward, out detectRay, 3f))
        {
            Pickup temp = detectRay.collider.GetComponent<Pickup>();
            if (temp != null && temp.itemData != null)
            {
                player.ui.interactionIndicator.gameObject.SetActive(true);
            }
            else
            {
                player.ui.interactionIndicator.gameObject.SetActive(false);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.groundCheck.position, player.stats.groundDist);
    }
    void Movement()
    {
        speedValve = player.stats.isSprinting ? player.stats.sprintSpeed : player.stats.isCrouching ? player.stats.crouchSpeed : player.stats.speed;
        if (player.stats.playerMovementEnabled)
        {
            playerInput = player.transform.right * x + transform.forward * z;
            player.body.Move(playerInput * speedValve * Time.deltaTime);
            //player.body.velocity = MoveVec;
        }
        if (player.stats.onGround && player.stats.isJumping) velocity.y = Mathf.Sqrt(player.stats.jumpForce * -2f * player.stats.gravity);
        player.capsuleCollider.height = player.stats.isCrouching ? capsuleColliderCrouchHeight : capsuleColliderInitHeight;
    }
    void Gravity()
    {
        velocity.y += player.stats.gravity * Time.deltaTime;
        player.body.Move(velocity * Time.deltaTime);
    }
    void GroundCheck()
    {
        player.stats.onGround = Physics.CheckSphere(player.groundCheck.position, player.stats.groundDist, player.groundMask);
        if (player.stats.onGround && velocity.y < 0) velocity.y = -2f;
    }
    void CameraFOV()
    {
        player.fpsCam.playerMainCamera.fieldOfView = player.stats.isSprinting ? Mathf.Lerp(player.fpsCam.playerMainCamera.fieldOfView, sprintFOV, player.stats.sprintFOVChangeDuration * Time.deltaTime) : player.fpsCam.playerMainCamera.fieldOfView = Mathf.Lerp(player.fpsCam.playerMainCamera.fieldOfView, normalFOV, player.stats.sprintFOVChangeDuration * Time.deltaTime);
    }
    void KeybindedActions()
    {
        if (Input.GetKeyDown("f")) player.GetPickupsForPlayer();
        if (Input.GetKeyDown("k")) player.TakeDamageFromPlayer(100f, false);
    }
    void Logics()
    {
        AimingLogic();
        if (!player.stats.playerMovementEnabled) return;
        CrouchingLogic();
        JumpingLogic();
        SprintingLogic();
        WalkingLogic();
        //player.cameraAnim.SetBool("isSliding", stats.isSliding);
        //player.cameraAnim.SetBool("isCrouching", stats.isCrouching);
    }
    #region Logic Snippets
    void AimingLogic()
    {
        if (!player.stats.toggleAiming)
        {
            player.stats.isAiming = Input.GetButton("Fire2") ? true : false;
        }
        else
        {
            if (Input.GetButtonDown("Fire2") && !player.stats.isAiming) player.stats.isAiming = true;
            else if (Input.GetButtonDown("Fire2") && player.stats.isAiming) player.stats.isAiming = false;
        }
    }
    void CrouchingLogic()
    {
        if (Input.GetKeyDown("c"))
        {
            if (!player.stats.isCrouching)
            {
                player.stats.isCrouching = true;
                player.stats.onGround = true;
            }
            else
            {
                player.stats.isCrouching = false;
            }
        }
    }
    void JumpingLogic()
    {
        if (Input.GetKey("space") && player.stats.onGround)
        {
            if (player.stats.isCrouching) player.stats.isCrouching = false;
            else player.stats.isJumping = true;
        }
        else player.stats.isJumping = false;
    }
    void SprintingLogic()
    {
        if (Input.GetKey("left shift") && !player.stats.isAiming && !player.stats.isCrouching && Input.GetAxis("Vertical") > 0)
        {
            player.stats.isSprinting = true;

        }
        else { player.stats.isSprinting = false; }
    }
    void WalkingLogic()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) { player.stats.isWalking = true; }
        else player.stats.isWalking = false;
    }
    #endregion
}