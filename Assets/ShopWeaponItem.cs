using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWeaponItem : MonoBehaviour
{
    ShopMenuScript script;
    public WeaponData weaponData;
    public Text weaponName;
    public Text weaponState;
    public Image weaponIcon;
    [SerializeField] Button weaponButton;
    public bool isUnlocked = false;
    public bool isPurchased = false;
    // Start is called before the first frame update
    public void SetItemData(WeaponData data, bool unlocked, bool purchased, ShopMenuScript sms)
    {
        script = sms;
        weaponData = data;
        weaponName.text = weaponData.name;
        weaponIcon.sprite = weaponData.itemIcon;
        if (unlocked)
        {
            weaponButton.interactable = true;
            isUnlocked = true;
            weaponState.text = "Unlocked";
            if (purchased)
            {
                weaponState.text = "Purchased";
                isPurchased = true;
            }
        }
        else
        {
            isUnlocked = false;
            weaponButton.interactable = false;
            weaponState.text = "Unlock at Level " + weaponData.unlockingLevel.ToString();
        }
    }
    public void SelectItem()
    {
        script.SetPreviewInfo(weaponData);
        script.TogglePreviewUI(true);
    }
}
