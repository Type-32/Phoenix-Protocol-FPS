using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceHolderScript : MonoBehaviour
{
    public LoadoutMenu loadoutMenu;
    public GameObject slotPrefab;
    public Transform choiceHolder;
    public List<LoadoutSelectionSlot> slotScripts = new List<LoadoutSelectionSlot>();
    public void InstantiateChoiceSlot(WeaponData data, int index)
    {
        LoadoutSelectionSlot temp = Instantiate(slotPrefab, choiceHolder).GetComponent<LoadoutSelectionSlot>();
        temp.loadoutMenu = loadoutMenu;
        temp.SetWeaponSelectionInfo(data, index);
        slotScripts.Add(temp);
    }
    public void InstantiateAllChoiceSlots(int index)
    {
        for(int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            InstantiateChoiceSlot(GlobalDatabase.singleton.allWeaponDatas[i], index);
        }
    }
    public void ClearChoiceSlot()
    {
        for(int i = 0; i < slotScripts.Count; i++)
        {
            Destroy(slotScripts[i].gameObject);
        }
        slotScripts.Clear();
    }
    public void RefreshChoices(int index)
    {
        ClearChoiceSlot();
        InstantiateAllChoiceSlots(index);
    }
    public Button FindElementInSelection(WeaponData data)
    {
        for(int i = 0; i < slotScripts.Count; i++)
        {
            if(data == slotScripts[i].weaponData)
            {
                return slotScripts[i].gameObject.GetComponent<Button>();
            }
        }
        return null;
    }
}
