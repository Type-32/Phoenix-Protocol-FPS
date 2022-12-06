using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public EquipmentManager equipment;
    [HideInInspector] public UIManager ui;
    private QuantityStatsHUD.AmmoHUDStats temp;
    // Start is called before the first frame update
    public void EquipmentUIAwake()
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
        temp.currentAmmo = equipment.stats.count;
        temp.weaponIcon = equipment.stats.equipmentData.itemIcon;
        temp.eqCount1 = equipment.player.holder.equipmentSlots[0].equipment.stats.count;
        temp.eqCount2 = equipment.player.holder.equipmentSlots[1].equipment.stats.count;
        temp.eqIcon1 = equipment.player.holder.equipmentSlots[0].equipment.stats.equipmentData.itemIcon;
        temp.eqIcon2 = equipment.player.holder.equipmentSlots[1].equipment.stats.equipmentData.itemIcon;
        ui.quantityHUD.SetAmmoHUDStats(temp, false, equipment.stats.equipmentData.recoveryTime, false);
        equipment.ui.ui.crosshair.SetActive(true);
    }
}
