using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxPart : MonoBehaviour, IDamagable
{
    public PlayerControllerManager player;
    public bool applyBotConfig = false;
    public enum PlayerPart
    {
        Head,
        Torso,
        Legs,
        Hands,
        None
    };
    public PlayerPart part = PlayerPart.None;
    public bool TakeDamage(float amount, bool bypassArmor, Vector3 targetPos, Quaternion targetRot, int weaponIndex, bool isWeapon)
    {
        float processed = amount;
        switch (part)
        {
            case PlayerPart.Head:
                processed = amount + 10;
                break;
            case PlayerPart.Torso:
                processed = amount;
                break;
            case PlayerPart.Legs:
                processed = amount - 5;
                break;
            case PlayerPart.Hands:
                processed = amount - 10;
                break;
        }
        bool tmp = false;
        if (!applyBotConfig)
        {
            tmp = player.TakeDamage(processed, bypassArmor, targetPos, targetRot, weaponIndex, isWeapon);
        }
        return tmp;
    }
}
