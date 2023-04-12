using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWeaponHoriItem : MonoBehaviour
{
    [SerializeField] private GameObject Toggler;
    [SerializeField] private Text WeaponName;
    [SerializeField] private Text UnlockingLevel;
    [SerializeField] private Text PurchasePrice;
    [SerializeField] private Image WeaponIcon;
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
    }

    public void OnPress()
    {
        shop.SetPreviewInfo(CachedWeaponData);
        shop.TogglePreviewUI(true);
        ToggleSelection(true);
    }

    public void ToggleSelection(bool state)
    {
        Toggler.SetActive(state);
    }
}
