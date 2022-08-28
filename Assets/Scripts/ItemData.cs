using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item Data", menuName = "New Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Togglable,
        Interactable,
        Usable,
        GunItem,
        None
    }
    public enum GunType
    {
        Pistol,
        AssaultRifle,
        MarksmanRifle,
        SpecialPistol,
        FunctionPistol,
        None
    }
    public GameObject itemPrefab;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public ItemType itemType = ItemType.None;
    public GunType gunType = GunType.None;
    public bool equippedByDefault = false;

    [Space]
    [Header("Interactable Item Type Options")]
    public bool isInteractable = true;

    [Space]
    [Header("Usable Item Type Options")]
    public bool isUsable = true;

    [Space]
    [Header("Gun Item Type Parameters")]
    public bool isEnabled = true;
    public int maxAmmoPerMag = 10;
    public int magazineCount = 3;

    public virtual void Use()
    {
        Debug.Log("Using " + itemName);
    }
}
