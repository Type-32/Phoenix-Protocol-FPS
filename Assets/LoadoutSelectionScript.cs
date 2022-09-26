using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelectionScript : MonoBehaviour
{
    public GameObject loadoutSelectionItemPrefab;
    public GameObject loadoutWeaponSelectionItemPrefab;
    [Space]
    public GameObject loadoutButtonsUI;
    public GameObject loadoutWeaponSelectionUI;
    public GameObject loadoutAttachmentSelections;

    [Space]
    public Transform loadoutButtonsHolder;
    public Transform loadoutWeaponSelectsHolder;
    public LoadoutPreviewUI loadoutPreviewUI;
    public LoadoutCustomButtonsHolder customButtonsHolder;
    public LoadoutCustomization loadoutCustomization;
    public List<LoadoutSelectionItem> loadoutItems = new List<LoadoutSelectionItem>();
    public List<LoadoutWeaponSelectionItem> loadoutWeaponSelects = new List<LoadoutWeaponSelectionItem>();
    [HideInInspector] public int forSelectedSlot = 0;

    [Space]
    public int selectedLoadoutIndex;
    public int selectedMainWeaponIndex;
    public int selectedSecondWeaponIndex;
    public int selectedEquipmentIndex1;
    public int selectedEquipmentIndex2;

    [Space]
    [Header("More References")]
    public List<LoadoutData> loadoutDataList = new List<LoadoutData>();
    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        LoadoutSelectionItem[] tempItems = loadoutPreviewUI.GetComponentsInChildren<LoadoutSelectionItem>();
        for(int i = 0; i < tempItems.Length; i++)
        {
            loadoutItems.Add(tempItems[i]);
        }
        InstantiateLoadoutSelections();
        InstantiateLoadoutWeaponSelections();
        OpenLoadoutButtonsVisual();
        DisablePreview();
        DisableWeaponSelection();
        loadoutPreviewUI.QuitCustomizationUI();
    }

    public void InstantiateLoadoutSelections()
    {
        for (int i = 0; i < loadoutDataList.Count; i++)
        {
            LoadoutSelectionItem temp = Instantiate(loadoutSelectionItemPrefab, loadoutButtonsHolder).GetComponent<LoadoutSelectionItem>();
            temp.itemLoadoutData = loadoutDataList[i];
            temp.DeselectLoadout();
            temp.loadoutIndex = i;
            loadoutDataList[i].loadoutIndex = i;
            temp.SetLoadoutName(loadoutDataList[i].loadoutName);
            loadoutItems.Add(temp);
            if (loadoutDataList[i].isDefault)
            {
                temp.SelectLoadout();
                temp.ToggleSelectVisual(true);
            }
        }
    }
    public void InstantiateLoadoutWeaponSelections()
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            LoadoutWeaponSelectionItem temp = Instantiate(loadoutWeaponSelectionItemPrefab, loadoutWeaponSelectsHolder).GetComponent<LoadoutWeaponSelectionItem>();
            //loadoutDataList[i].loadoutIndex = i;
            loadoutWeaponSelects.Add(temp);
            temp.weaponData = GlobalDatabase.singleton.allWeaponDatas[i];
            temp.weaponIndex = i;
        }
    }

    public void OnSelectLoadoutCallback(int selectedIndex, int selectedWeaponIndex1, int selectedWeaponIndex2, int selectedEquipment1, int selectedEquipment2)
    {
        selectedLoadoutIndex = selectedIndex;
        selectedMainWeaponIndex = selectedWeaponIndex1;
        selectedSecondWeaponIndex = selectedWeaponIndex2;
        selectedEquipmentIndex1 = selectedEquipment1;
        selectedEquipmentIndex2 = selectedEquipment2;
        loadoutPreviewUI.SetPreviewInfo(loadoutDataList[selectedLoadoutIndex]);
        DisableAllSelectedVisuals();
        loadoutItems[selectedLoadoutIndex].ToggleSelectVisual(true);
        EnablePreview();
    }
    public void DisableAllSelectedVisuals()
    {
        for (int i = 0; i < loadoutItems.Count; i++)
        {
            loadoutItems[i].DeselectLoadout();
        }
    }
    public void DisablePreview()
    {
        loadoutPreviewUI.gameObject.SetActive(false);
    }
    public void EnablePreview()
    {
        loadoutPreviewUI.gameObject.SetActive(true);
    }
    public void EnableWeaponSelection()
    {
        loadoutWeaponSelectionUI.SetActive(true);
    }
    public void DisableWeaponSelection()
    {
        loadoutWeaponSelectionUI.SetActive(false);
    }
    public void CloseLoadoutButtonsVisual()
    {
        loadoutButtonsUI.SetActive(false);
    }
    public void OpenLoadoutButtonsVisual()
    {
        loadoutButtonsUI.SetActive(true);
    }
    public void ToggleCustomizationMenu(bool value)
    {
        loadoutCustomization.gameObject.SetActive(value);
    }
    public void ToggleCustomizeSelectionUI(bool value)
    {
        loadoutAttachmentSelections.SetActive(value);
    }
    public void ToggleCustomizeButtonsUI(bool value)
    {
        customButtonsHolder.gameObject.SetActive(value);

        if (loadoutCustomization.sightObjects.Count <= 0) customButtonsHolder.buttons[0].SetActive(false);
        else customButtonsHolder.buttons[0].SetActive(value);

        if (loadoutCustomization.barrelObjects.Count <= 0) customButtonsHolder.buttons[1].SetActive(false);
        else customButtonsHolder.buttons[1].SetActive(value);

        if (loadoutCustomization.underbarrelObjects.Count <= 0) customButtonsHolder.buttons[2].SetActive(false);
        else customButtonsHolder.buttons[2].SetActive(value);

        if (loadoutCustomization.leftbarrelObjects.Count <= 0) customButtonsHolder.buttons[3].SetActive(false);
        else customButtonsHolder.buttons[3].SetActive(value);

        if (loadoutCustomization.rightbarrelObjects.Count <= 0) customButtonsHolder.buttons[4].SetActive(false);
        else customButtonsHolder.buttons[4].SetActive(value);
    }
}
