using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoadoutDataJSON
{
    public int SelectedSlot = 0;
    public List<LoadoutSlotDataJSON> Slots;
    public LoadoutDataJSON()
    {
        SelectedSlot = 0;
        Slots = new();
        for(int i = 0; i < 8; i++)
        {
            Slots.Add(new LoadoutSlotDataJSON());
        }
    }
    public LoadoutDataJSON(int count)
    {
        SelectedSlot = 0;
        Slots = new();
        for (int i = 0; i < count; i++)
        {
            Slots.Add(new LoadoutSlotDataJSON());
        }
    }
}
[System.Serializable]
public class LoadoutSlotDataJSON
{
    public string SlotName;
    public bool EquippedByDefault;
    public int Weapon1;
    public int Weapon2;
    public int Equipment1;
    public int Equipment2;

    public int WA_Sight1;
    public int WA_Sight2;
    public int WA_Barrel1;
    public int WA_Barrel2;
    public int WA_Underbarrel1;
    public int WA_Underbarrel2;
    public int WA_Rightbarrel1;
    public int WA_Rightbarrel2;
    public int WA_Leftbarrel1;
    public int WA_Leftbarrel2;
    public int WeaponSkin1;
    public int WeaponSkin2;
    public LoadoutSlotDataJSON()
    {
        SlotName = "Custom Loadout";
        EquippedByDefault = false;
        Weapon1 = 0;
        Weapon2 = 2;
        Equipment1 = 0;
        Equipment2 = 0;
        WA_Sight1 = -1;
        WA_Sight2 = -1;
        WA_Barrel1 = -1;
        WA_Barrel2 = -1;
        WA_Underbarrel1 = -1;
        WA_Underbarrel2 = -1;
        WA_Rightbarrel1 = -1;
        WA_Rightbarrel2 = -1;
        WA_Leftbarrel1 = -1;
        WA_Leftbarrel2 = -1;
        WeaponSkin1 = -1;
        WeaponSkin2 = -1;
    }
}
