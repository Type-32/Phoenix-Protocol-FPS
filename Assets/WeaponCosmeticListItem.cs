using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserConfiguration;
using LauncherManifest;
using PrototypeLib.Modules.FileOperations.IO;

public class WeaponCosmeticListItem : MonoBehaviour
{
    ShopMenuScript script;
    [SerializeField] Image weaponIcon;
    [SerializeField] Text weaponSkinText;
    [SerializeField] Text weaponSkinPrice;
    [SerializeField] Text rarityText;
    [SerializeField] Color common, uncommon, rare, epic, legendary;
    [HideInInspector] public WeaponAppearanceMeshData data;
    public void SetInfo(WeaponAppearanceMeshData data, ShopMenuScript sms)
    {
        script = sms;
        weaponIcon.sprite = data.itemIcon;
        weaponSkinText.text = data.weaponData.itemName + " - " + data.itemName;
        weaponSkinPrice.text = $"${data.purchasePrice}";
        rarityText.text = data.rarity.ToString();
        switch (data.rarity)
        {
            case Rarity.Common:
                rarityText.color = common;
                break;
            case Rarity.Uncommon:
                rarityText.color = uncommon;
                break;
            case Rarity.Rare:
                rarityText.color = rare;
                break;
            case Rarity.Epic:
                rarityText.color = epic;
                break;
            case Rarity.Legendary:
                rarityText.color = legendary;
                break;
        }
        this.data = data;
    }
    public void PurchaseItem()
    {
        UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        WeaponAppearance temp = new WeaponAppearance(data.weaponData.GlobalWeaponIndex, data.WeaponAppearanceMeshDataIndex);
        if (data.purchasePrice <= jsonData.userCoins && (!jsonData.AppearancesData.unlockedWeaponAppearances.Contains(temp) && jsonData.AppearancesData.availableWeaponAppearances.Contains(temp)) && jsonData.ShopData.ownedWeaponIndexes.Contains(data.weaponData.GlobalWeaponIndex))
        {
            MenuManager.Instance.AddNotification("Success Purchase", "You have successfully purchased the " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + "!");
            UserDatabase.Instance.AddUserCurrency(data.purchasePrice);
            jsonData.AppearancesData.unlockedWeaponAppearances.Add(temp);
            jsonData.AppearancesData.availableWeaponAppearances.Remove(temp);

            FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
            //script.RemoveWeaponCosmeticListItem(this);
        }
        else
        {
            if (!jsonData.ShopData.ownedWeaponIndexes.Contains(data.weaponData.GlobalWeaponIndex)) MenuManager.Instance.AddNotification("Failed Purchase", "You do not have the weapon " + data.weaponData.itemName + " to obtain this weapon skin.");
            else if (data.purchasePrice > jsonData.userCoins) MenuManager.Instance.AddNotification("Failed Purchase", "You do not have enough money to purchase " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + ".");
            else MenuManager.Instance.AddNotification("Failed Purchase", "An error occured while trying to purchase " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + ".");
        }
    }
}
