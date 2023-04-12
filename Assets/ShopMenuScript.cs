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

    [Space]
    [Header("Selections")]
    public GameObject weaponsMenuSelection;
    public GameObject cosmeticsMenuSelection;

    [Space, Header("Weapons Menu Refs")]
    public GameObject shopWeaponItemPrefab;
    public Transform shopWeaponItemHolder;
    public GameObject shopSelectedWeaponPreview;

    [Space, Header("Preview UI Refs")]
    public GameObject previewUI;
    public GameObject previewAttachItemPrefab;
    public Transform previewAttachItemHolder;
    public Image previewIcon;
    public Text previewWeaponName;
    public Text previewWeaponPrice;
    public Slider previewDamage;
    public Slider previewFireRate;
    public Slider previewAmmo;
    public Slider previewRange;
    public Button purchasePreview;

    [Space, Header("Weapon Menu")]
    [SerializeField] WeaponData previewingWeaponData;
    [SerializeField] List<ShopAttachPreviewItem> attachList = new();
    [SerializeField] List<ShopWeaponItem> shopWeaponList = new();

    [Space, Header("Cosmetics Menu")]
    [SerializeField] GameObject weaponCosmeticListItem;
    [SerializeField] Transform weaponCosmeticListItemHolder;
    [SerializeField] List<WeaponCosmeticListItem> weaponCosmeticList;

    void Start()
    {
<<<<<<< HEAD
        InitializeWeaponsMenu();
        //InitializeCosmeticsMenu();
        weaponsMenu.SetActive(true);
        //cosmeticsMenu.SetActive(false);
=======
        UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        InitializeWeaponsMenu(jsonData);
        InitializeCosmeticsMenu(jsonData.AppearancesData);
        weaponsMenu.SetActive(false);
        cosmeticsMenu.SetActive(false);
>>>>>>> parent of 962acc21 (Overhauling Shop Menu)
        TogglePreviewUI(false);
        MenuManager.Instance.ToggleShopMenu(false);
    }

    private void InitializeCosmeticsMenu(AppearancesDataJSON jsonData)
    {
<<<<<<< HEAD
        AppearancesDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).AppearancesData;
=======
        return;
>>>>>>> parent of 962acc21 (Overhauling Shop Menu)
        for (int i = 0; i < jsonData.availableWeaponAppearances.Count; i++)
        {
            WeaponCosmeticListItem temp = Instantiate(weaponCosmeticListItem, weaponCosmeticListItemHolder).GetComponent<WeaponCosmeticListItem>();
            weaponCosmeticList.Add(temp);
            temp.SetInfo(CosmeticSystem.FindWeaponAppearanceMeshData(jsonData.availableWeaponAppearances[i]), this);
        }
    }

    private void InitializeWeaponsMenu(UserDataJSON jsonData)
    {
        bool informPopupNeeded = false;
        string content = "You have unlocked the following content(s) in your last session: \n";
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponDatas.Count; i++)
        {
<<<<<<< HEAD
            ShopWeaponHoriItem item = Instantiate(shopWeaponItemPrefab, shopWeaponItemHolder).GetComponent<ShopWeaponHoriItem>();
            item.ToggleSelection(false);
=======
            ShopWeaponItem item = Instantiate(shopWeaponItemPrefab, shopWeaponItemHolder).GetComponent<ShopWeaponItem>();
            bool un = false, pur = false;
>>>>>>> parent of 962acc21 (Overhauling Shop Menu)
            if (jsonData.ShopData.availableWeaponIndexes.Contains(i) || jsonData.ShopData.unlockedWeaponIndexes.Contains(i) || jsonData.ShopData.ownedWeaponIndexes.Contains(i))
            {
                if (jsonData.ShopData.availableWeaponIndexes.Contains(i))
                {
                    un = pur = false;
                    if (jsonData.userLevel >= GlobalDatabase.Instance.allWeaponDatas[i].unlockingLevel)
                    {
                        un = true;
                        pur = false;
                        informPopupNeeded = true;
                        jsonData.ShopData.availableWeaponIndexes.Remove(i);
                        jsonData.ShopData.unlockedWeaponIndexes.Add(i);
                        content = content + GlobalDatabase.Instance.allWeaponDatas[i].itemName + "\n";
                    }
                }
                if (jsonData.ShopData.unlockedWeaponIndexes.Contains(i))
                {
                    un = true;
                    pur = false;
                }
                if (jsonData.ShopData.ownedWeaponIndexes.Contains(i))
                {
                    un = pur = true;
                }
                item.SetItemData(GlobalDatabase.Instance.allWeaponDatas[i], un, pur, this);
                shopWeaponList.Add(item);
            }
            else
            {
                if (jsonData.userLevel >= GlobalDatabase.Instance.allWeaponDatas[i].unlockingLevel)
                {
                    un = true;
                    pur = false;
                    informPopupNeeded = true;
                    jsonData.ShopData.unlockedWeaponIndexes.Add(i);
                    content = content + GlobalDatabase.Instance.allWeaponDatas[i].itemName + "\n";
                }
                else
                {
                    jsonData.ShopData.availableWeaponIndexes.Add(i);
                }
                item.SetItemData(GlobalDatabase.Instance.allWeaponDatas[i], un, pur, this);
                shopWeaponList.Add(item);
            }
        }
        FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
        if (informPopupNeeded) MenuManager.Instance.AddModalWindow("Unlocking Content", content);
    }
    public void SetPreviewInfo(WeaponData data, bool showPurchaseButton)
    {
<<<<<<< HEAD
        foreach (ShopWeaponHoriItem i in shopWeaponList)
        {
            if (i.CachedWeaponData == data) continue;
            i.ToggleSelection(false);
        }
        CurrentPreviewWeaponData = data;
=======
        previewingWeaponData = data;
>>>>>>> parent of 962acc21 (Overhauling Shop Menu)
        previewIcon.sprite = data.itemIcon;
        previewWeaponName.text = data.itemName;
        previewWeaponPrice.text = "$" + data.purchasePrice.ToString();
        previewDamage.value = data.damage;
        previewFireRate.value = data.fireRate;
        previewRange.value = data.range;
        previewAmmo.value = data.maxAmmoPerMag;
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
            ShopAttachPreviewItem item = Instantiate(previewAttachItemPrefab, previewAttachItemHolder).GetComponent<ShopAttachPreviewItem>();
            attachList.Add(item);
            item.SetInfo(data.applicableAttachments[i].attachmentIcon, this);
        }
        purchasePreview.interactable = showPurchaseButton;
    }
    public void ToggleWeaponsMenu(bool value)
    {
        weaponsMenu.SetActive(value);
        if (value)
        {
            cosmeticsMenu.SetActive(false);
            cosmeticsMenuSelection.SetActive(false);
            weaponsMenuSelection.SetActive(true);
        }
        else
        {
            TogglePreviewUI(false);
        }
    }
    public void ToggleCratesMenu(bool value)
    {
        cosmeticsMenu.SetActive(value);
        if (value)
        {
            weaponsMenu.SetActive(false);
            weaponsMenuSelection.SetActive(false);
            cosmeticsMenuSelection.SetActive(true);
        }
    }
    public void TogglePreviewUI(bool value)
    {
        previewUI.SetActive(value);
    }
    public void OnClickPurchaseButton()
    {
<<<<<<< HEAD
        foreach (ShopWeaponHoriItem i in shopWeaponList)
        {
            i.ToggleSelection(false);
        }
        PurchaseWeapon(CurrentPreviewWeaponData);
        TogglePreviewUI(false);
=======
        PurchaseWeapon(previewingWeaponData);
>>>>>>> parent of 962acc21 (Overhauling Shop Menu)
    }
    public void PurchaseWeapon(WeaponData data)
    {
        if (data != null)
        {
            UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
            if (jsonData.userCoins >= data.purchasePrice)
            {
<<<<<<< HEAD
                if (jsonData.ShopData.ownedWeaponIndexes.Contains(data.GlobalWeaponIndex))
                {
                    MenuManager.Instance.AddModalWindow("Error", "You have already purchased the weapon " + data.itemName + ".");
                    return;
                }
                MenuManager.Instance.AddModalWindow("Purchase Result", "You have purchased the weapon " + data.itemName + " successfully!\nYou can equip this weapon in your loadouts now.");
                //FindForWeaponDataInList(data).SetInfo(data, true, true, this);
=======
                MenuManager.Instance.AddNotification("Purchase Result", "You have purchased the weapon " + data.itemName + " successfully!\nYou can equip this weapon in your loadouts now.");
                FindForWeaponDataInList(data).SetItemData(data, true, true, this);
>>>>>>> parent of 962acc21 (Overhauling Shop Menu)
                UserDatabase.Instance.AddUserCurrency(data.purchasePrice);
                jsonData.ShopData.ownedWeaponIndexes.Add(Database.FindWeaponDataIndex(data));
                purchasePreview.interactable = false;

                FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
                MenuManager.Instance.loadoutSelectionMenu.GetComponent<LoadoutSelectionScript>().InstantiateLoadoutItemSelections();
            }
            else
            {
                MenuManager.Instance.AddNotification("Purchase Result", "Cannot Purchase Weapon! You need more money!");
            }
        }
    }
    public ShopWeaponItem FindForWeaponDataInList(WeaponData data)
    {
        for (int i = 0; i < shopWeaponList.Count; i++)
        {
            if (shopWeaponList[i].weaponData == data)
            {
                return shopWeaponList[i];
            }
        }
        return new();
    }
    public void RemoveWeaponCosmeticListItem(WeaponCosmeticListItem item)
    {
        if (weaponCosmeticList.Contains(item)) weaponCosmeticList.Remove(item);
        Destroy(item.gameObject);
    }
}
