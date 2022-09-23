using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuantityStatsHUD : MonoBehaviour
{
    public GameObject ammoHUD;
    public GameObject equipmentHUD;

    [Space]
    [Header("Ammo HUD")]
    public Text currentAmmo;
    public Text ammoPool;
    public Text firemodeIndicator;
    public Image weaponTypeIcon;
    public enum FireMode
    {
        Automatic,
        Single,
        Burst,
        SniperSingle,
        Projectile,
        None
    }
    public enum WeaponType
    {
        AssaultRifle,
        MarksmanRifle,
        Pistol,
        Shotgun,
        Equipment,
        Melee,
        GrenadeLauncher,
        SniperRifle,
        Projectile,
        None
    }

    public struct AmmoHUDStats
    {
        public int currentAmmo;
        public int ammoPool;
        public FireMode firemode;
        public Sprite weaponIcon;
    };
    public void SetAmmoHUDStats(AmmoHUDStats stat, bool doubleStat)
    {
        if (doubleStat)
        {
            ammoPool.enabled = true;
            firemodeIndicator.enabled = true;

            currentAmmo.text = stat.currentAmmo.ToString();
            ammoPool.text = stat.ammoPool.ToString();
            firemodeIndicator.text = stat.firemode == FireMode.Automatic ? "bbb" : stat.firemode == FireMode.Burst ? "bb" : "b";
            //firemodeIndicator.color = stat.firemode == FireMode.Automatic ? Color.yellow : stat.firemode == FireMode.Burst ? Color.cyan : Color.green;
            weaponTypeIcon.sprite = stat.weaponIcon;
            if (weaponTypeIcon.sprite == null) weaponTypeIcon.enabled = false;
            else weaponTypeIcon.enabled = true;
        }
        else
        {
            ammoPool.enabled = false;
            firemodeIndicator.enabled = false;

            currentAmmo.text = stat.currentAmmo.ToString();
            //weaponTypeIcon.sprite = stat.weaponType == WeaponType.AssaultRifle ? assaultRifleIcon : stat.weaponType == WeaponType.MarksmanRifle ? marksmanRifleIcon : stat.weaponType == WeaponType.Pistol ? pistolIcon : stat.weaponType == WeaponType.Shotgun ? shotgunIcon : stat.weaponType == WeaponType.Melee ? meleeIcon : null;
            if (weaponTypeIcon.sprite == null) weaponTypeIcon.enabled = false;
            else weaponTypeIcon.enabled = true;
        }
    }
}
