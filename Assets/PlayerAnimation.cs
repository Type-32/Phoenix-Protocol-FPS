using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public PlayerControllerManager player;
    public Animator animator;
    void Update()
    {
        if (!player.pv.IsMine)
        {
            animator.SetBool("isSprinting", player.stats.isSprinting);
            animator.SetBool("isCrouching", player.stats.isCrouching);
            animator.SetBool("isSliding", player.stats.isSliding);
            animator.SetBool("isWalking", player.stats.isWalking);
            animator.SetBool("isDowned", player.stats.isDowned);
            animator.SetBool("isDead", player.stats.isDead);
            player.holder.weaponSlots[player.holder.weaponIndex].gun.animate.animate.SetBool("isSprinting", player.stats.isSprinting);
            player.holder.weaponSlots[player.holder.weaponIndex].gun.animate.animate.SetBool("isSliding", player.stats.isSliding);
        }
    }
}
