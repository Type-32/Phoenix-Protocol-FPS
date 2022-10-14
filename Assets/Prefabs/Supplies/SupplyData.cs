using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Supply Data", fileName = "New Supply Data", order = 1)]
public class SupplyData : ScriptableObject
{
    [Header("Supply Generic Attributes")]
    public string supplyName = "";
    public Sprite supplyIcon;
    public GameObject supplyInGamePrefab;

    [Space]
    [Header("Supply Data Headings")]
    public bool supplyAmmo = false;
    public bool supplyHealth = false;
    public bool supplyArmor = false;
    public bool destroyOnUse = true;

    [Space]
    [Header("Supply Data Values")]
    public int supplyAmmoMagAmount = 0;
    public float supplyHealthAmount = 0;
    public float supplyArmorAmount = 0;
}
