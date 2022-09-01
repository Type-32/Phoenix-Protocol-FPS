using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryData : MonoBehaviour
{
    public PlayerControllerManager player;
    public List<WeaponData> availableWeapons = new List<WeaponData>();
}
