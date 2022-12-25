using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Weapon Appearance Mesh Data", menuName = "New Weapon Appearance Mesh Data", order = 1)]
public class WeaponAppearanceMeshData : ScriptableObject
{
    public WeaponData weaponData;
    public Mesh mesh;
    public int purchasePrice;
}
