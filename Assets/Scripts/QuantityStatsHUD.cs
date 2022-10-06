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
    [SerializeField] Color reloadGradient;
    [SerializeField] Color fullGradient;
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
    float _rd = 0f;
    bool reload = false;
    public void SetAmmoHUDStats(AmmoHUDStats stat, bool isReloading, float reloadDuration, bool doubleStat)
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
            reload = isReloading;
            _rd = reloadDuration;
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
    private void Update()
    {
        if (reload)
        {
            weaponReloadSlider.value = Mathf.Lerp(weaponReloadSlider.value, 1f, _rd * Time.deltaTime);
            weaponReloadSliderFill.color = Color.Lerp(weaponReloadSliderFill.color, reloadGradient, _rd * Time.deltaTime * 2);
        }
        else
        {
            weaponReloadSlider.value = 0f;
            weaponReloadSliderFill.color = fullGradient;
        }
    }
}
