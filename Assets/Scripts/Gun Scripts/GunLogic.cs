using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunLogic : MonoBehaviour
{
    public GunManager gun;
    public bool isAiming = false;
    public bool isSprinting = false;
    [HideInInspector] public float spreadConstant = 1f;
    // Update is called once per frame
    /*
    void Update()
    {
        if (!gun.stats.gunInteractionEnabled) return;
    }*/
    public void GunGeneralLogic()
    {
        AimingLogic();
        GunMovementLogic();
        AttachmentLogic();
        HipfireSpreadConstantLogic();
    }
    void HipfireSpreadConstantLogic()
    {
        if (gun.stats.isAiming)
        {
            spreadConstant = Mathf.Lerp(spreadConstant, gun.stats.weaponData.weaponType == QuantityStatsHUD.WeaponType.Shotgun ? 0.3f : 0f, gun.stats.weaponData.aimSpeed);
        }
        else
        {
            spreadConstant = Mathf.Lerp(spreadConstant, gun.stats.weaponData.weaponType == QuantityStatsHUD.WeaponType.Shotgun ? 1.2f : 1f, gun.stats.weaponData.aimSpeed);
        }
    }
    void AimingLogic()
    {
        if ((Input.GetButton("Fire2") || (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.RightBracket))) && !gun.stats.isReloading && !gun.player.stats.isSliding) gun.stats.isAiming = true;
        else gun.stats.isAiming = false;
    }
    void GunMovementLogic()
    {
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) gun.stats.isWalking = true;
        else gun.stats.isWalking = false;

        if (Input.GetKey("left shift") && !gun.stats.isAiming && !gun.player.stats.isCrouching && gun.stats.isWalking) gun.stats.isSprinting = true;
        else gun.stats.isSprinting = false;
    }
    void AttachmentLogic()
    {
        if (Input.GetKeyDown("h"))
        {
            if (gun.stats.isAttaching)
            {
                gun.stats.isAttaching = false;
            }
            else
            {
                gun.stats.isAttaching = true;
            }
        }
        if (gun.stats.isAiming) gun.stats.isAttaching = false;
    }
}
