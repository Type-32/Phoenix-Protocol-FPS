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
    public List<WeaponData> slotWeaponData = new();
    public List<EquipmentData> slotEquipmentData = new();
    void Awake()
    {
        slotWeaponData.Clear();
        slotEquipmentData.Clear();
        slotWeaponData.Add(GlobalDatabase.Instance.allWeaponDatas[(int)photonView.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(1)]]);
        slotWeaponData.Add(GlobalDatabase.Instance.allWeaponDatas[(int)photonView.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(2)]]);
        slotEquipmentData.Add(GlobalDatabase.Instance.allEquipmentDatas[(int)photonView.Owner.CustomProperties[LoadoutKeys.SelectedEquipmentIndex(1)]]);
        slotEquipmentData.Add(GlobalDatabase.Instance.allEquipmentDatas[(int)photonView.Owner.CustomProperties[LoadoutKeys.SelectedEquipmentIndex(2)]]);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!playerManager.pv.IsMine && targetPlayer == playerManager.pv.Owner)
        {
            if (changedProps.ContainsKey(SynchronizationKeys.WeaponDataChangedMode) &&
                changedProps.ContainsKey(SynchronizationKeys.WeaponDataChanged))
            {
                if ((int)changedProps[SynchronizationKeys.WeaponDataChangedMode] < 2)
                {
                    slotWeaponData[(int)changedProps[SynchronizationKeys.WeaponDataChangedMode]] =
                        GlobalDatabase.Instance.allWeaponDatas
                            [(int)changedProps[SynchronizationKeys.WeaponDataChanged]];
                }
                else
                {
                    slotEquipmentData[(int)changedProps[SynchronizationKeys.WeaponDataChangedMode]] =
                        GlobalDatabase.Instance.allEquipmentDatas[
                            (int)changedProps[SynchronizationKeys.WeaponDataChanged]];
                }
            }
            else if (changedProps.ContainsKey(LoadoutKeys.SelectedWeaponIndex(1)) ||
                     changedProps.ContainsKey(LoadoutKeys.SelectedWeaponIndex(2)) ||
                     changedProps.ContainsKey(LoadoutKeys.SelectedEquipmentIndex(1)) ||
                     changedProps.ContainsKey(LoadoutKeys.SelectedEquipmentIndex(2)))
            {
                slotWeaponData[0] =
                    GlobalDatabase.Instance.allWeaponDatas[(int)changedProps[LoadoutKeys.SelectedWeaponIndex(1)]];
                slotWeaponData[1] =
                    GlobalDatabase.Instance.allWeaponDatas[(int)changedProps[LoadoutKeys.SelectedWeaponIndex(2)]];
                slotEquipmentData[0] =
                    GlobalDatabase.Instance.allEquipmentDatas[(int)changedProps[LoadoutKeys.SelectedEquipmentIndex(1)]];
                slotEquipmentData[1] =
                    GlobalDatabase.Instance.allEquipmentDatas[(int)changedProps[LoadoutKeys.SelectedEquipmentIndex(2)]];
            }
        }
        else if (playerManager.pv.IsMine && targetPlayer == playerManager.pv.Owner)
        {
            if (changedProps.ContainsKey(SynchronizationKeys.WeaponDataChangedMode) &&
                changedProps.ContainsKey(SynchronizationKeys.WeaponDataChanged))
            {
                if ((int)changedProps[SynchronizationKeys.WeaponDataChangedMode] < 2)
                {
                    slotWeaponData[(int)changedProps[SynchronizationKeys.WeaponDataChangedMode]] =
                        GlobalDatabase.Instance.allWeaponDatas
                            [(int)changedProps[SynchronizationKeys.WeaponDataChanged]];
                }
                else
                {
                    slotEquipmentData[(int)changedProps[SynchronizationKeys.WeaponDataChangedMode]] =
                        GlobalDatabase.Instance.allEquipmentDatas[
                            (int)changedProps[SynchronizationKeys.WeaponDataChanged]];
                }
            }
            else if (changedProps.ContainsKey(LoadoutKeys.SelectedWeaponIndex(1)) ||
                     changedProps.ContainsKey(LoadoutKeys.SelectedWeaponIndex(2)) ||
                     changedProps.ContainsKey(LoadoutKeys.SelectedEquipmentIndex(1)) ||
                     changedProps.ContainsKey(LoadoutKeys.SelectedEquipmentIndex(2)))
            {
                slotWeaponData.Clear();
                slotEquipmentData.Clear();
                slotWeaponData.Add(GlobalDatabase.Instance.allWeaponDatas[(int)photonView.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(1)]]);
                slotWeaponData.Add(GlobalDatabase.Instance.allWeaponDatas[(int)photonView.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(2)]]);
                slotEquipmentData.Add(GlobalDatabase.Instance.allEquipmentDatas[(int)photonView.Owner.CustomProperties[LoadoutKeys.SelectedEquipmentIndex(1)]]);
                slotEquipmentData.Add(GlobalDatabase.Instance.allEquipmentDatas[(int)photonView.Owner.CustomProperties[LoadoutKeys.SelectedEquipmentIndex(2)]]);
            }
        }
    }

}
