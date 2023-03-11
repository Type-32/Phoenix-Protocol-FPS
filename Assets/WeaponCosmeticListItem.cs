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
    // Start is called before the first frame update
    void Awake()
    {
        script = FindObjectOfType<ShopMenuScript>();
    }
    public void SetInfo(WeaponAppearanceMeshData data)
    {
        weaponIcon.sprite = data.itemIcon;
        weaponSkinText.text = data.weaponData.itemName + " - " + data.itemName;
        weaponSkinPrice.text = "$" + data.purchasePrice.ToString();
        rarityText.text = data.rarity.ToString();
        switch (data.rarity)
        {
            case WeaponAppearanceMeshData.Rarity.Common:
                rarityText.color = common;
                break;
            case WeaponAppearanceMeshData.Rarity.Uncommon:
                rarityText.color = uncommon;
                break;
            case WeaponAppearanceMeshData.Rarity.Rare:
                rarityText.color = rare;
                break;
            case WeaponAppearanceMeshData.Rarity.Epic:
                rarityText.color = epic;
                break;
            case WeaponAppearanceMeshData.Rarity.Legendary:
                rarityText.color = legendary;
                break;
        }
        this.data = data;
    }
    public void PurchaseItem()
    {
        UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        AppearancesDataJSON app = FileOps<AppearancesDataJSON>.ReadFile(UserSystem.AppearancesConfigPath);
        WeaponAppearance temp = new WeaponAppearance(data);
        if (data.purchasePrice <= jsonData.userCoins && (!app.unlockedWeaponAppearances.Contains(temp) && app.availableWeaponAppearances.Contains(temp)) && jsonData.shopData.ownedWeaponIndexes.Contains(data.weaponData.GlobalWeaponIndex))
        {
            MenuManager.Instance.AddNotification("Success Purchase", "You have successfully purchased the " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + "!");
            UserDatabase.Instance.AddUserCurrency(data.purchasePrice);
            app.unlockedWeaponAppearances.Add(temp);
            app.availableWeaponAppearances.Remove(temp);

            FileOps<AppearancesDataJSON>.WriteFile(app, UserSystem.AppearancesConfigPath);
            FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
            script.RemoveWeaponCosmeticListItem(this);
        }
        else
        {
            if (!jsonData.shopData.ownedWeaponIndexes.Contains(data.weaponData.GlobalWeaponIndex)) MenuManager.Instance.AddNotification("Failed Purchase", "You do not have the weapon " + data.weaponData.itemName + " to obtain this weapon skin.");
            else if (data.purchasePrice > jsonData.userCoins) MenuManager.Instance.AddNotification("Failed Purchase", "You do not have enough money to purchase " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + ".");
            else MenuManager.Instance.AddNotification("Failed Purchase", "An error occured while trying to purchase " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + ".");
        }
    }
}
