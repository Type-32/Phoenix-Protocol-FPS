using PrototypeLib.Modules.FileOpsIO;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using UserConfiguration;
using InfoTypes.InRoomPreview;

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
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponDatas.Count; i++)
        {
            if (GlobalDatabase.Instance.allWeaponDatas[i] == itemLoadoutData.weaponData[0])
            {
                tempIndex1 = i;
            }
            if (GlobalDatabase.Instance.allWeaponDatas[i] == itemLoadoutData.weaponData[1])
            {
                tempIndex2 = i;
            }
        }
        LoadoutDataJSON tmp = FileOps<LoadoutDataJSON>.ReadFile(UserSystem.LoadoutDataPath);
        tmp.SelectedSlot = loadoutIndex;
        Debug.Log(tmp.Slots[tmp.SelectedSlot].SlotName + " Selected the Slot name");
        FileOps<LoadoutDataJSON>.WriteFile(tmp, UserSystem.LoadoutDataPath);
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
