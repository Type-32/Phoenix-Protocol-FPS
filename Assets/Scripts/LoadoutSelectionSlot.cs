using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoadoutSelectionSlot : MonoBehaviourPunCallbacks
{
    public LoadoutMenu loadoutMenu;
    public PlayerManager playerManager;
    public Text weaponName;
    public Text weaponType;
    public Image weaponIcon;
    public WeaponData weaponData;
    public int mode = 0;
    private void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        loadoutMenu = playerManager.loadoutMenu;
    }
    public void SetWeaponSelectionInfo(WeaponData data, int index)
    {
        weaponName.text = data.itemName;
        weaponType.text = data.weaponType.ToString();
        weaponIcon.sprite = data.itemIcon;
        weaponData = data;
        mode = index;
    }
    public void SettingWeaponData(int mode)
    {
        playerManager.slotHolderScript.slotWeaponData[mode] = weaponData; //Change on client side
    }
    public void SelectWeaponChoice()
    {
        playerManager.slotHolderScript.slotWeaponData[mode] = weaponData; //Change on client side
        if (playerManager.pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            for(int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allWeaponDatas[i] == weaponData) hash.Add("weaponDataChanged", i);
            }
            hash.Add("weaponDataChangedMode", mode);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash); //Setup Hashtable to send Changed data to server side
        }
        playerManager.slotHolderScript.RefreshLoadoutSlotInfo();
        loadoutMenu.CloseSelectionMenu();
    }
    /*
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedWeaponData)
    {
        if (!loadoutMenu.playerManager.pv.IsMine && targetPlayer == loadoutMenu.playerManager.pv.Owner)
        {//receive on server side
            SettingWeaponData((int)changedWeaponData["weaponDataMode"]);
        }
    }*/

}
