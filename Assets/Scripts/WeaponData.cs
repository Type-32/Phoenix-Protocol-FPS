using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "New Weapon Data", order = 1)]
public class WeaponData : ItemData
{
    public GameObject weaponPrefab;
    public QuantityStatsHUD.WeaponType weaponType;
    public GameObject weaponProjectile;

    [Space]
    [Header("Weapon Stats")]
    public int maxAmmoPerMag = 20;
    public int magazineCount = 3;
    [Range(0f,200f)] public float damage = 30f;
    public float range = 100f;
    public float reloadTime = 3f;
    public float impactForce = 10f;
    public float fireRate = 15f;
    [Range(1f, 3f)] public float FOVMultiplier = 1.1f;
    public float boltRecoveryDuration = 1.5f;
    public float aimSpeed = 3f;
    [Range(0f, 50f)] public float damagePerPellet = 10f;
    public int pelletsPerFire = 1;

    [Space]
    [Header("Weapon Parameters")]
    public bool isEnabled = true;
    public bool isMelee = false;
    public bool isExplosive = false;
    public bool isArmorPenetrative = false;
    public bool isSniperProperties = false;
    public bool enableAutomatic = true;
    public bool enableBurst = true;
    public bool enableSingle = true;

    [Space]
    [Header("Audio Clips")]
    public List<AudioClip> bassClips = new List<AudioClip>();
    public List<AudioClip> fireClips = new List<AudioClip>();
    public List<AudioClip> mechClips = new List<AudioClip>();

}
