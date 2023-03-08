using PrototypeLib.Modules.FileOpsIO;
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
        GunsmithDataJSON gmt = FileOps<GunsmithDataJSON>.ReadFile(UserSystem.GunsmithPath);
        for (int i = 0; i < gmt.weaponSmithings.Count; i++)
        {
            GunsmithSystem.VerifyWeaponSmithingData(gmt.weaponSmithings[i]);
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
}
