using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[CreateAssetMenu(fileName = "New Item Data", menuName = "New Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [Space]
    [Header("Generic")]
    public string itemName;
    [TextArea]
    public string itemDescription;
    public Sprite itemIcon;
    public bool equippedByDefault = false;

    [Space]
    [Header("Inventorial Data")]
    public int unlockingLevel = 1;
    public bool requiresPurchase = false;
    public bool overrideUnlockingLevel = false;
    public int purchasePrice = 100;
}
