using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControls : MonoBehaviour
{

    [SerializeField] PlayerControllerManager player;
    public float speedValve = 0f;
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
    Vector3 smoothedPlayerInput;
    Vector3 smoothMoveVelocity;
    public float smoothTime = 1.5f;
    float x, z;
    Vector3 posTemp;
    Quaternion rotTemp;
    [HideInInspector] public float slideTime = 0f;
    float slideValveSpeed;
    bool hadSlide = true;

    [Space]
    [SerializeField] Vector3 velocityDebug;
    private void Awake()
    {
        normalFOV = player.stats.cameraFieldOfView;
        sprintFOV = player.stats.sprintFOVMultiplier * player.stats.cameraFieldOfView;
        //slideTime = 0.7f;
    }
    public void SetNewFOV(float value)
    {
        normalFOV = value;
        sprintFOV = player.stats.sprintFOVMultiplier * value;
    }
    private void Start()
    {
        if (!player.pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            return;
        }
        hadSlide = true;
        posTemp = transform.position;
        rotTemp = transform.rotation;
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
        if (!player.pv.IsMine) return;

        velocityDebug = player.body.velocity;
        x = player.stats.playerMovementEnabled ? Input.GetAxis("Horizontal") : 0f;
        z = player.stats.playerMovementEnabled ? Input.GetAxis("Vertical") : 0f;
        player.holder.WeaponFunction();
        //if (Input.GetKeyDown("l")) Cursor.lockState = CursorLockMode.None;
        if (slideTime <= 0f)
        {
            player.stats.isSliding = false;
            if (!hadSlide)
            {
                player.SynchronizePlayerState(player.stats.isSliding, 2);
                hadSlide = true;
            }
        }
        else
        {
            player.stats.isSliding = true;
        }
        if (slideTime > 0f)
        {
            slideValveSpeed = Mathf.Lerp(slideValveSpeed, player.stats.speed, Time.deltaTime * 1.5f);
            slideTime -= Time.deltaTime;
        }
        Logics();
        GroundCheck();
        Gravity();

        Movement();
        CameraFOV();
        InteractIndicatorCheck();
        KeybindedActions();

    }
    public void InteractWithPickup()
    {
        player.pv.RPC(nameof(RPC_InteractWithPickup), RpcTarget.All, player.fpsCam.transform.position, player.fpsCam.transform.rotation, player.fpsCam.transform.forward);
    }
    public void InteractIndicatorCheck()
    {
        RaycastHit detectRay;
        if (Physics.Raycast(player.fpsCam.transform.position, player.fpsCam.transform.forward, out detectRay, 3f))
        {
            Pickup temp = detectRay.collider.GetComponent<Pickup>();
            if (temp != null && temp.supplyData != null)
            {
                player.ui.interactionIndicator.gameObject.SetActive(true);
            }
            else
            {
                player.ui.interactionIndicator.gameObject.SetActive(false);
            }
        }
        else
        {
            player.ui.interactionIndicator.gameObject.SetActive(false);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.groundCheck.position, player.stats.groundDist);
    }
    void Movement()
    {
        if (player.stats.onGround) speedValve = player.stats.isSliding ? slideValveSpeed : player.stats.isSprinting ? player.stats.sprintSpeed : player.stats.isCrouching ? player.stats.crouchSpeed : player.stats.speed;
        if (player.stats.playerMovementEnabled)
        {
            playerInput = player.transform.right * x + transform.forward * z;
            player.body.Move(playerInput * speedValve * Time.deltaTime);
        }
        if (player.stats.onGround && player.stats.isJumping) velocity.y = Mathf.Sqrt((!player.stats.isSliding ? player.stats.jumpForce : player.stats.jumpForce * 0.2f) * -2f * player.stats.gravity);
        player.capsuleCollider.height = player.stats.isCrouching ? capsuleColliderCrouchHeight : capsuleColliderInitHeight;
    }
    void FixedMovement()
    {
        //transform.position
    }
    void Gravity()
    {
        if (!player.stats.enableGravity) return;
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
        return;
        if (!player.stats.isAiming) player.fpsCam.playerMainCamera.fieldOfView = player.stats.isSprinting ? Mathf.Lerp(player.fpsCam.playerMainCamera.fieldOfView, sprintFOV, player.stats.sprintFOVChangeDuration * Time.deltaTime) : Mathf.Lerp(player.fpsCam.playerMainCamera.fieldOfView, normalFOV, player.stats.sprintFOVChangeDuration * Time.deltaTime);
    }
    void KeybindedActions()
    {
        if (Input.GetKeyDown("f")) InteractWithPickup();
        if (Input.GetKeyDown("n")) player.ToggleNightVision();
        if (Input.GetKeyDown("x") && player.playerManager.recordKills >= 3 && !player.usingStreakGifts) StartCoroutine(player.UseStreakGift(5f, 3));
        if (Input.GetKeyDown("k")) player.TakeDamage(100f, true, transform.position, transform.rotation, 0, true);
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
            player.stats.isAiming = (Input.GetButton("Fire2") | Input.GetKey(KeyCode.RightShift) | Input.GetKey(KeyCode.RightBracket)) & !player.stats.isSliding ? true : false;
        }
        else
        {
            if ((Input.GetButtonDown("Fire2") || (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.RightBracket))) && !player.stats.isAiming) player.stats.isAiming = true;
            else if ((Input.GetButtonDown("Fire2") || (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.RightBracket))) && player.stats.isAiming) player.stats.isAiming = false;
        }
    }
    void CrouchingLogic()
    {
        if (Input.GetKeyDown("c") && player.stats.onGround)
        {
            if (player.stats.isSliding)
            {
                ActivateSlide();
                //player.stats.onGround = true;
            }
            else if (player.stats.isSprinting && player.stats.onGround)
            {
                if (!player.stats.isSliding)
                {
                    ActivateSlide();
                }
            }
            else if (!player.stats.isCrouching)
            {
                player.stats.isCrouching = true;
                player.stats.onGround = true;
            }
            else
            {
                player.stats.isCrouching = false;
            }
            player.SynchronizePlayerState(player.stats.isCrouching, 1);
            player.SynchronizePlayerState(player.stats.isSliding, 2);
        }
        if (player.stats.isCrouching || player.stats.isSliding) player.ChangePlayerHitbox(player.stats.crouchCenter, player.stats.crouchRadius, player.stats.crouchHeight);
        else player.ChangePlayerHitbox(player.stats.originalCenter, player.stats.originalRadius, player.stats.originalHeight);
        player.fpsCam.SetPlayerVerticalPosition(player.stats.isCrouching ? 1.1f : 1.461f);
        player.stats.onGround = true;
    }
    void JumpingLogic()
    {
        if (Input.GetKey("space") && player.stats.onGround)
        {
            /*
            if (player.stats.isSliding)
                ActivateSlide();*/
            if (!player.stats.isSliding)
            {
                if (player.stats.isCrouching) player.stats.isCrouching = false;
                else player.stats.isJumping = true;
            }
            else
                player.stats.isJumping = true;
        }
        else player.stats.isJumping = false;
    }
    void SprintingLogic()
    {
        if (Input.GetKey("left shift") && !player.stats.isAiming && !player.stats.isCrouching && Input.GetAxis("Vertical") > 0 && !player.stats.isSliding)
        {
            player.stats.isSprinting = true;
        }
        else
        {
            player.stats.isSprinting = false;
        }
        if (Input.GetKeyDown("left shift"))
        {
            player.SynchronizePlayerState(player.stats.isSprinting, 0);
        }
        else if (Input.GetKeyUp("left shift"))
        {
            player.SynchronizePlayerState(player.stats.isSprinting, 0);
        }
    }
    bool localVal = false;
    void WalkingLogic()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) { player.stats.isWalking = true; }
        else player.stats.isWalking = false;
        if ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) == 0 && !localVal)
        {
            player.SynchronizePlayerState(player.stats.isWalking, 3);
            localVal = true;
        }
        else
        {
            player.SynchronizePlayerState(player.stats.isWalking, 3);
            localVal = false;
        }
    }
    #endregion
    void ActivateSlide()
    {
        if (player.stats.isSliding)
        {
            slideTime = 0f;
            player.stats.isCrouching = false;
            slideValveSpeed = player.stats.slideSpeed;
            player.SynchronizePlayerState(false, 2);
            return;
        }
        player.stats.isCrouching = true;
        slideTime = 0.7f;
        slideValveSpeed = player.stats.slideSpeed;
        player.SynchronizePlayerState(true, 2);
        hadSlide = false;
        return;
    }
    Transform nullTransform;
    [PunRPC]
    void RPC_InteractWithPickup(Vector3 playerPos, Quaternion playerRot, Vector3 playerForward)
    {
        RaycastHit detectRay;
        if (Physics.Raycast(playerPos, playerForward, out detectRay, 3f))
        {
            Pickup temp = detectRay.collider.GetComponent<Pickup>();
            if (temp != null && temp.supplyData != null)
            {
                if (temp.supplyData.supplyAmmo)
                {
                    int _mapg = player.holder.weaponSlots[player.holder.weaponIndex].gun.stats.weaponData.maxAmmoPerMag;
                    player.holder.weaponSlots[player.holder.weaponIndex].gun.stats.ammoPool += _mapg * temp.supplyData.supplyAmmoMagAmount;
                }
                if (temp.supplyData.supplyArmor)
                {
                    float _armor = temp.supplyData.supplyArmorAmount;
                    player.stats.armor += _armor;
                    if (player.stats.armor > player.stats.armorLimit) player.stats.armor = player.stats.armorLimit;
                }
                if (temp.supplyData.supplyHealth)
                {
                    float _health = temp.supplyData.supplyHealthAmount;
                    player.stats.health += _health;
                    if (player.stats.health > player.stats.healthLimit) player.stats.health = player.stats.healthLimit;
                }
                if (temp.supplyData.destroyOnUse) Destroy(temp.gameObject);
            }
        }
    }
}
