using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuScript : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject weaponsMenu;
    public GameObject cratesMenu;

    [Space]
    [Header("Selections")]
    public GameObject weaponsMenuSelection;
    public GameObject cratesMenuSelection;

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

    [Space]
    [SerializeField] WeaponData previewingWeaponData;
    [SerializeField] List<ShopAttachPreviewItem> attachList = new();
    [SerializeField] List<ShopWeaponItem> shopWeaponList = new();

    void Start()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"))) UserDatabase.Instance.InitializeUserDataToJSON();
        if (File.Exists(Path.Combine(Application.persistentDataPath, "UserDataConfig.json")))
        {
            string tempJson = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
            if (string.IsNullOrEmpty(tempJson) || string.IsNullOrWhiteSpace(tempJson))
            {
                UserDatabase.Instance.InitializeUserDataToJSON();
            }
        }
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
        //Debug.LogWarning("Reading User Data To Files...");
        UserDataJSON jsonData = UserDatabase.Instance.emptyUserDataJSON;
        jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        InitializeWeaponsMenu(jsonData);
        weaponsMenu.SetActive(false);
        cratesMenu.SetActive(false);
        TogglePreviewUI(false);
        MainMenuUIManager.instance.ToggleShopMenu(false);
    }
    public void InitializeWeaponsMenu(UserDataJSON jsonData)
    {
        bool informPopupNeeded = false;
        string content = "You have unlocked the following content(s) in your last session: \n";
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            ShopWeaponItem item = Instantiate(shopWeaponItemPrefab, shopWeaponItemHolder).GetComponent<ShopWeaponItem>();
            bool un = false, pur = false;
            if (jsonData.shopData.availableWeaponIndexes.Contains(i) || jsonData.shopData.unlockedWeaponIndexes.Contains(i) || jsonData.shopData.ownedWeaponIndexes.Contains(i))
            {
                if (jsonData.shopData.availableWeaponIndexes.Contains(i))
                {
                    un = pur = false;
                    if (jsonData.userLevel >= GlobalDatabase.singleton.allWeaponDatas[i].unlockingLevel)
                    {
                        un = true;
                        pur = false;
                        informPopupNeeded = true;
                        jsonData.shopData.availableWeaponIndexes.Remove(i);
                        jsonData.shopData.unlockedWeaponIndexes.Add(i);
                        content = content + GlobalDatabase.singleton.allWeaponDatas[i].itemName + "\n";
                    }
                }
                if (jsonData.shopData.unlockedWeaponIndexes.Contains(i))
                {
                    un = true;
                    pur = false;
                }
                if (jsonData.shopData.ownedWeaponIndexes.Contains(i))
                {
                    un = pur = true;
                }
                item.SetItemData(GlobalDatabase.singleton.allWeaponDatas[i], un, pur);
                shopWeaponList.Add(item);
            }
            else
            {
                if (jsonData.userLevel >= GlobalDatabase.singleton.allWeaponDatas[i].unlockingLevel)
                {
                    un = true;
                    pur = false;
                    informPopupNeeded = true;
                    jsonData.shopData.unlockedWeaponIndexes.Add(i);
                    content = content + GlobalDatabase.singleton.allWeaponDatas[i].itemName + "\n";
                }
                else
                {
                    jsonData.shopData.availableWeaponIndexes.Add(i);
                }
                item.SetItemData(GlobalDatabase.singleton.allWeaponDatas[i], un, pur);
                shopWeaponList.Add(item);
            }
        }
        UserDatabase.Instance.WriteInputDataToJSON(jsonData);
        if (informPopupNeeded) MainMenuUIManager.instance.AddModalWindow("Unlocking Content", content);
    }
    public void SetPreviewInfo(WeaponData data, bool showPurchaseButton)
    {
        previewingWeaponData = data;
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
            item.SetInfo(data.applicableAttachments[i].attachmentIcon);
        }
        purchasePreview.interactable = showPurchaseButton;
    }
    public void ToggleWeaponsMenu(bool value)
    {
        weaponsMenu.SetActive(value);
        if (value)
        {
            cratesMenu.SetActive(false);
            cratesMenuSelection.SetActive(false);
        }
        else
        {
            TogglePreviewUI(false);
        }
    }
    public void ToggleCratesMenu(bool value)
    {
        cratesMenu.SetActive(value);
        if (value)
        {
            weaponsMenu.SetActive(false);
            weaponsMenuSelection.SetActive(false);
        }
    }
    public void TogglePreviewUI(bool value)
    {
        previewUI.SetActive(value);
    }
    public void OnClickPurchaseButton()
    {
        PurchaseWeapon(previewingWeaponData);
    }
    public void PurchaseWeapon(WeaponData data)
    {
        if (data != null)
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
            //Debug.LogWarning("Reading User Data To Files...");
            UserDataJSON jsonData = UserDatabase.Instance.emptyUserDataJSON;
            jsonData = JsonUtility.FromJson<UserDataJSON>(json);
            if (jsonData.userCoins >= data.purchasePrice)
            {
                MainMenuUIManager.instance.AddNotification("Purchase Result", "You have purchased the weapon " + data.itemName + " successfully!\nYou can equip this weapon in your loadouts now.");
                FindForWeaponDataInList(data).SetItemData(data, true, true);
                jsonData.userCoins -= data.purchasePrice;
                //jsonData.shopData.unlockedWeaponIndexes.Remove(Launcher.Instance.FindGlobalWeaponIndex(data));
                jsonData.shopData.ownedWeaponIndexes.Add(Launcher.Instance.FindGlobalWeaponIndex(data));
                purchasePreview.interactable = false;
                MainMenuUIManager.instance.UpdateCoin(jsonData.userCoins);

                UserDatabase.Instance.WriteInputDataToJSON(jsonData);
                MainMenuUIManager.instance.loadoutSelectionMenu.GetComponent<LoadoutSelectionScript>().InstantiateLoadoutItemSelections();
                //Debug.LogWarning("Writing User Data To Files...");
            }
            else
            {
                MainMenuUIManager.instance.AddNotification("Purchase Result", "Cannot Purchase Weapon! You need more money!");
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
}
