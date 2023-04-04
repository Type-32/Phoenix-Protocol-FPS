using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour
{
    public GunManager gun;
    public UIManager ui;
    private QuantityStatsHUD.AmmoHUDStats temp;
    // Start is called before the first frame update
    public void GunUIAwake()
    {
        if (gun.ModelMode) return;
        PlayerControllerManager playerTemp = GetComponentInParent<PlayerControllerManager>();
        ui = playerTemp.GetComponentInChildren<UIManager>();
    }

    // Update is called once per frame
    /*
    void Update()
    {
    }*/
    public void UIFunctions()
    {
        if (gun.ModelMode) return;
        temp.currentAmmo = gun.stats.ammo;
        temp.ammoPool = gun.stats.ammoPool;
        temp.firemode = gun.stats.fireMode;
        temp.weaponIcon = gun.stats.weaponData.itemIcon;
        temp.eqCount1 = gun.player.holder.equipmentSlots[0].equipment.stats.count;
        temp.eqCount2 = gun.player.holder.equipmentSlots[1].equipment.stats.count;
        temp.eqIcon1 = gun.player.holder.equipmentSlots[0].equipment.stats.equipmentData.itemIcon;
        temp.eqIcon2 = gun.player.holder.equipmentSlots[1].equipment.stats.equipmentData.itemIcon;
        ui.quantityHUD.SetAmmoHUDStats(temp, gun.stats.isReloading, gun.stats.weaponData.reloadTime, true);

        if (gun.stats.isAiming)
        {
            gun.ui.ui.crosshair.SetActive(false);
        }
        else
        {
            gun.ui.ui.crosshair.SetActive(true);
        }
    }
}
