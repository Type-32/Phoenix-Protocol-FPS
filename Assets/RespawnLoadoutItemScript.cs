using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;
using Unity.Services.Lobbies.Models;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RespawnLoadoutItemScript : MonoBehaviour
{
    private PlayerManager player;
    private LoadoutSlotDataJSON localLoadoutDataSlot;
    [SerializeField] Text loadoutTitle, primaryText, secondaryText, eqText1, eqText2;
    [SerializeField] Image primaryImage, secondaryImage, eqImage1, eqImage2;

    public void SetInfo(LoadoutSlotDataJSON lsd, PlayerManager pm)
    {
        player = pm;
        localLoadoutDataSlot = lsd;
        loadoutTitle.text = lsd.SlotName;
        primaryText.text = GlobalDatabase.Instance.allWeaponDatas[lsd.Weapon1].itemName;
        secondaryText.text = GlobalDatabase.Instance.allWeaponDatas[lsd.Weapon2].itemName;
        primaryImage.sprite = GlobalDatabase.Instance.allWeaponDatas[lsd.Weapon1].itemIcon;
        secondaryImage.sprite = GlobalDatabase.Instance.allWeaponDatas[lsd.Weapon2].itemIcon;
        eqText1.text = GlobalDatabase.Instance.allEquipmentDatas[lsd.Equipment1].itemName;
        eqText2.text = GlobalDatabase.Instance.allEquipmentDatas[lsd.Equipment2].itemName;
        eqImage1.sprite = GlobalDatabase.Instance.allEquipmentDatas[lsd.Equipment1].itemIcon;
        eqImage2.sprite = GlobalDatabase.Instance.allEquipmentDatas[lsd.Equipment2].itemIcon;
    }

    public void OnClickEquip()
    {
        Hashtable temp = new Hashtable();
        temp.Add(LoadoutKeys.SelectedWeaponIndex(1), localLoadoutDataSlot.Weapon1);
        temp.Add(LoadoutKeys.SelectedWeaponIndex(2), localLoadoutDataSlot.Weapon2);
        temp.Add(LoadoutKeys.SelectedEquipmentIndex(1), localLoadoutDataSlot.Equipment1);
        temp.Add(LoadoutKeys.SelectedEquipmentIndex(2), localLoadoutDataSlot.Equipment2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Sight, 1), localLoadoutDataSlot.WA_Sight1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Sight, 2), localLoadoutDataSlot.WA_Sight2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 1), localLoadoutDataSlot.WA_Barrel1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 2), localLoadoutDataSlot.WA_Barrel2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Underbarrel, 1), localLoadoutDataSlot.WA_Underbarrel1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Underbarrel, 2), localLoadoutDataSlot.WA_Underbarrel2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Leftbarrel, 1), localLoadoutDataSlot.WA_Leftbarrel1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Leftbarrel, 2), localLoadoutDataSlot.WA_Leftbarrel2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Rightbarrel, 1), localLoadoutDataSlot.WA_Rightbarrel1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Rightbarrel, 2), localLoadoutDataSlot.WA_Rightbarrel2);
        temp.Add(LoadoutKeys.SelectedWeaponAppearance(1), localLoadoutDataSlot.WeaponSkin1);
        temp.Add(LoadoutKeys.SelectedWeaponAppearance(2), localLoadoutDataSlot.WeaponSkin2);

        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
        
        player.SetWeaponPreview(GlobalDatabase.Instance.allWeaponDatas[localLoadoutDataSlot.Weapon1].itemIcon, GlobalDatabase.Instance.allWeaponDatas[localLoadoutDataSlot.Weapon2].itemIcon, GlobalDatabase.Instance.allEquipmentDatas[localLoadoutDataSlot.Equipment1].itemIcon,GlobalDatabase.Instance.allEquipmentDatas[localLoadoutDataSlot.Equipment2].itemIcon);
        player.ToggleLoadoutSwapMenu(false);
    }
}
