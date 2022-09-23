using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Weapon Appearance Data", menuName = "New Weapon Appearance Data", order = 1)]
public class WeaponAppearanceData : ScriptableObject
{
    public WeaponData mainWeaponData;
    public Sprite weaponIconVariant;
    public GameObject weaponVariant;
    public string weaponVariantName;
    [HideInInspector] public int appearanceIndex;
}
