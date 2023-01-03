using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserConfiguration;

public class GlobalDatabase : MonoBehaviour
{
    public GunsmithDataJSON emptyGunsmithDataJSON;
    public LoadoutDataJSON emptyLoadoutDataJSON;
    public LoadoutSlotDataJSON emptyLoadoutSlotDataJSON;
    public SettingsOptionsJSON emptySettingsOptionsJSON;
    public AppearancesDataJSON emptyAppearancesDataJSON;
    public List<WeaponData> allWeaponDatas = new();
    public List<EquipmentData> allEquipmentDatas = new();
    public List<WeaponAppearanceMeshData> allWeaponAppearanceDatas = new();
    public List<WeaponAttachmentData> allWeaponAttachmentDatas = new();
    public List<PlayerCosmeticData> allPlayerCosmeticDatas = new();
    public List<LoadoutData> allLoadoutDatas = new();
    public List<UpdateLogData> allLogDatas = new();
    public static GlobalDatabase singleton;
    public LoadoutSelectionScript loadoutSelectionScript;
    private void Awake()
    {
        if (singleton)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        singleton = this;
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
        for (int i = 0; i < GunsmithSystem.GunsmithJsonData.weaponSmithings.Count; i++)
        {
            GunsmithSystem.VerifyWeaponSmithingData(GunsmithSystem.GunsmithJsonData.weaponSmithings[i]);
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
