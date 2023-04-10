using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;

public class MatchLoadoutManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerManager playerManager;
    public WeaponData[] slotWeaponData = new WeaponData[2];
    public EquipmentData[] slotEquipmentData = new EquipmentData[2];
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!playerManager.pv.IsMine && targetPlayer == playerManager.pv.Owner)
        {
            if (changedProps.ContainsKey("weaponDataChangedMode") && changedProps.ContainsKey("weaponDataChanged"))
            {
                if ((int)changedProps["weaponDataChangedMode"] < 2)
                {
                    playerManager.matchLoadoutManager.slotWeaponData[(int)changedProps["weaponDataChangedMode"]] = GlobalDatabase.Instance.allWeaponDatas[(int)changedProps["weaponDataChanged"]];
                }
                else
                {
                    playerManager.matchLoadoutManager.slotEquipmentData[(int)changedProps["weaponDataChangedMode"]] = GlobalDatabase.Instance.allEquipmentDatas[(int)changedProps["weaponDataChanged"]];
                }
            }
            else if (changedProps.ContainsKey(LoadoutKeys.SelectedWeaponIndex(1)) || changedProps.ContainsKey(LoadoutKeys.SelectedWeaponIndex(2)) || changedProps.ContainsKey(LoadoutKeys.SelectedEquipmentIndex(1)) || changedProps.ContainsKey(LoadoutKeys.SelectedEquipmentIndex(2)))
            {
                slotWeaponData[0] = GlobalDatabase.Instance.allWeaponDatas[(int)changedProps[LoadoutKeys.SelectedWeaponIndex(1)]];
                slotWeaponData[1] = GlobalDatabase.Instance.allWeaponDatas[(int)changedProps[LoadoutKeys.SelectedWeaponIndex(2)]];
                slotEquipmentData[0] = GlobalDatabase.Instance.allEquipmentDatas[(int)changedProps[LoadoutKeys.SelectedEquipmentIndex(1)]];
                slotEquipmentData[1] = GlobalDatabase.Instance.allEquipmentDatas[(int)changedProps[LoadoutKeys.SelectedEquipmentIndex(2)]];
            }
        }
    }

}
