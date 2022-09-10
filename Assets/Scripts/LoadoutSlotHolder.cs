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
    public WeaponData[] slotWeaponData;
    public Image[] slotIcons;
    public Text[] slotNames;
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!playerManager.pv.IsMine && targetPlayer == playerManager.pv.Owner && (changedProps.ContainsKey("weaponDataChangedMode") && changedProps.ContainsKey("weaponDataChanged")))
        {
            playerManager.slotHolderScript.slotWeaponData[(int)changedProps["weaponDataChangedMode"]] = GlobalDatabase.singleton.allWeaponDatas[(int)changedProps["weaponDataChanged"]];
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
