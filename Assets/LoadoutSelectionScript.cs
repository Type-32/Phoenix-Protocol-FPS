using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UserConfiguration;
using TMPro;
using PrototypeLib.Modules.FileOpsIO;

public class LoadoutSelectionScript : MonoBehaviour
{
    public UserDatabase database;
    [Space]
    public GameObject loadoutSelectionItemPrefab;
    public GameObject loadoutWeaponSelectionItemPrefab;
    public GameObject loadoutEquipmentSelectionItemPrefab;
    [Space]
    public GameObject loadoutButtonsUI;
    public GameObject loadoutWeaponSelectionUI;
    public GameObject loadoutEquipmentSelectionUI;
    public GameObject loadoutAttachmentSelections;
    public GameObject renameVisual;
    public TMP_InputField renameInputField;

    [Space]
    public Transform loadoutButtonsHolder;
    public Transform loadoutWeaponSelectsHolder;
    public Transform loadoutEquipmentSelectsHolder;
    public LoadoutPreviewUI loadoutPreviewUI;
    public LoadoutCustomButtonsHolder customButtonsHolder;
    public LoadoutCustomization loadoutCustomization;
    public List<LoadoutSelectionItem> loadoutItems = new List<LoadoutSelectionItem>();
    public List<LoadoutWeaponSelectionItem> loadoutWeaponSelects = new List<LoadoutWeaponSelectionItem>();
    public List<LoadoutEquipmentSelectionItem> loadoutEquipmentSelects = new List<LoadoutEquipmentSelectionItem>();
    [HideInInspector] public int forSelectedSlot = 0;
    [HideInInspector] public int forRenamingSlot = 0;

    [Space]
    public int selectedLoadoutIndex;
    public int selectedMainWeaponIndex;
    public int selectedSecondWeaponIndex;
    public int selectedEquipmentIndex1;
    public int selectedEquipmentIndex2;

    [Space]
    [Header("More References")]
    public List<LoadoutData> loadoutDataList = new List<LoadoutData>();
    private MenuIdentifier LocalMenuIdentifier;
    // Start is called before the first frame update
    private void Awake()
    {
        LocalMenuIdentifier = GetComponent<MenuIdentifier>();
        LoadoutSelectionItem[] tempItems = loadoutPreviewUI.GetComponentsInChildren<LoadoutSelectionItem>();
        for (int i = 0; i < tempItems.Length; i++)
        {
            loadoutItems.Add(tempItems[i]);
        }

        Debug.Log("Loadout Selection Script Awake()");
        //MainMenuUIManager.instance.CloseLoadoutSelectionMenu();
    }
    void Start()
    {
        Debug.Log("Loadout Selection Script Start()");
        InstantiateLoadoutSelections();
        //SetLoadoutDataFromPrefs();
        InstantiateLoadoutItemSelections();
        OpenLoadoutButtonsVisual();
        loadoutPreviewUI.QuitCustomizationUI();
        DisablePreview();
        loadoutItems[selectedLoadoutIndex].ToggleSelectVisual(true);
        ToggleRenameUI(false);
        DisableWeaponSelection();
        DisableEquipmentSelection();
        //MenuManager.instance.CloseLoadoutSelectionMenu();
    }
    public int FindGlobalWeaponIndex(WeaponData data)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (GlobalDatabase.singleton.allWeaponDatas[i] == data) return i;
        }
        return -1;
    }
    public void WriteLoadoutDataToJSON()
    {
        LoadoutDataJSON data = FileOps<LoadoutDataJSON>.ReadFile(UserSystem.LoadoutDataPath);
        data.SelectedSlot = selectedLoadoutIndex;
        //string json = JsonUtility.ToJson(data, true);
        //File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.LoadoutDataConfigKey), json);

        for (int i = 0; i < loadoutDataList.Count; i++)
        {
            //data.Slots[i] = GlobalDatabase.singleton.emptyLoadoutSlotDataJSON;
            //data.Slots[i].WeaponData1 = loadoutDataList[i].weaponData[0];
            //data.Slots[i].WeaponData2 = loadoutDataList[i].weaponData[1];
            data.Slots[i].Weapon1 = FindGlobalWeaponIndex(loadoutDataList[i].weaponData[0]);
            data.Slots[i].Weapon2 = FindGlobalWeaponIndex(loadoutDataList[i].weaponData[1]);
            data.Slots[i].Equipment1 = Database.FindEquipmentDataIndex(loadoutDataList[i].equipmentData[0]);
            data.Slots[i].Equipment2 = Database.FindEquipmentDataIndex(loadoutDataList[i].equipmentData[1]);
            data.Slots[i].WA_Sight1 = loadoutDataList[i].selectedSightIndex[0];
            data.Slots[i].WA_Sight2 = loadoutDataList[i].selectedSightIndex[1];
            data.Slots[i].WA_Barrel1 = loadoutDataList[i].selectedBarrelIndex[0];
            data.Slots[i].WA_Barrel2 = loadoutDataList[i].selectedBarrelIndex[1];
            data.Slots[i].WA_Underbarrel1 = loadoutDataList[i].selectedUnderbarrelIndex[0];
            data.Slots[i].WA_Underbarrel2 = loadoutDataList[i].selectedUnderbarrelIndex[1];
            data.Slots[i].WA_Rightbarrel1 = loadoutDataList[i].selectedSidebarrelRightIndex[0];
            data.Slots[i].WA_Rightbarrel2 = loadoutDataList[i].selectedSidebarrelRightIndex[1];
            data.Slots[i].WA_Leftbarrel1 = loadoutDataList[i].selectedSidebarrelLeftIndex[0];
            data.Slots[i].WA_Leftbarrel2 = loadoutDataList[i].selectedSidebarrelLeftIndex[1];
            data.Slots[i].WeaponSkin1 = loadoutDataList[i].selectedAppearanceDataIndex[0];
            data.Slots[i].WeaponSkin2 = loadoutDataList[i].selectedAppearanceDataIndex[1];
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.LoadoutDataConfigKey), json);
        Debug.LogWarning("Writing Loadout Data To Files...");
    }
    public void ReadLoadoutDataFromJSON()
    {
        LoadoutDataJSON jsonData = FileOps<LoadoutDataJSON>.ReadFile(UserSystem.LoadoutDataPath);
        UserDataJSON jsonUserData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        AppearancesDataJSON appearancesData = FileOps<AppearancesDataJSON>.ReadFile(UserSystem.AppearancesConfigPath);
        selectedLoadoutIndex = jsonData.SelectedSlot;
        selectedMainWeaponIndex = jsonData.Slots[selectedLoadoutIndex].Weapon1;
        selectedSecondWeaponIndex = jsonData.Slots[selectedLoadoutIndex].Weapon2;
        selectedEquipmentIndex1 = jsonData.Slots[selectedLoadoutIndex].Equipment1;
        selectedEquipmentIndex2 = jsonData.Slots[selectedLoadoutIndex].Equipment2;
        bool checkChange = false;
        for (int i = 0; i < loadoutDataList.Count; i++)
        {
            if (jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon1))
            {
                loadoutDataList[i].weaponData[0] = FindWeaponDataFromIndex(jsonData.Slots[i].Weapon1);
            }
            else
            {
                checkChange = true;
                loadoutDataList[i].weaponData[0] = FindWeaponDataFromIndex(0);
            }

            if (jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon2))
            {
                loadoutDataList[i].weaponData[1] = FindWeaponDataFromIndex(jsonData.Slots[i].Weapon2);
            }
            else
            {
                checkChange = true;
                loadoutDataList[i].weaponData[1] = FindWeaponDataFromIndex(2);
            }
            loadoutDataList[i].equipmentData[0] = Database.FindEquipmentData(jsonData.Slots[i].Equipment1);
            loadoutDataList[i].equipmentData[1] = Database.FindEquipmentData(jsonData.Slots[i].Equipment2);
            loadoutDataList[i].selectedSight[0] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Sight1);
            loadoutDataList[i].selectedSight[1] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Sight2);
            loadoutDataList[i].selectedBarrel[0] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Barrel1);
            loadoutDataList[i].selectedBarrel[1] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Barrel2);
            loadoutDataList[i].selectedUnderbarrel[0] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Underbarrel1);
            loadoutDataList[i].selectedUnderbarrel[1] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Underbarrel2);
            loadoutDataList[i].selectedSidebarrelLeft[0] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Leftbarrel1);
            loadoutDataList[i].selectedSidebarrelLeft[1] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Leftbarrel2);
            loadoutDataList[i].selectedSidebarrelRight[0] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Rightbarrel1);
            loadoutDataList[i].selectedSidebarrelRight[1] = FindAttachmentDataFromIndex(jsonData.Slots[i].WA_Rightbarrel2);
            loadoutDataList[i].selectedAppearanceData[0] = (jsonData.Slots[i].WeaponSkin1 == -1 ? null : (appearancesData.unlockedWeaponAppearances.Contains(new WeaponAppearance(GlobalDatabase.singleton.allWeaponAppearanceDatas[jsonData.Slots[i].WeaponSkin1])) ? GlobalDatabase.singleton.allWeaponAppearanceDatas[jsonData.Slots[i].WeaponSkin1] : null));
            loadoutDataList[i].selectedAppearanceData[1] = (jsonData.Slots[i].WeaponSkin2 == -1 ? null : (appearancesData.unlockedWeaponAppearances.Contains(new WeaponAppearance(GlobalDatabase.singleton.allWeaponAppearanceDatas[jsonData.Slots[i].WeaponSkin2])) ? GlobalDatabase.singleton.allWeaponAppearanceDatas[jsonData.Slots[i].WeaponSkin2] : null));

            loadoutDataList[i].selectedSightIndex[0] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon1) ? jsonData.Slots[i].WA_Sight1 : -1;
            loadoutDataList[i].selectedSightIndex[1] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon2) ? jsonData.Slots[i].WA_Sight2 : -1;
            loadoutDataList[i].selectedBarrelIndex[0] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon1) ? jsonData.Slots[i].WA_Barrel1 : -1;
            loadoutDataList[i].selectedBarrelIndex[1] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon2) ? jsonData.Slots[i].WA_Barrel2 : -1;
            loadoutDataList[i].selectedUnderbarrelIndex[0] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon1) ? jsonData.Slots[i].WA_Underbarrel1 : -1;
            loadoutDataList[i].selectedUnderbarrelIndex[1] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon2) ? jsonData.Slots[i].WA_Underbarrel2 : -1;
            loadoutDataList[i].selectedSidebarrelLeftIndex[0] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon1) ? jsonData.Slots[i].WA_Leftbarrel1 : -1;
            loadoutDataList[i].selectedSidebarrelLeftIndex[1] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon2) ? jsonData.Slots[i].WA_Leftbarrel2 : -1;
            loadoutDataList[i].selectedSidebarrelRightIndex[0] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon1) ? jsonData.Slots[i].WA_Rightbarrel1 : -1;
            loadoutDataList[i].selectedSidebarrelRightIndex[1] = jsonUserData.shopData.ownedWeaponIndexes.Contains(jsonData.Slots[i].Weapon2) ? jsonData.Slots[i].WA_Rightbarrel2 : -1;
            loadoutDataList[i].selectedAppearanceDataIndex[0] = (jsonData.Slots[i].WeaponSkin1 == -1 ? -1 : (appearancesData.unlockedWeaponAppearances.Contains(new WeaponAppearance(GlobalDatabase.singleton.allWeaponAppearanceDatas[jsonData.Slots[i].WeaponSkin1])) ? jsonData.Slots[i].WeaponSkin1 : -1));
            loadoutDataList[i].selectedAppearanceDataIndex[1] = (jsonData.Slots[i].WeaponSkin2 == -1 ? -1 : (appearancesData.unlockedWeaponAppearances.Contains(new WeaponAppearance(GlobalDatabase.singleton.allWeaponAppearanceDatas[jsonData.Slots[i].WeaponSkin2])) ? jsonData.Slots[i].WeaponSkin2 : -1));
        }
        if (checkChange) WriteLoadoutDataToJSON();
    }

    public WeaponData FindWeaponDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponDatas[i];
        }
        return null;
    }
    public WeaponAttachmentData FindAttachmentDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponAttachmentDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponAttachmentDatas[i];
        }
        return null;
    }

    public void InstantiateLoadoutSelections()
    {
        LoadoutDataJSON tp = FileOps<LoadoutDataJSON>.ReadFile(UserSystem.LoadoutDataPath);
        Debug.Log("Called Loadout Instantiation");
        for (int i = 0; i < loadoutDataList.Count; i++)
        {
            LoadoutSelectionItem temp = Instantiate(loadoutSelectionItemPrefab, loadoutButtonsHolder).GetComponent<LoadoutSelectionItem>();
            temp.itemLoadoutData = loadoutDataList[i];
            temp.DeselectLoadout();
            temp.loadoutIndex = i;
            loadoutDataList[i].loadoutIndex = i;
            loadoutDataList[i].loadoutName = tp.Slots[i].SlotName;
            temp.SetLoadoutName(tp.Slots[i].SlotName);
            loadoutItems.Add(temp);
            if (i == tp.SelectedSlot) temp.SelectLoadout();
            temp.ToggleSelectVisual(true);
            selectedLoadoutIndex = tp.SelectedSlot;
        }
        //loadoutItems[selectedLoadoutIndex].SelectLoadout();
    }
    public void InstantiateLoadoutItemSelections()
    {
        UserDataJSON jsonUserData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        if (loadoutWeaponSelects.Count != 0)
        {
            for (int i = 0; i < loadoutWeaponSelects.Count; i++)
            {
                Destroy(loadoutWeaponSelects[i].gameObject);
            }
            loadoutWeaponSelects.Clear();
        }
        if (loadoutEquipmentSelects.Count != 0)
        {
            for (int i = 0; i < loadoutEquipmentSelects.Count; i++)
            {
                Destroy(loadoutEquipmentSelects[i].gameObject);
            }
            loadoutEquipmentSelects.Clear();
        }
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (!jsonUserData.shopData.ownedWeaponIndexes.Contains(i)) continue;
            LoadoutWeaponSelectionItem temp = Instantiate(loadoutWeaponSelectionItemPrefab, loadoutWeaponSelectsHolder).GetComponent<LoadoutWeaponSelectionItem>();
            //loadoutDataList[i].loadoutIndex = i;
            loadoutWeaponSelects.Add(temp);
            temp.weaponData = GlobalDatabase.singleton.allWeaponDatas[i];
            temp.weaponIndex = i;
            temp.customButtonsHolder = this.customButtonsHolder;
            FileOps<UserDataJSON>.WriteFile(jsonUserData, UserSystem.UserDataPath);
        }
        for (int i = 0; i < GlobalDatabase.singleton.allEquipmentDatas.Count; i++)
        {
            LoadoutEquipmentSelectionItem temp = Instantiate(loadoutEquipmentSelectionItemPrefab, loadoutEquipmentSelectsHolder).GetComponent<LoadoutEquipmentSelectionItem>();
            //loadoutDataList[i].loadoutIndex = i;
            loadoutEquipmentSelects.Add(temp);
            temp.equipmentData = GlobalDatabase.singleton.allEquipmentDatas[i];
            temp.equipmentIndex = i;
            temp.customButtonsHolder = this.customButtonsHolder;
            FileOps<UserDataJSON>.WriteFile(jsonUserData, UserSystem.UserDataPath);
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
        MenuManager.instance.AddNotification("Loadout Selection", "You have selected Loadout " + loadoutDataList[selectedLoadoutIndex].loadoutName + ".");
        //WriteLoadoutDataToJSON();
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
    public void EnableEquipmentSelection()
    {
        loadoutEquipmentSelectionUI.SetActive(true);
    }
    public void DisableEquipmentSelection()
    {
        loadoutEquipmentSelectionUI.SetActive(false);
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
    public void ToggleRenameUI(bool value)
    {
        renameVisual.SetActive(value);
        if (!value)
        {
            renameInputField.text = "";
        }
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

        if (loadoutCustomization.skinObjects.Count <= 0) customButtonsHolder.buttons[5].SetActive(false);
        else customButtonsHolder.buttons[5].SetActive(value);
    }
    public void ConfirmRename()
    {
        LoadoutDataJSON tmp = FileOps<LoadoutDataJSON>.ReadFile(UserSystem.LoadoutDataPath);
        loadoutItems[forRenamingSlot].SetLoadoutName(renameInputField.text);
        tmp.Slots[forRenamingSlot].SlotName = renameInputField.text;
        FileOps<LoadoutDataJSON>.WriteFile(tmp, UserSystem.LoadoutDataPath);
        ToggleRenameUI(false);
    }
}
