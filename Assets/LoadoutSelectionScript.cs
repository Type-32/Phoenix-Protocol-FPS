using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UserConfiguration;

public class LoadoutSelectionScript : MonoBehaviour
{
    public UserDatabase database;
    [Space]
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
        InstantiateLoadoutWeaponSelections();
        OpenLoadoutButtonsVisual();
        loadoutPreviewUI.QuitCustomizationUI();
        DisablePreview();
        loadoutItems[selectedLoadoutIndex].ToggleSelectVisual(true);
        DisableWeaponSelection();
        MainMenuUIManager.instance.CloseLoadoutSelectionMenu();
    }
    public int FindGlobalWeaponIndex(WeaponData data)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (GlobalDatabase.singleton.allWeaponDatas[i] == data) return i;
        }
        return -1;
    }
    public void InitializeLoadoutDataToJSON()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"))) database.InitializeUserDataToJSON();
        if (File.Exists(Path.Combine(Application.persistentDataPath, "UserDataConfig.json")))
        {
            string tempJson = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
            if (string.IsNullOrEmpty(tempJson) || string.IsNullOrWhiteSpace(tempJson))
            {
                database.InitializeUserDataToJSON();
            }
        }
        LoadoutDataJSON data = new();
        data = GlobalDatabase.singleton.emptyLoadoutDataJSON;
        string json = JsonUtility.ToJson(data, true);
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"))) File.CreateText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json")).Close();
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"), json);
        Debug.LogWarning("Initializing Loadout Data To Files...");
    }
    public void WriteLoadoutDataToJSON()
    {
        LoadoutDataJSON data = new();
        data = GlobalDatabase.singleton.emptyLoadoutDataJSON;
        data.SelectedSlot = selectedLoadoutIndex;
        //string json = JsonUtility.ToJson(data, true);
        //File.WriteAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"), json);

        for (int i = 0; i < loadoutDataList.Count; i++)
        {
            //data.Slots[i] = GlobalDatabase.singleton.emptyLoadoutSlotDataJSON;
            //data.Slots[i].WeaponData1 = loadoutDataList[i].weaponData[0];
            //data.Slots[i].WeaponData2 = loadoutDataList[i].weaponData[1];
            data.Slots[i].Weapon1 = FindGlobalWeaponIndex(loadoutDataList[i].weaponData[0]);
            data.Slots[i].Weapon2 = FindGlobalWeaponIndex(loadoutDataList[i].weaponData[1]);
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
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"), json);
        Debug.LogWarning("Writing Loadout Data To Files...");
    }
    public void ReadLoadoutDataFromJSON()
    {
        Debug.Log("Called ReadLoadoutDataFromJSON");
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"))) InitializeLoadoutDataToJSON();
        if (File.Exists(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json")))
        {
            string tempJson = File.ReadAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"));
            if (string.IsNullOrEmpty(tempJson) || string.IsNullOrWhiteSpace(tempJson))
            {
                InitializeLoadoutDataToJSON();
            }
        }
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"));
        Debug.LogWarning("Reading Loadout Data To Files...");
        LoadoutDataJSON jsonData = JsonUtility.FromJson<LoadoutDataJSON>(json);
        json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
        UserDataJSON jsonUserData = JsonUtility.FromJson<UserDataJSON>(json);
        selectedLoadoutIndex = jsonData.SelectedSlot;
        selectedMainWeaponIndex = jsonData.Slots[selectedLoadoutIndex].Weapon1;
        selectedSecondWeaponIndex = jsonData.Slots[selectedLoadoutIndex].Weapon2;
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
        }
        if (checkChange) WriteLoadoutDataToJSON();
    }
    public int GetLoadoutDataFromPreferences(string key)
    {
        int returner = 0;
        switch (key)
        {
            case "selectedLoadoutIndex":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "selectedMainWeaponIndex":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "selectedSecondWeaponIndex":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_SightIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_SightIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_BarrelIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_BarrelIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_UnderbarrelIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_UnderbarrelIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_LeftbarrelIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_LeftbarrelIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_RightbarrelIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_RightbarrelIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_AppearanceIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_AppearanceIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
        }
        Debug.LogError(key + " " + returner);
        return returner;
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
    /*
    public void SetLoadoutDataFromPrefs()
    {
        Debug.LogError("Setting Loadout Data From Prefs...");
        if (PlayerPrefs.HasKey("selectedLoadoutIndex")) selectedLoadoutIndex = GetLoadoutDataFromPreferences("selectedLoadoutIndex");
        if (PlayerPrefs.HasKey("selectedMainWeaponIndex")) selectedMainWeaponIndex = GetLoadoutDataFromPreferences("selectedMainWeaponIndex");
        if (PlayerPrefs.HasKey("selectedSecondWeaponIndex")) selectedSecondWeaponIndex = GetLoadoutDataFromPreferences("selectedSecondWeaponIndex");
        if (PlayerPrefs.HasKey("SMWA_SightIndex1")) loadoutDataList[selectedLoadoutIndex].selectedSightIndex[0] = GetLoadoutDataFromPreferences("SMWA_SightIndex1");
        if (PlayerPrefs.HasKey("SMWA_SightIndex2")) loadoutDataList[selectedLoadoutIndex].selectedSightIndex[1] = GetLoadoutDataFromPreferences("SMWA_SightIndex2");
        if (PlayerPrefs.HasKey("SMWA_BarrelIndex1")) loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[0] = GetLoadoutDataFromPreferences("SMWA_BarrelIndex1");
        if (PlayerPrefs.HasKey("SMWA_BarrelIndex2")) loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[1] = GetLoadoutDataFromPreferences("SMWA_BarrelIndex2");
        if (PlayerPrefs.HasKey("SMWA_UnderbarrelIndex1")) loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[0] = GetLoadoutDataFromPreferences("SMWA_UnderbarrelIndex1");
        if (PlayerPrefs.HasKey("SMWA_UnderbarrelIndex2")) loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[1] = GetLoadoutDataFromPreferences("SMWA_UnderbarrelIndex2");
        if (PlayerPrefs.HasKey("SMWA_LeftbarrelIndex1")) loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[0] = GetLoadoutDataFromPreferences("SMWA_LeftbarrelIndex1");
        if (PlayerPrefs.HasKey("SMWA_LeftbarrelIndex2")) loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[1] = GetLoadoutDataFromPreferences("SMWA_LeftbarrelIndex2");
        if (PlayerPrefs.HasKey("SMWA_RightbarrelIndex1")) loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[0] = GetLoadoutDataFromPreferences("SMWA_RightbarrelIndex1");
        if (PlayerPrefs.HasKey("SMWA_RightbarrelIndex2")) loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[1] = GetLoadoutDataFromPreferences("SMWA_RightbarrelIndex2");
        if (PlayerPrefs.HasKey("SMWA_AppearanceIndex1")) loadoutDataList[selectedLoadoutIndex].selectedAppearanceDataIndex[0] = GetLoadoutDataFromPreferences("SMWA_AppearanceIndex1");
        if (PlayerPrefs.HasKey("SMWA_AppearanceIndex2")) loadoutDataList[selectedLoadoutIndex].selectedAppearanceDataIndex[1] = GetLoadoutDataFromPreferences("SMWA_AppearanceIndex2");

        loadoutDataList[selectedLoadoutIndex].weaponData[0] = FindWeaponDataFromIndex(selectedMainWeaponIndex);
        loadoutDataList[selectedLoadoutIndex].weaponData[1] = FindWeaponDataFromIndex(selectedSecondWeaponIndex);
        loadoutDataList[selectedLoadoutIndex].selectedSight[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSightIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedSight[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSightIndex[1]);
        loadoutDataList[selectedLoadoutIndex].selectedBarrel[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedBarrel[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[1]);
        loadoutDataList[selectedLoadoutIndex].selectedUnderbarrel[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedUnderbarrel[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[1]);
        loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeft[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeft[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[1]);
        loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRight[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRight[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[1]);
    }*/

    public void InstantiateLoadoutSelections()
    {
        Debug.Log("Called Loadout Instantiation");
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
                if (i == selectedLoadoutIndex) temp.SelectLoadout();
                temp.ToggleSelectVisual(true);
            }
        }
        //loadoutItems[selectedLoadoutIndex].SelectLoadout();
    }
    public void InstantiateLoadoutWeaponSelections()
    {
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
        UserDataJSON jsonUserData = JsonUtility.FromJson<UserDataJSON>(json);
        if (loadoutWeaponSelects.Count != 0)
        {
            for (int i = 0; i < loadoutWeaponSelects.Count; i++)
            {
                Destroy(loadoutWeaponSelects[i].gameObject);
            }
            loadoutWeaponSelects.Clear();
        }
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            WeaponValidation tp = WeaponSystem.ValidateWeapon(i, true);
            if (tp != WeaponValidation.Valid)
            {
                continue;
            }
            else
            {
                LoadoutWeaponSelectionItem temp = Instantiate(loadoutWeaponSelectionItemPrefab, loadoutWeaponSelectsHolder).GetComponent<LoadoutWeaponSelectionItem>();
                //loadoutDataList[i].loadoutIndex = i;
                loadoutWeaponSelects.Add(temp);
                temp.weaponData = GlobalDatabase.singleton.allWeaponDatas[i];
                temp.weaponIndex = i;
                temp.customButtonsHolder = this.customButtonsHolder;
                database.WriteInputDataToJSON(jsonUserData);
            }
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
        MainMenuUIManager.instance.AddNotification("Loadout Selection", "You have selected Loadout Number " + (selectedLoadoutIndex + 1) + ".");
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
