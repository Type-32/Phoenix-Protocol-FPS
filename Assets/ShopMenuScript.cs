using PrototypeLib.Modules.FileOperations.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UserConfiguration;

public class ShopMenuScript : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject weaponsMenu;
    public GameObject cosmeticsMenu;
    public GameObject lootCratesMenu;

    [Space, Header("Weapons Menu Refs")]
    public GameObject shopWeaponItemPrefab;
    public Transform shopWeaponItemHolder;

    [Space, Header("Preview UI Refs")]
    public GameObject previewUI;
    [SerializeField] private GameObject previewAttachItemPrefab;
    [SerializeField] private Transform previewAttachItemHolder;
    [SerializeField] private Image previewIcon;
    [SerializeField] private Text previewWeaponName;
    [SerializeField] private Text previewWeaponPrice;
    [SerializeField] private LoadoutWeaponStatisticsDisplay previewStats;
    [SerializeField] List<LAPreview> attachList = new();
    [SerializeField] List<ShopWeaponHoriItem> shopWeaponList = new();
    [HideInInspector] public WeaponData CurrentPreviewWeaponData;

    [Space, Header("Weapon Menu")]

    [Space, Header("Cosmetics Menu")]
    [SerializeField] GameObject weaponCosmeticListItem;
    [SerializeField] Transform weaponCosmeticListItemHolder;
    [SerializeField] Dictionary<WeaponData, WeaponCosmeticListItem> weaponCosmeticList = new();

    void Start()
    {
        InitializeWeaponsMenu();
        //InitializeCosmeticsMenu();
        weaponsMenu.SetActive(true);
        //cosmeticsMenu.SetActive(false);
        TogglePreviewUI(false);
        //MenuManager.Instance.ToggleShopMenu(false);
    }

    private void InitializeCosmeticsMenu()
    {
        AppearancesDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).AppearancesData;
        for (int i = 0; i < jsonData.availableWeaponAppearances.Count; i++)
        {
            var apdata = CosmeticSystem.FindWeaponAppearanceMeshData(jsonData.availableWeaponAppearances[i]);
            WeaponCosmeticListItem temp = Instantiate(weaponCosmeticListItem, weaponCosmeticListItemHolder).GetComponent<WeaponCosmeticListItem>();
            weaponCosmeticList.Add(apdata.weaponData, temp);
            temp.SetInfo(apdata, this);
            temp.gameObject.SetActive(false);
        }
    }
    private void InitializeWeaponsMenu()
    {
        UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        bool informPopupNeeded = false;
        string content = "You have unlocked the following content(s) in your last session: \n";
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponDatas.Count; i++)
        {
            ShopWeaponHoriItem item = Instantiate(shopWeaponItemPrefab, shopWeaponItemHolder).GetComponent<ShopWeaponHoriItem>();
            item.ToggleSelection(false);
            if (jsonData.ShopData.availableWeaponIndexes.Contains(i) || jsonData.ShopData.unlockedWeaponIndexes.Contains(i) || jsonData.ShopData.ownedWeaponIndexes.Contains(i))
            {
                if (jsonData.ShopData.availableWeaponIndexes.Contains(i))
                {
                    if (jsonData.userLevel >= GlobalDatabase.Instance.allWeaponDatas[i].unlockingLevel)
                    {
                        informPopupNeeded = true;
                        jsonData.ShopData.availableWeaponIndexes.Remove(i);
                        jsonData.ShopData.unlockedWeaponIndexes.Add(i);
                        content = content + GlobalDatabase.Instance.allWeaponDatas[i].itemName + "\n";
                    }
                }
                item.SetInfo(GlobalDatabase.Instance.allWeaponDatas[i], this);
                shopWeaponList.Add(item);
            }
            else
            {
                if (jsonData.userLevel >= GlobalDatabase.Instance.allWeaponDatas[i].unlockingLevel)
                {
                    informPopupNeeded = true;
                    jsonData.ShopData.unlockedWeaponIndexes.Add(i);
                    content = content + GlobalDatabase.Instance.allWeaponDatas[i].itemName + "\n";
                }
                else
                {
                    jsonData.ShopData.availableWeaponIndexes.Add(i);
                }
                item.SetInfo(GlobalDatabase.Instance.allWeaponDatas[i], this);
                shopWeaponList.Add(item);
            }
        }
        FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
        if (informPopupNeeded) MenuManager.Instance.AddModalWindow("Unlocking Content", content);
    }
    public void SetPreviewInfo(WeaponData data)
    {
        foreach (ShopWeaponHoriItem i in shopWeaponList)
        {
            if (i.CachedWeaponData == data) continue;
            i.ToggleSelection(false);
        }
        CurrentPreviewWeaponData = data;
        previewIcon.sprite = data.itemIcon;
        previewWeaponName.text = data.itemName;
        previewWeaponPrice.text = $"${data.purchasePrice}";
        previewStats.SetInfo(data.damage, data.range, data.maxAmmoPerMag);
        if (attachList.Count != 0)
        {
            for (int i = 0; i < attachList.Count; i++)
            {
                Destroy(attachList[i].gameObject);
            }
            attachList.Clear();
        }
        for (int i = 0; i < data.applicableAttachments.Count; i++)
        {
            LAPreview item = Instantiate(previewAttachItemPrefab, previewAttachItemHolder).GetComponent<LAPreview>();
            attachList.Add(item);
            item.SetInfo(data.applicableAttachments[i]);
        }
    }
    public void TogglePreviewUI(bool value)
    {
        previewUI.SetActive(value);
    }
    public void OnClickPurchaseButton()
    {
        foreach (ShopWeaponHoriItem i in shopWeaponList)
        {
            i.ToggleSelection(false);
        }
        PurchaseWeapon(CurrentPreviewWeaponData);
        TogglePreviewUI(false);
    }
    public void PurchaseWeapon(WeaponData data)
    {
        if (data != null)
        {
            UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
            if (jsonData.userCoins >= data.purchasePrice && jsonData.userLevel >= data.unlockingLevel)
            {
                if (jsonData.ShopData.ownedWeaponIndexes.Contains(data.GlobalWeaponIndex))
                {
                    MenuManager.Instance.AddModalWindow("Error", "You have already purchased the weapon " + data.itemName + ".");
                    return;
                }
                MenuManager.Instance.AddModalWindow("Purchase Result", "You have purchased the weapon " + data.itemName + " successfully!\nYou can equip this weapon in your loadouts now.");
                //FindForWeaponDataInList(data).SetInfo(data, true, true, this);
                UserDatabase.Instance.AddUserCurrency(data.purchasePrice);
                jsonData.ShopData.ownedWeaponIndexes.Add(Database.FindWeaponDataIndex(data));
                FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
                MenuManager.Instance.loadoutSelectionMenu.GetComponent<LoadoutSelectionScript>().InstantiateLoadoutItemSelections();
            }
            else if (jsonData.userCoins < data.purchasePrice && jsonData.userLevel >= data.unlockingLevel)
            {
                MenuManager.Instance.AddModalWindow("Error", "Cannot Purchase Weapon! You need more money!");
                TogglePreviewUI(false);
            }
            else if (jsonData.userCoins >= data.purchasePrice && jsonData.userLevel < data.unlockingLevel)
            {
                MenuManager.Instance.AddModalWindow("Error", "Cannot Purchase Weapon! You need more levels!");
                TogglePreviewUI(false);
            }
            else
            {
                MenuManager.Instance.AddNotification("Error", "Cannot Purchase Weapon! You need more money and levels!");
                TogglePreviewUI(false);
            }
        }
    }
    public ShopWeaponHoriItem FindForWeaponDataInList(WeaponData data)
    {
        for (int i = 0; i < shopWeaponList.Count; i++)
        {
            if (shopWeaponList[i].CachedWeaponData == data)
            {
                return shopWeaponList[i];
            }
        }
        return new();
    }
}
