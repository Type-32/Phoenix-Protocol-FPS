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
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        CosmeticSystem.ValidateLoadoutCosmetics();
        loadoutSelectionScript.ReadLoadoutDataFromJSON();
        for (int i = 0; i < allWeaponDatas.Count; i++)
        {
            WeaponSystem.ValidateWeapon(i, true);
        }
        foreach (WeaponAppearanceMeshData tp in allWeaponAppearanceDatas)
        {
            CosmeticSystem.VerifyWeaponAppearanceData(tp, true);
        }
        UserDataJSON gmt = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        for (int i = 0; i < gmt.WeaponSmithings.Count; i++)
        {
            GunsmithSystem.VerifyWeaponSmithingData(gmt.WeaponSmithings[i]);
        }
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
