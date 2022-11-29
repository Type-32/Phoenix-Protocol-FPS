using UnityEngine;
public interface IDamagable
{
    bool TakeDamage(float amount, bool bypassArmor, Vector3 targetPos, Quaternion targetRot, int weaponIndex, bool isWeapon);
}