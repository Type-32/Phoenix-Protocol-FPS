using UnityEngine;
public interface IDamagable
{
    bool TakeDamage(float amount, bool bypassArmor, Transform present);
}