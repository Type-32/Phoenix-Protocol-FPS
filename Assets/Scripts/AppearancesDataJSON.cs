using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AppearancesDataJSON
{
    public List<int> ownedPlayerAppearances = new();
    public List<int> availablePlayerAppearances = new();
    public List<WeaponAppearance> unlockedWeaponAppearances = new();
    public List<WeaponAppearance> availableWeaponAppearances = new();
    public AppearancesDataJSON()
    {
        ownedPlayerAppearances = new();
        availablePlayerAppearances = new();
        unlockedWeaponAppearances = new();
        availableWeaponAppearances = new();
    }
}
[System.Serializable]
public class WeaponAppearance
{
    public int weaponIndex;
    public int appearanceIndex;
    internal WeaponAppearance()
    {
        weaponIndex = 0;
        appearanceIndex = -1;
    }
    public WeaponAppearance(int weaponIndex, int appearanceIndex)
    {
        this.weaponIndex = weaponIndex;
        this.appearanceIndex = appearanceIndex;
    }
    public WeaponAppearance(WeaponAppearanceMeshData data)
    {
        weaponIndex = data.weaponData.GlobalWeaponIndex;
        appearanceIndex = data.WeaponAppearanceMeshDataIndex;
    }
};

