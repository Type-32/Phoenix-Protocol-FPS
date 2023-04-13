using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Weapon Appearance Mesh Data", menuName = "Project Phoenix/Loadouts/New Weapon Appearance Mesh Data", order = 1)]
public class WeaponAppearanceMeshData : ItemData
{
    public WeaponData weaponData;
    public Mesh mesh;
    public int WeaponAppearanceMeshDataIndex
    {
        get
        {
            for (int i = 0; i < GlobalDatabase.Instance.allWeaponAppearanceDatas.Count; i++)
            {
                if (GlobalDatabase.Instance.allWeaponAppearanceDatas[i] == this) return i;
            }
            return -1;
        }
    }
    public WeaponAppearance AppearanceContent
    {
        get
        {
            return new WeaponAppearance(weaponData.GlobalWeaponIndex, WeaponAppearanceMeshDataIndex);
        }
    }
    public Rarity rarity;
    public bool overrideDefaultTrailerColor = false;
    [ColorUsage(true, true)] public Color trailColor = Color.yellow; //Default to Color.Yellow
}
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

