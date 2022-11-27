using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserConfiguration;

public class LoadoutSelectionItem : MonoBehaviour
{
    public LoadoutSelectionScript loadoutSelection;

    public Text loadoutSelectionItemName;
    public GameObject selectionVisual;
    public LoadoutData itemLoadoutData;
    public int loadoutIndex = 0;
    private void Awake()
    {
        loadoutSelection = GetComponentInParent<LoadoutSelectionScript>();
    }
    void Start()
    {
        DeselectLoadout();
    }
    public void ToggleSelectVisual(bool value)
    {
        selectionVisual.SetActive(value);
    }
    // Update is called once per frame
    public void DeselectLoadout()
    {
        selectionVisual.SetActive(false);
    }
    public void SelectLoadout()
    {
        int tempIndex1 = 0;
        int tempIndex2 = 0;
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (GlobalDatabase.singleton.allWeaponDatas[i] == itemLoadoutData.weaponData[0])
            {
                tempIndex1 = i;
            }
            if (GlobalDatabase.singleton.allWeaponDatas[i] == itemLoadoutData.weaponData[1])
            {
                tempIndex2 = i;
            }
        }
        LoadoutDataJSON tmp = WeaponSystem.LoadoutJsonData;
        tmp.SelectedSlot = loadoutIndex;
        Debug.Log(tmp.Slots[tmp.SelectedSlot].SlotName + " Selected the Slot name");
        WeaponSystem.WriteToLoadoutConfig(tmp);
        loadoutSelection.OnSelectLoadoutCallback(loadoutIndex, tempIndex1, tempIndex2, 0, 0);
        selectionVisual.SetActive(true);
        //loadoutSelection.WriteLoadoutDataToJSON();
    }
    public void SetLoadoutName(string content)
    {
        loadoutSelectionItemName.text = content;
    }
    public void RenameLoadout()
    {
        loadoutSelection.ToggleRenameUI(true);
        loadoutSelection.forRenamingSlot = loadoutIndex;
    }
}
