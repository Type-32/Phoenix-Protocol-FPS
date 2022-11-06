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
    public Slider previewDamage;
    public Slider previewFireRate;
    public Slider previewAmmo;
    public Slider previewRange;

    [Space]
    [SerializeField] WeaponData previewingWeaponData;
    [SerializeField] List<ShopAttachPreviewItem> attachList = new();

    void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/UserDataConfig.json")) UserDatabase.Instance.InitializeUserDataToJSON();
        string json = File.ReadAllText(Application.persistentDataPath + "/UserDataConfig.json");
        Debug.LogWarning("Reading User Data To Files...");
        UserDataJSON jsonData = UserDatabase.Instance.emptyUserDataJSON;
        jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        InitializeWeaponsMenu(jsonData);
        weaponsMenu.SetActive(false);
        cratesMenu.SetActive(false);
        TogglePreviewUI(false);
    }
    public void InitializeWeaponsMenu(UserDataJSON jsonData)
    {
        for(int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            ShopWeaponItem item = Instantiate(shopWeaponItemPrefab, shopWeaponItemHolder).GetComponent<ShopWeaponItem>();
            bool un = false, pur = false;
            if (jsonData.shopData.availableWeaponIndexes.Contains(i))
            {
                un = pur = false;
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
        }
    }
    public void SetPreviewInfo(WeaponData data)
    {
        previewingWeaponData = data;
        previewIcon.sprite = data.itemIcon;
        previewWeaponName.text = data.itemName;
        previewDamage.value = data.damage;
        previewFireRate.value = data.fireRate;
        previewRange.value = data.range;
        previewAmmo.value = data.maxAmmoPerMag;
        if(attachList.Count != 0)
        {
            for(int i = 0; i < attachList.Count; i++)
            {
                Destroy(attachList[i].gameObject);
            }
            attachList.Clear();
        }
        for(int i = 0; i < data.applicableAttachments.Count; i++)
        {
            ShopAttachPreviewItem item = Instantiate(previewAttachItemPrefab, previewAttachItemHolder).GetComponent<ShopAttachPreviewItem>();
            attachList.Add(item);
            item.SetInfo(data.applicableAttachments[i].attachmentIcon);
        }
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
}
