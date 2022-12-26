using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserConfiguration;
using LauncherManifest;

public class WeaponCosmeticListItem : MonoBehaviour
{
    ShopMenuScript script;
    [SerializeField] Image weaponIcon;
    [SerializeField] Text weaponSkinText;
    [SerializeField] Text weaponSkinPrice;
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
        this.data = data;
    }
    public void PurchaseItem()
    {
        UserDataJSON jsonData = UserSystem.UserJsonData;
        AppearancesDataJSON app = CosmeticSystem.AppearancesJsonData;
        AppearancesDataJSON.WeaponAppearance temp = CosmeticSystem.RevertWeaponAppearanceMeshData(data);
        if (data.purchasePrice <= jsonData.userCoins && (!app.unlockedWeaponAppearances.Contains(temp) && app.availableWeaponAppearances.Contains(temp)) && jsonData.shopData.ownedWeaponIndexes.Contains(data.weaponData.GlobalWeaponIndex))
        {
            MenuManager.instance.AddNotification("Success Purchase", "You have successfully purchased the " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + "!");
            jsonData.userCoins -= data.purchasePrice;
            app.unlockedWeaponAppearances.Add(temp);
            app.availableWeaponAppearances.Remove(temp);
            CosmeticSystem.WriteToConfig(app);
            UserSystem.WriteToUserConfig(jsonData);
            MenuManager.instance.UpdateCoin(jsonData.userCoins);
            script.RemoveWeaponCosmeticListItem(this);
        }
        else
        {
            if (!jsonData.shopData.ownedWeaponIndexes.Contains(data.weaponData.GlobalWeaponIndex)) MenuManager.instance.AddNotification("Failed Purchase", "You do not have the weapon " + data.weaponData.itemName + " to obtain this weapon skin.");
            else if (data.purchasePrice > jsonData.userCoins) MenuManager.instance.AddNotification("Failed Purchase", "You do not have enough money to purchase " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + ".");
            else MenuManager.instance.AddNotification("Failed Purchase", "An error occured while trying to purchase " + data.itemName + " Weapon Skin for " + data.weaponData.itemName + ".");
        }
    }
}
