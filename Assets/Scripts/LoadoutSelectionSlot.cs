using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelectionSlot : MonoBehaviour
{
    public LoadoutMenu loadoutMenu;
    public Text weaponName;
    public Text weaponType;
    public Image weaponIcon;
    public WeaponData weaponData;
    public int mode = 0;
    private void Awake()
    {
        loadoutMenu = FindObjectOfType<LoadoutMenu>();
    }
    public void SetWeaponSelectionInfo(WeaponData data, int index)
    {
        weaponName.text = data.itemName;
        weaponType.text = data.weaponType.ToString();
        weaponIcon.sprite = data.itemIcon;
        weaponData = data;
        mode = index;
    }
    public void SelectWeaponChoice()
    {
        loadoutMenu.slotHolderScript.slotWeaponData[mode] = weaponData;
        loadoutMenu.slotHolderScript.RefreshLoadoutSlotInfo();
        loadoutMenu.CloseSelectionMenu();
    }
}
