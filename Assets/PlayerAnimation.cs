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
        }
    }
}
