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
        temp.currentAmmo = gun.stats.ammo;
        temp.ammoPool = gun.stats.ammoPool;
        temp.firemode = gun.stats.fireMode;
        ui.quantityHUD.SetAmmoHUDStats(temp, true);

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
