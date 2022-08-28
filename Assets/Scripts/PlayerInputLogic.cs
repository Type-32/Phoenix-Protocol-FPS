using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputLogic : MonoBehaviour
{
    public PlayerManager player;
    public PlayerStats stats;
    public enum MovementState
    {
        walking,
        sprinting,
        air
    };
    void Update()
    {
        if (!stats.playerMovementEnabled) return;

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) { stats.isWalking = true; }
        else stats.isWalking = false;

        if (!stats.toggleAiming)
        {
            if (Input.GetButton("Fire2")) stats.isAiming = true;
            else stats.isAiming = false;
        }
        else
        {
            if (Input.GetButtonDown("Fire2") && !stats.isAiming) stats.isAiming = true;
            else if (Input.GetButtonDown("Fire2") && stats.isAiming) stats.isAiming = false;
        }
        if (Input.GetKeyDown("c"))
        {
            if (!stats.isCrouching)
            {
                stats.isCrouching = true;
                stats.onGround = true;
            }
            else
            {
                stats.isCrouching = false;
            }
        }
        //player.cameraAnim.SetBool("isSliding", stats.isSliding);
        //player.cameraAnim.SetBool("isCrouching", stats.isCrouching);
        if (Input.GetKey("left shift") && !stats.isAiming && !stats.isCrouching && Input.GetAxis("Vertical") > 0) 
        {
             stats.isSprinting = true;
            
        }
        else { stats.isSprinting = false; }
        
        if (Input.GetKey("space") && stats.onGround)
        {
            if (stats.isCrouching) stats.isCrouching = false;
            else stats.isJumping = true;
        }
        else stats.isJumping = false;
    }
}
