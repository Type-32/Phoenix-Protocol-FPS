using System.Runtime.InteropServices.ComTypes;
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
    public Slider weaponReloadSlider;
    public Image weaponReloadSliderFill;
    public Image eq1;
    public Image eq2;
    public Text eqCounter1, eqCounter2;
    [SerializeField] Color reloadGradient;
    [SerializeField] Color fullGradient;
    public struct AmmoHUDStats
    {
        public int currentAmmo;
        public int ammoPool;
        public int eqCount1;
        public int eqCount2;
        public FireMode firemode;
        public Sprite weaponIcon;
        public Sprite eqIcon1;
        public Sprite eqIcon2;
    };
    float _rd = 0f;
    bool reload = false;
    public void SetAmmoHUDStats(AmmoHUDStats stat, bool isReloading, float reloadDuration, bool doubleStat)
    {
        if (doubleStat)
        {
            ammoPool.enabled = true;
            firemodeIndicator.enabled = true;

            currentAmmo.text = isReloading ? "-" : stat.currentAmmo.ToString();
            ammoPool.text = stat.ammoPool.ToString();
            firemodeIndicator.text = stat.firemode == FireMode.Automatic ? "bbb" : stat.firemode == FireMode.Burst ? "bb" : "b";
            //firemodeIndicator.color = stat.firemode == FireMode.Automatic ? Color.yellow : stat.firemode == FireMode.Burst ? Color.cyan : Color.green;
            weaponTypeIcon.sprite = stat.weaponIcon;
            if (weaponTypeIcon.sprite == null) weaponTypeIcon.enabled = false;
            else weaponTypeIcon.enabled = true;
            reload = isReloading;
            _rd = reloadDuration;
            eq1.sprite = stat.eqIcon1;
            eq2.sprite = stat.eqIcon2;
            eqCounter1.text = stat.eqCount1.ToString();
            eqCounter2.text = stat.eqCount2.ToString();
        }
        else
        {
            ammoPool.enabled = false;
            firemodeIndicator.enabled = false;

            currentAmmo.text = stat.currentAmmo.ToString();
            //weaponTypeIcon.sprite = stat.weaponType == WeaponType.AssaultRifle ? assaultRifleIcon : stat.weaponType == WeaponType.MarksmanRifle ? marksmanRifleIcon : stat.weaponType == WeaponType.Pistol ? pistolIcon : stat.weaponType == WeaponType.Shotgun ? shotgunIcon : stat.weaponType == WeaponType.Melee ? meleeIcon : null;
            weaponTypeIcon.sprite = stat.weaponIcon;
            if (weaponTypeIcon.sprite == null) weaponTypeIcon.enabled = false;
            else weaponTypeIcon.enabled = true;
            eq1.sprite = stat.eqIcon1;
            eq2.sprite = stat.eqIcon2;
            eqCounter1.text = stat.eqCount1.ToString();
            eqCounter2.text = stat.eqCount2.ToString();
        }
    }
}
public enum FireMode
{
    Automatic,
    Single,
    Burst,
    SniperSingle,
    Projectile,
    Shotgun,
    None
}
public enum WeaponType
{
    AssaultRifle,
    SubmachineGun,
    MarksmanRifle,
    Pistol,
    Shotgun,
    Melee,
    ProjectileLauncher,
    SniperRifle,
    None
}
