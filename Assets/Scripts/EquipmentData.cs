using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Equipment Data", menuName = "New Equipment Data", order = 1)]
public class EquipmentData : ItemData
{
    public GameObject equipmentPrefab;
    public QuantityStatsHUD.WeaponType equipmentType;

    [Space]
    [Header("Equipment Stats")]
    public int initialCount = 3;
    [Range(0f, 200f)] public float damage = 80f;
    public float areaOfInfluence = 5f;
    public float influenceForce = 10f;
    public float recoveryTime = 3f;
    public float explosionDelay = 5f;
    public float throwForce;
    public float throwUpwardForce;


    [Space]
    [Header("Equipment Parameters")]
    public bool isExplosive = false;
    public bool explosionOnImpact = false;

    [Space]
    [Header("Audio Clips")]
    public List<AudioClip> collidingClips = new();
    public List<AudioClip> triggeredClips = new();
}
