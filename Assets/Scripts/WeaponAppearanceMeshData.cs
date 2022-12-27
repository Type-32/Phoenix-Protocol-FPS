using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Weapon Appearance Mesh Data", menuName = "New Weapon Appearance Mesh Data", order = 1)]
public class WeaponAppearanceMeshData : ItemData
{
    public WeaponData weaponData;
    public Mesh mesh;
    public int WeaponAppearanceMeshDataIndex
    {
        get
        {
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponAppearanceDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allWeaponAppearanceDatas[i] == this) return i;
            }
            return -1;
        }
    }
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    public Rarity rarity;
}
