using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutPreviewUI : MonoBehaviour
{
    public Text[] texts;
    public Image[] images;

    public void SetWeaponSlotInfo(int index, WeaponData weaponData)
    {
        string temp = "";
        switch (index)
        {
            case 0:
                temp = "PRIMARY - ";
                break;
            case 1:
                temp = "SECONDARY - ";
                break;
        }
        texts[index <= 1 ? index : 0].text = temp + weaponData.itemName;
        images[index <= 1 ? index : 0].sprite = weaponData.itemIcon;
    }
    public void SetPreviewInfo(LoadoutData data)
    {
        SetWeaponSlotInfo(0, data.weaponData[0]);
        SetWeaponSlotInfo(1, data.weaponData[1]);
    }
}
