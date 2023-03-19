using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Player Cosmetic Data", menuName = "Project Phoenix/Players/New Player Cosmetic Data", order = 1)]
public class PlayerCosmeticData : ItemData
{
    public bool unlockedByDefault = false;
    public enum CosmeticPart
    {
        None,
        Torso,
        Head,
        Legs
    };
    public CosmeticPart appliedPart;
}
