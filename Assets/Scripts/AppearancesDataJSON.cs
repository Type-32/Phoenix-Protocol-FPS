using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AppearancesDataJSON
{
    public struct WeaponAppearance
    {
        public bool isUnlocked;
        public int weaponIndex;
        public int appearanceIndex;
    };
    public List<int> ownedPlayerAppearances = new();
    public List<int> availablePlayerAppearances = new();
    public List<WeaponAppearance> unlockedWeaponAppearances = new();
    public List<WeaponAppearance> availableWeaponAppearances = new();
    internal AppearancesDataJSON()
    {
        ownedPlayerAppearances = new();
        availablePlayerAppearances = new();
        unlockedWeaponAppearances = new();
        availableWeaponAppearances = new();
    }
}
