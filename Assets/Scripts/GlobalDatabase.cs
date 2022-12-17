using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserConfiguration;

public class GlobalDatabase : MonoBehaviour
{
    public LoadoutDataJSON emptyLoadoutDataJSON;
    public LoadoutSlotDataJSON emptyLoadoutSlotDataJSON;
    public SettingsOptionsJSON emptySettingsOptionsJSON;
    public List<WeaponData> allWeaponDatas = new();
    public List<EquipmentData> allEquipmentDatas = new();
    public List<WeaponAppearanceData> allWeaponAppearanceDatas = new();
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
        loadoutSelectionScript.ReadLoadoutDataFromJSON();
        for (int i = 0; i < allWeaponDatas.Count; i++)
        {
            WeaponSystem.ValidateWeapon(i, true);
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
