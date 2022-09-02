using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemData itemData;
    public GameObject item;
    public abstract void Use();
    public abstract void InitializeStart();
    public abstract void InitializeAwake();
}
