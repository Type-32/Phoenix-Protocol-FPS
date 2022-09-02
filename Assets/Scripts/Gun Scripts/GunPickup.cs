using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [Header("Main Object")]
    public GunStats stats;
    public EquipmentHolder holder;
    public GameObject mainGunObject;

    Transform parent;
    void Start(){
        parent = FindObjectOfType<EquipmentHolder>().transform;
        holder = FindObjectOfType<EquipmentHolder>();
    }
    public void PickupGun(){
        GameObject cachedObj = Instantiate(mainGunObject, parent);
        Debug.Log("Gun Picked From Pickup ");
        cachedObj.GetComponent<GunStats>().weaponData = stats.weaponData;
        cachedObj.GetComponent<GunStats>().damage = stats.damage;
        cachedObj.GetComponent<GunStats>().range = stats.range;
        cachedObj.GetComponent<GunStats>().impactForce = stats.impactForce;
        cachedObj.GetComponent<GunStats>().reloadTime = stats.reloadTime;
        cachedObj.GetComponent<GunStats>().throwForce = stats.throwForce;
        cachedObj.GetComponent<GunStats>().autoReload = stats.autoReload;
        cachedObj.GetComponent<GunStats>().aimSpeed = stats.aimSpeed;
        cachedObj.GetComponent<GunStats>().FOVMultiplier = stats.FOVMultiplier;
        cachedObj.GetComponent<GunStats>().ammo = stats.ammo;
        cachedObj.GetComponent<GunStats>().maxAmmo = stats.maxAmmo;
        cachedObj.GetComponent<GunStats>().ammoPool = stats.ammoPool;
        cachedObj.GetComponent<GunStats>().maxAmmoPool = stats.maxAmmoPool;
        cachedObj.GetComponent<GunStats>().fireRate = stats.fireRate;
        cachedObj.GetComponent<GunStats>().boltRecoveryDuration = stats.boltRecoveryDuration;
        cachedObj.GetComponent<GunStats>().swayIntensity = stats.swayIntensity;
        cachedObj.GetComponent<GunStats>().maxSwayIntensity = stats.maxSwayIntensity;
        cachedObj.GetComponent<GunStats>().smoothness = stats.smoothness;
        cachedObj.GetComponent<GunStats>().rotSwayIntensity = stats.rotSwayIntensity;
        cachedObj.GetComponent<GunStats>().maxRotSwayIntensity = stats.maxRotSwayIntensity;
        cachedObj.GetComponent<GunStats>().rotSmoothness = stats.rotSmoothness;
        cachedObj.GetComponent<GunStats>().verticalRecoil = stats.verticalRecoil;
        cachedObj.GetComponent<GunStats>().horizontalRecoil = stats.horizontalRecoil;
        cachedObj.GetComponent<GunStats>().sphericalShake = stats.sphericalShake;
        cachedObj.GetComponent<GunStats>().transitionalSnappiness = stats.transitionalSnappiness;
        cachedObj.GetComponent<GunStats>().recoilReturnSpeed = stats.recoilReturnSpeed;
        cachedObj.GetComponent<GunStats>().selectedBarrelIndex = stats.selectedBarrelIndex;
        cachedObj.GetComponent<GunStats>().selectedSightIndex = stats.selectedSightIndex;
        cachedObj.GetComponent<GunStats>().selectedUnderbarrelIndex = stats.selectedUnderbarrelIndex;
        Debug.Log("Data Copied ");
        Destroy(gameObject);
    }
    /*    [Header("Generic Attributes")]
    public WeaponFireTypes weaponFireType;
    [Range(0f,200f)] public float damage = 20f;
    public float range = 100f;
    public float impactForce = 3f;
    public float reloadTime = 3f;
    [Range(5, 20)] public float throwForce = 8f;
    public bool autoReload = false;
    public float aimSpeed = 10f;
    [Range(1f, 1.5f)] public float FOVMultiplier = 1.1f;

    [Space]
    [Header("Ammo & Stuff")]
    public int ammo = 30;
    public int maxAmmo = 30;
    public int ammoPool = 120;
    public int maxAmmoPool = 120;

    [Space]
    [Header("Automatic Shooting Statistics")]
    public float fireRate = 15f;
    [Range(0, 2)] public int availableFireModes = 1;

    [Space]
    [Header("SniperSingle Generic Statistics")]
    public float boltRecoveryDuration = 1.5f;

    [Space]
    [Header("Positional Weapon Sway")]
    public float swayIntensity = 0.05f;
    public float maxSwayIntensity = 0.15f;
    public float smoothness = 5f;

    [Space]
    [Header("Rotational Weapon Sway")]
    public float rotSwayIntensity = 1f;
    public float maxRotSwayIntensity = 2f;
    public float rotSmoothness = 5f;

    [Space]
    [Header("Gun Recoil Stats")]
    public float verticalRecoil;
    public float horizontalRecoil;
    public float sphericalShake;
    public float transitionalSnappiness = 5f;
    public float recoilReturnSpeed = 8f;

    */
}
