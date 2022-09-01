using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] PlayerControllerManager player;
    public int space = 8;
    public List<ItemData> items = new List<ItemData>();
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    public static PlayerInventory instance;
    private void Awake()
    {
        instance = this;
    }

    public bool AddItem(ItemData item)
    {
        if (!item.equippedByDefault)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough spaces. ");
                return false;
            }
            items.Add(item);
            if(onItemChangedCallback != null) onItemChangedCallback.Invoke();
        }
        return true;
    }
    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
    }
}
