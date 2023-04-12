using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWeaponHoriItem : MonoBehaviour
{
    [SerializeField] private Text WeaponName;
    [SerializeField] private Text UnlockingLevel;
    [SerializeField] private Text PurchasePrice;
    [SerializeField] private Image WeaponIcon;
    public Toggle ItemToggle;
    [HideInInspector] public ToggleGroup CachedToggleGroup;
    [HideInInspector] public ShopMenuScript shop;
    [HideInInspector] public WeaponData CachedWeaponData;
    
    public void SetInfo(WeaponData data, ShopMenuScript sms)
    {
        CachedWeaponData = data;
        shop = sms;
        WeaponName.text = data.itemName;
        UnlockingLevel.text = $"Unlock at Lv.{data.unlockingLevel}";
        PurchasePrice.text = $"${data.purchasePrice}";
        WeaponIcon.sprite = data.itemIcon;
        CachedToggleGroup = shop.weaponAvailableToggleGroup;
    }

    public void OnPress()
    {
        shop.SetPreviewInfo(CachedWeaponData);
    }
}
