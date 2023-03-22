using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Gunsmith Upgradable Data", menuName = "Project Phoenix/Loadouts/New Gunsmith Upgradable Data", order = 1)]
public class GunsmithUpgradables : ScriptableObject
{
    public Sprite skillIcon;
    public SmithingUpgrades upgrade;
}
