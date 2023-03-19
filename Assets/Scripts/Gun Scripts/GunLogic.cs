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
        //Using Lambda Expression to single-fy the code
        spreadConstant = gun.stats.isAiming ? Mathf.Lerp(spreadConstant, gun.stats.weaponData.weaponType == QuantityStatsHUD.WeaponType.Shotgun ? 0.3f : 0f, gun.stats.weaponData.aimSpeed) : Mathf.Lerp(spreadConstant, gun.stats.weaponData.weaponType == QuantityStatsHUD.WeaponType.Shotgun ? 1.1f : gun.stats.weaponData.weaponType == QuantityStatsHUD.WeaponType.Pistol ? 0.25f : 0.75f, gun.stats.weaponData.aimSpeed);
    }
    void AimingLogic()
    {
        gun.stats.isAiming = ((Input.GetButton("Fire2") || (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.RightBracket))) && !gun.player.stats.isSliding) ? true : false;
    }
    void GunMovementLogic()
    {
        gun.stats.isWalking = gun.player.stats.isWalking;
        gun.stats.isSprinting = (Input.GetKey("left shift") && !gun.stats.isAiming && !gun.player.stats.isCrouching && gun.stats.isWalking) ? true : false;
    }
    void AttachmentLogic()
    {
        if (Input.GetKeyDown("h"))
            gun.stats.isAttaching = gun.stats.isAttaching ? false : true;
        if (gun.stats.isAiming) gun.stats.isAttaching = false;
    }
}
