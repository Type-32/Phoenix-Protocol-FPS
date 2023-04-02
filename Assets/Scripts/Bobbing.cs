using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float crouchBobAmount = 0.1f;
    public float walkBobAmount = 0.1f;
    public float sprintBobAmount = 0.15f;
    public float crouchBobSpeed = 10f;
    public float walkBobSpeed = 14f;
    public float sprintBobSpeed = 16f;
    public float returnDuration = 5f;
    public PlayerControllerManager player;

    private float defaultYPos = 0f;
    private float defaultXPos = 0f;
    private float timer;

    private void Start()
    {
        if (!player.pv.IsMine) Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (!player.pv.IsMine) return;
        if (!player.stats.playerMovementEnabled) return;
        transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, defaultXPos, Time.deltaTime * 3), Mathf.Lerp(transform.localPosition.y, defaultYPos, Time.deltaTime * 3), transform.localPosition.z);
        if (player.stats.isAiming) return;
        CameraBob();
    }
    public void CameraBob()
    {
        //Debug.Log("Check CamBob");
        if (!player.stats.onGround) return;

        if ((Mathf.Abs(Input.GetAxis("Horizontal")) > 0f || Mathf.Abs(Input.GetAxis("Vertical")) > 0f))
        {
            timer += Time.deltaTime * (player.stats.isSprinting ? sprintBobSpeed : player.stats.isCrouching ? crouchBobSpeed : walkBobSpeed);
            transform.localPosition = new Vector3(defaultXPos + (Mathf.Cos(timer / 2) * 0.8f) * (player.stats.isSprinting ? sprintBobAmount : player.stats.isCrouching ? crouchBobAmount : walkBobAmount), defaultYPos + Mathf.Sin(timer) * (player.stats.isSprinting ? sprintBobAmount : player.stats.isCrouching ? crouchBobAmount : walkBobAmount), transform.localPosition.z);
        }
    }
}
