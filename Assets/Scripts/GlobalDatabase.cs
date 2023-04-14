using PrototypeLib.Modules.FileOperations.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserConfiguration;

public class GlobalDatabase : MonoBehaviour
{
    public List<WeaponData> allWeaponDatas = new();
    public List<EquipmentData> allEquipmentDatas = new();
    public List<WeaponAppearanceMeshData> allWeaponAppearanceDatas = new();
    public List<WeaponAttachmentData> allWeaponAttachmentDatas = new();
    public List<PlayerCosmeticData> allPlayerCosmeticDatas = new();
    public List<LoadoutData> allLoadoutDatas = new();
    public List<UpdateLogData> allLogDatas = new();
    public static GlobalDatabase Instance;
    public LoadoutSelectionScript loadoutSelectionScript;

    [ColorUsage(true, true)] public Color DefaultTrailColor = Color.yellow;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        loadoutSelectionScript.ReadLoadoutDataFromJSON();
        //Debug.LogError($"Unlocked WAMDs: {FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).AppearancesData.unlockedWeaponAppearances.Count}");
        for (int i = 0; i < allWeaponDatas.Count; i++)
        {
            WeaponSystem.ValidateWeapon(i, true);
        }
        CosmeticSystem.VerifyWeaponAppearanceData(true);
        UserDataJSON gmt = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        for (int i = 0; i < gmt.WeaponSmithings.Count; i++)
        {
            GunsmithSystem.VerifyWeaponSmithingData(gmt.WeaponSmithings[i]);
        }
        CosmeticSystem.ValidateLoadoutCosmetics();
    }
    public int FindIndexFromWeaponData(WeaponData data)
    {
        for (int i = 0; i < allWeaponDatas.Count; i++)
        {
            if (allWeaponDatas[i] == data) return i;
        }
        return -1;
    }
    public int RandomWeapon(bool returnIndex = true) { return Random.Range(0, allWeaponDatas.Count - 1); }
    public WeaponData RandomWeaponData(bool returnIndex = true) { return allWeaponDatas[RandomWeapon(returnIndex)]; }
    public int RandomEquipment(bool returnIndex = true) { return Random.Range(0, allEquipmentDatas.Count - 1); }
    public EquipmentData RandomEquipmentData(bool returnIndex = true) { return allEquipmentDatas[RandomEquipment(returnIndex)]; }
}
