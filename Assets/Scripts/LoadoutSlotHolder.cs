using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class LoadoutSlotHolder : MonoBehaviourPunCallbacks
{
    [SerializeField] LoadoutMenu loadoutMenu;
    [SerializeField] PlayerManager playerManager;
    public WeaponData[] slotWeaponData = new WeaponData[2];
    public EquipmentData[] slotEquipmentData = new EquipmentData[2];
    public Image[] slotIcons;
    public Text[] slotNames;
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!playerManager.pv.IsMine && targetPlayer == playerManager.pv.Owner)
        {
            if (changedProps.ContainsKey("weaponDataChangedMode") && changedProps.ContainsKey("weaponDataChanged"))
            {
                if ((int)changedProps["weaponDataChangedMode"] < 2)
                {
                    playerManager.slotHolderScript.slotWeaponData[(int)changedProps["weaponDataChangedMode"]] = GlobalDatabase.singleton.allWeaponDatas[(int)changedProps["weaponDataChanged"]];
                }
                else
                {
                    playerManager.slotHolderScript.slotEquipmentData[(int)changedProps["weaponDataChangedMode"]] = GlobalDatabase.singleton.allEquipmentDatas[(int)changedProps["weaponDataChanged"]];
                }
            }
            else if (changedProps.ContainsKey("selectedMainWeaponIndex") || changedProps.ContainsKey("selectedSecondWeaponIndex") || changedProps.ContainsKey("selectedEquipmentIndex1") || changedProps.ContainsKey("selectedEquipmentIndex2"))
            {
                slotWeaponData[0] = GlobalDatabase.singleton.allWeaponDatas[(int)changedProps["selectedMainWeaponIndex"]];
                slotWeaponData[1] = GlobalDatabase.singleton.allWeaponDatas[(int)changedProps["selectedSecondWeaponIndex"]];
                slotEquipmentData[0] = GlobalDatabase.singleton.allEquipmentDatas[(int)changedProps["selectedEquipmentIndex1"]];
                slotEquipmentData[1] = GlobalDatabase.singleton.allEquipmentDatas[(int)changedProps["selectedEquipmentIndex2"]];
            }
        }
    }

    public void SetLoadoutSlotInfo(WeaponData data, int index)
    {
        slotWeaponData[index] = data;
        slotIcons[index].sprite = data.itemIcon;
        slotNames[index].text = (index == 0 ? "PRIMARY - " : "SECONDARY - ") + data.itemName;
    }
    public void RefreshLoadoutSlotInfo()
    {
        for (int i = 0; i < slotWeaponData.Length; i++)
        {
            SetLoadoutSlotInfo(slotWeaponData[i], i);
        }
    }

}
