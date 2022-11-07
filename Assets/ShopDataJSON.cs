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
    internal ShopDataJSON()
    {
        unlockedWeaponIndexes = new List<int>();
        ownedWeaponIndexes = new List<int>();
        availableWeaponIndexes = new List<int>();
        availableWeaponCrates = new Dictionary<int, int>();
    }
}
