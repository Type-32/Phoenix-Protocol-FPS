using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutWeaponStatisticsDisplay : MonoBehaviour
{
    [SerializeField] Slider damage, range, capacity;
    [SerializeField] Text damageText, rangeText, capacityText;
    public void SetInfo(WeaponData data)
    {
        damage.value = data.damage;
        damageText.text = data.damage.ToString();
        range.value = data.range;
        rangeText.text = data.range.ToString();
        capacity.value = data.maxAmmoPerMag;
        capacityText.text = data.maxAmmoPerMag.ToString();
    }
    public void SetInfo(float dmg, float rng, int mag)
    {
        damage.value = dmg;
        damageText.text = dmg.ToString();
        range.value = rng;
        rangeText.text = rng.ToString();
        capacity.value = mag;
        capacityText.text = mag.ToString();
    }
}
