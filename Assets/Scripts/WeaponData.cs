using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Project Phoenix/Loadouts/New Weapon Data", order = 1)]
public class WeaponData : ItemData
{
    public GameObject weaponPrefab;
    public WeaponType weaponType;
    public GameObject weaponProjectile;
    public int GlobalWeaponIndex
    {
        get
        {
            int temp = -1;
            for (int i = 0; i < GlobalDatabase.Instance.allWeaponDatas.Count; i++)
            {
                if (this == GlobalDatabase.Instance.allWeaponDatas[i])
                {
                    temp = i;
                    break;
                }
            }
            return temp;
        }
    }

    [Space]
    [Header("Weapon Stats")]
    public int maxAmmoPerMag = 20;
    public int magazineCount = 3;
    [Range(0f, 200f)] public float damage = 30f;
    public float range = 100f;
    public float reloadTime = 3f;
    public float impactForce = 10f;
    public float fireRate = 15f;
    [Range(1f, 1.5f), Tooltip("Normally Set Around 1.1f.")] public float FOVMultiplier = 1.1f;
    [Range(0.5f, 2f)] public float hipfireSpread = 1f;
    public float rechamberDelay = 0.1f;
    public float shellEjectionDelay = 0f;
    public float boltRecoveryDuration = 1.5f;
    [Range(3f, 8f), Tooltip("Normally Set Around 2~3f")] public float aimSpeed = 3f;
    [Range(0f, 50f)] public float damagePerPellet = 10f;
    public int pelletsPerFire = 1;
    public float reloadTimePerPellet = 0.8f;
    public int maxDurability = 2500;
    public int durationCostPerUse = 10;

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
    public bool hasHipfireInaccuracy = true;
    public bool ejectCasingAfterRechamber = false;
    public bool reloadByBullet = false;
    public bool useRechamberClipAudioList = true;
    public bool allowMagazineUpgrades = true;
    public bool allowDamageUpgrades = true;
    public bool allowRangeUpgrades = true;
    public bool allowCatridgeUpgrades = true;
    public bool allowReloadUpgrades = true;
    public bool allowADSUpgrades = true;
    public bool allowHipfireUpgrades = true;
    public bool allowDurabilityUpgrades = true;
    public bool allowRecoilUpgrades = true;
    public bool allowRepairs = true;
    public bool automaticLimits = true;
    public int magazineUpgradeLimit
    {
        get
        {
            int temp = 0;
            if (automaticLimits) temp = maxAmmoPerMag + maxAmmoPerMag / 2;
            else temp = magazineUpgradeLimit;
            return temp;
        }
    }

    [Space]
    [Header("Audio Clips")]
    public List<AudioClip> bassClips = new List<AudioClip>();
    public List<AudioClip> fireClips = new List<AudioClip>();
    public List<AudioClip> silencedFireClips = new List<AudioClip>();
    public List<AudioClip> mechClips = new List<AudioClip>();
    public List<AudioClip> NPC_FireClips = new List<AudioClip>();
    public List<AudioClip> NPC_SilencedFireClips = new List<AudioClip>();
    public List<AudioClip> rechamberClips = new List<AudioClip>();
    public AudioClip rechamberStart = default;
    public AudioClip rechamberEnd = default;
    public AudioClip pullBolt = default;
    public AudioClip pullMagazine = default;
    public AudioClip insertMagazine = default;
    public AudioClip recoverBolt = default;
    public AudioClip fullBolt = default;
    public AudioClip slapGun = default;
    public AudioClip insertRound = default;
    public AudioClip raiseGun = default;
    public AudioClip aimIn = default;
    public AudioClip aimOut = default;

    [Space]
    [Header("Customizations")]
    public List<WeaponAppearanceMeshData> applicableVariants = new List<WeaponAppearanceMeshData>();
    public List<WeaponAttachmentData> applicableAttachments = new List<WeaponAttachmentData>();

    //[Space]
    //[Header("Appearances")]


}
