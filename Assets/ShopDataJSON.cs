using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ShopDataJSON
{
    public List<int> unlockedWeaponIndexes;
    public List<int> ownedWeaponIndexes;
    public List<int> availableWeaponIndexes;
    public Dictionary<int, int> availableWeaponCrates;
    public Dictionary<int, bool> announcedUnlockedWeaponIndexes;
    public Dictionary<int, bool> announcedOwnedWeaponIndexes;
    internal ShopDataJSON()
    {
        unlockedWeaponIndexes = new();
        ownedWeaponIndexes = new();
        availableWeaponIndexes = new();
        announcedUnlockedWeaponIndexes = new();
        announcedOwnedWeaponIndexes = new();
    }
}
