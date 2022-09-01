using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSlotHolder : MonoBehaviour
{
    public WeaponData[] slotWeaponData;
    public Image[] slotIcons;
    public Text[] slotNames;
    
    public void SetLoadoutSlotInfo(WeaponData data, int index)
    {
        slotWeaponData[index] = data;
        slotIcons[index].sprite = data.itemIcon;
        slotNames[index].text = (index == 0 ? "PRIMARY - " : "SECONDARY - ") + data.itemName;
    }
    public void RefreshLoadoutSlotInfo()
    {
        for (int i = 0; i < slotWeaponData.Length; i++)
        {
            SetLoadoutSlotInfo(slotWeaponData[i], i);
        }
    }
}
