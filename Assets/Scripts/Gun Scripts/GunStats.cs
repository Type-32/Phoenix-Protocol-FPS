using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStats : MonoBehaviour
{
    //public WeaponSO weaponData;
    [Header("Weapon Data")]
    public WeaponData weaponData;

    [HideInInspector] public float damage = 20f;
    [HideInInspector] public float range = 100f;
    [HideInInspector] public float impactForce = 3f;
    [HideInInspector] public float reloadTime = 3f;
    [HideInInspector] public float throwForce = 8f;
    [HideInInspector] public bool autoReload = false;
    [HideInInspector] public float aimSpeed = 3f;
    [HideInInspector] public float FOVMultiplier = 1.1f;
    [HideInInspector] public int ammo = 30;
    [HideInInspector] public int maxAmmo = 30;
    [HideInInspector] public int ammoPool = 120;
    [HideInInspector] public int maxAmmoPool = 120;
    [HideInInspector] public float fireRate = 15f;
    [HideInInspector] public float boltRecoveryDuration = 1.5f;
    [HideInInspector] public FireMode fireMode = FireMode.Automatic;
    public bool gunInteractionEnabled = true;

    [Space]
    [Header("Positional Weapon Sway")]
    public float swayIntensity = 0.05f;
    public float maxSwayIntensity = 0.15f;
    public float smoothness = 5f;
    public float aimSwayIntensity = 0.005f;

    [Space]
    [Header("Rotational Weapon Sway")]
    public float rotSwayIntensity = 1f;
    public float maxRotSwayIntensity = 2f;
    public float rotSmoothness = 5f;
    public float aimRotSwayIntensity = 0.05f;

    [Space]
    [Header("Gun Camera Recoil Stats")]
    public float verticalRecoil = 1f;
    public float horizontalRecoil = 0.2f;
    public float sphericalShake = 0.8f;
    public float positionRecoilRetaliation = 0.02f;
    public float positionRecoilVertical = 0.01f;
    public float transitionalSnappiness = 5f;
    public float recoilReturnSpeed = 8f;
    public float positionTransitionalSnappiness = 5f;
    public float positionRecoilReturnSpeed = 8f;

    [Space]
    [Header("Gun Body Recoil Stats")]
    public float recoilX = 2f;
    public float aimingRecoilX = 0.15f;
    public float recoilY = 2f;
    public float recoilZ = 4f;
    public float kickBackZ = 0.2f;
    public float gunSnappiness = 20f, gunReturnAmount = 24f;
    public float gunRotationSnappiness = 5f, gunRotationReturnAmount = 8f;

    [HideInInspector] public int selectedBarrelIndex = 0;
    [HideInInspector] public int selectedUnderbarrelIndex = 0;
    [HideInInspector] public int selectedSightbarrelLeftIndex = 0;
    [HideInInspector] public int selectedSightbarrelRightIndex = 0;
    [HideInInspector] public int selectedSightbarrelUpIndex = 0;
    [HideInInspector] public int selectedSightIndex = 0;

    [HideInInspector] public bool isWalking = false;
    [HideInInspector] public bool isReloading = false;
    [HideInInspector] public bool isAiming = false;
    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public bool isShooting = false;
    [HideInInspector] public bool isAttaching = false;

    private void Awake()
    {
        InitializeGunStats();
    }
    public void InitializeGunStats()
    {
        ammo = weaponData.maxAmmoPerMag;
        maxAmmo = weaponData.maxAmmoPerMag;
        ammoPool = weaponData.maxAmmoPerMag * weaponData.magazineCount;
        maxAmmoPool = weaponData.maxAmmoPerMag * weaponData.magazineCount;
        fireRate = weaponData.fireRate;
        impactForce = weaponData.impactForce;
        reloadTime = weaponData.reloadTime;
        damage = weaponData.damage;
        range = weaponData.range;
        FOVMultiplier = weaponData.FOVMultiplier;
        boltRecoveryDuration = weaponData.boltRecoveryDuration;
        aimSpeed = weaponData.aimSpeed;
    }
}
