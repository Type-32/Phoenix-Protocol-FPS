using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    public PlayerManager player;
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
    private void Awake()
    {
        normalFOV = player.stats.cameraFieldOfView;
        sprintFOV = player.stats.sprintFOVMultiplier * player.stats.cameraFieldOfView;
    }
    private void Start()
    {
        mouseLook = player.fpsCam.GetComponent<MouseLookScript>();

        capsuleColliderInitHeight = player.capsuleCollider.height;
        capsuleColliderCrouchHeight = player.capsuleCollider.height / 2;
        cameraInitPos = player.fpsCam.transform.position;
        cameraCrouchPos = player.fpsCam.transform.position;
        cameraCrouchPos = new Vector3(cameraCrouchPos.x, 0, cameraCrouchPos.z);
        startYScale = transform.localScale.y;
        interactionIndicator = player.ui.interactionIndicator.GetComponent<InteractionIndicatorScript>();
    }
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        aimingMouseSensitivity = player.stats.mouseSensitivity * 0.8f;
        //Logic
        if (player.stats.isSprinting) speedValve = player.stats.sprintSpeed;
        else if (player.stats.isCrouching && !player.stats.isSliding) speedValve = player.stats.crouchSpeed;
        else speedValve = player.stats.speed;
        if (player.stats.onGround && player.stats.isJumping) velocity.y = Mathf.Sqrt(player.stats.jumpForce * -2f * player.stats.gravity);
        if (Input.GetKeyDown("f")) player.GetPickupsForPlayer();

        player.stats.onGround = Physics.CheckSphere(player.groundCheck.position, player.stats.groundDist, player.groundMask);
        if(player.stats.onGround && velocity.y < 0) velocity.y = -2f;

        if (player.stats.playerMovementEnabled)
        {
            playerInput = player.transform.right * x + transform.forward * z;
            player.body.Move(playerInput * speedValve * Time.deltaTime);
            //player.body.velocity = MoveVec;
        }
        velocity.y += player.stats.gravity * Time.deltaTime;
        player.body.Move(velocity * Time.deltaTime);


        //Player Camera FOV Modification
        if (player.stats.isSprinting) player.fpsCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(player.fpsCam.GetComponent<Camera>().fieldOfView, sprintFOV, player.stats.sprintFOVChangeDuration * Time.deltaTime);
        else player.fpsCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(player.fpsCam.GetComponent<Camera>().fieldOfView, normalFOV, player.stats.sprintFOVChangeDuration * Time.deltaTime);

        if (player.stats.isCrouching)
        {
            player.capsuleCollider.height = capsuleColliderCrouchHeight;
            //transform.localScale = new Vector3(transform.localScale.x, player.stats.crouchYScale, transform.localScale.x);
        }
        else
        {
            player.capsuleCollider.height = capsuleColliderInitHeight;
            //transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.x);
        }
        if (Input.GetKeyDown("k")) player.TakeDamageFromPlayer(100f, false);
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
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.groundCheck.position, player.stats.groundDist);
    }

}
