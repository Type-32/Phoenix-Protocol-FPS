using UnityEngine;
using System.Collections.Generic;
using System;
using PrototypeLib.Modules.FileOperations.IO;
using UserConfiguration;

[System.Serializable]
public class UserDataJSON
{
    public bool hasInitialized;
    public string username;
    public int userLevel;
    public int userLevelXP;
    public int userCoins;
    public string AccessToken;
    public ShopDataJSON ShopData;
    public UserProfileData ProfileData;
    public SmithingResources SmithingResources;
    public LoadoutDataJSON LoadoutData;
    public AppearancesDataJSON AppearancesData;
    public List<WeaponSmithingData> WeaponSmithings = new();
    public UserDataJSON()
    {
        username = $"User {UnityEngine.Random.Range(1000, 9999)}";
        hasInitialized = false;
        userLevel = 1;
        userLevelXP = 0;
        userCoins = 1000;
        SmithingResources.iron = 1000; //Gained Through Matches, approx. 500~2500 per match
        SmithingResources.experience = 0; //Gained Through Matches, approx. 50~300 per match
        SmithingResources.composites = 500; //Gained Through Matches, approx. 150~400 per match
        SmithingResources.mechanisms = 50; //Gained Through Combining Iron and Composites, approx. 10~50 per fusion, around 600~1500 iron needed and 150~400 composites needed
        SmithingResources.tools = 5; //Gained Through Match Kills, approx 1~3 per match
        AccessToken = "";
        WeaponSmithings = new();
        ProfileData = new();
        AppearancesData = new();
        ShopData = new();
        LoadoutData = new LoadoutDataJSON(8);
        AppearancesData = new();
    }
}
[System.Serializable]
public class LoadoutDataJSON
{
    public int SelectedSlot = 0;
    public List<LoadoutSlotDataJSON> Slots;
    public LoadoutDataJSON()
    {
        SelectedSlot = 0;
        Slots = new();
        for (int i = 0; i < 8; i++) Slots.Add(new LoadoutSlotDataJSON($"Custom Loadout {i}"));
    }
    public LoadoutDataJSON(int count)
    {
        SelectedSlot = 0;
        Slots = new();
        for (int i = 0; i < count; i++) Slots.Add(new LoadoutSlotDataJSON($"Custom Loadout {i}"));
    }
}
[System.Serializable]
public class LoadoutSlotDataJSON
{
    public string SlotName;
    public bool EquippedByDefault;
    public int Weapon1;
    public int Weapon2;
    public int Equipment1;
    public int Equipment2;

    public int WA_Sight1;
    public int WA_Sight2;
    public int WA_Barrel1;
    public int WA_Barrel2;
    public int WA_Underbarrel1;
    public int WA_Underbarrel2;
    public int WA_Rightbarrel1;
    public int WA_Rightbarrel2;
    public int WA_Leftbarrel1;
    public int WA_Leftbarrel2;
    public int WeaponSkin1;
    public int WeaponSkin2;
    public LoadoutSlotDataJSON(string slotName)
    {
        SlotName = slotName;
        EquippedByDefault = false;
        Weapon1 = 0;
        Weapon2 = 2;
        Equipment1 = 0;
        Equipment2 = 0;
        WA_Sight1 = -1;
        WA_Sight2 = -1;
        WA_Barrel1 = -1;
        WA_Barrel2 = -1;
        WA_Underbarrel1 = -1;
        WA_Underbarrel2 = -1;
        WA_Rightbarrel1 = -1;
        WA_Rightbarrel2 = -1;
        WA_Leftbarrel1 = -1;
        WA_Leftbarrel2 = -1;
        WeaponSkin1 = -1;
        WeaponSkin2 = -1;
    }
}
[System.Serializable]
public class AppearancesDataJSON
{
    public List<int> ownedPlayerAppearances = new();
    public List<int> availablePlayerAppearances = new();
    public List<WeaponAppearance> unlockedWeaponAppearances = new();
    public List<WeaponAppearance> availableWeaponAppearances = new();
    public AppearancesDataJSON()
    {
        ownedPlayerAppearances = new();
        availablePlayerAppearances = new();
        unlockedWeaponAppearances = new();
        availableWeaponAppearances = new();
    }
}

#region Backend Types
[System.Serializable]
public class WeaponAppearance : IEquatable<WeaponAppearance>
{
    public int weaponIndex;
    public int appearanceIndex;
    internal WeaponAppearance()
    {
        weaponIndex = 0;
        appearanceIndex = -1;
    }
    public WeaponAppearance(int weaponIndex, int appearanceIndex)
    {
        this.weaponIndex = weaponIndex;
        this.appearanceIndex = appearanceIndex;
    }

    public WeaponAppearance(WeaponAppearanceMeshData data)
    {
        weaponIndex = data.weaponData.GlobalWeaponIndex;
        appearanceIndex = data.WeaponAppearanceMeshDataIndex;
    }
    public bool Equals(WeaponAppearance other)
    {
        if (other.appearanceIndex == appearanceIndex && other.weaponIndex == weaponIndex) return true;
        return false;
    }
}
[System.Serializable]
public class UserProfileData
{
    public string description;
    public Sprite userPFP;
    public int totalKills, totalDeaths, totalXPGained;
    public UserProfileData()
    {
        description = "Legend!";
        userPFP = null;
        totalKills = 0;
        totalDeaths = 0;
        totalXPGained = 0;
    }
}
[System.Serializable]
public class WeaponSmithingData
{
    #region Original Values
    public float o_Damage { get { return GlobalDatabase.Instance.allWeaponDatas[weaponIndex].damage; } }
    public int o_MagazineCapacity { get { return GlobalDatabase.Instance.allWeaponDatas[weaponIndex].maxAmmoPerMag; } }
    public float o_Range { get { return GlobalDatabase.Instance.allWeaponDatas[weaponIndex].range; } }
    public CatridgeType o_Catridge { get { return CatridgeType.Standard; } }
    public int o_Durability { get { return GlobalDatabase.Instance.allWeaponDatas[weaponIndex].maxDurability; } }
    public float o_Reload { get { return GlobalDatabase.Instance.allWeaponDatas[weaponIndex].reloadTime; } }
    public float o_Hipfire { get { return 0f; } }
    public float o_Recoil { get { return 0f; } }
    #endregion
    public int weaponIndex;
    public SmithingUpgrades upgrades;
    public WeaponSmithingData(int i)
    {
        weaponIndex = i;
    }
};
[System.Serializable]
public class SmithingUpgrades
{
    public SmithingUpgradeType type;
    public string name;
    public int magCapacityMod;//Max Magazine Capacity
    public float damageMod;//Damage
    public float rangeMod;//Range
    public float reloadMod;//Reload Duration
    public float hipfireMod;//Hipfire Accuracy
    public float recoilMod;//Recoil Amount
    public int durabilityMod;
    public CatridgeType catridgeMod;//Catridge; In affiliation with Damage, Range
    public ModificationRecord record;
    #region Set Functions
    public bool SetCatridge(CatridgeType catridge)
    {
        bool success = false;
        if (catridge == catridgeMod || catridge == CatridgeType.Standard) return success;
        catridgeMod = catridge;
        success = true;
        if (catridge == CatridgeType.HighPowered)
        {
            damageMod += 10f;
            rangeMod -= 50f;
        }
        if (catridge == CatridgeType.HighCaliber)
        {
            damageMod += 15f;
            rangeMod += 100f;
        }
        if (catridge == CatridgeType.Sonic) rangeMod -= 75f;
        if (catridge == CatridgeType.ArmorPiercing)
        {
            damageMod += 5f;
            rangeMod += 25f;
        }
        if (catridge == CatridgeType.Phosphorus)
        {
            damageMod -= 5f;
            rangeMod -= 75f;
        }
        record.CheckModifications(magCapacityMod, damageMod, rangeMod, reloadMod, hipfireMod, recoilMod, durabilityMod);
        return success;
    }
    public void SetDamageMod(float amount)
    {
        damageMod = amount;
        record.CheckModifications(magCapacityMod, damageMod, rangeMod, reloadMod, hipfireMod, recoilMod, durabilityMod);
    }
    public void SetMagCapacityMod(int amount)
    {
        magCapacityMod = amount;
        record.CheckModifications(magCapacityMod, damageMod, rangeMod, reloadMod, hipfireMod, recoilMod, durabilityMod);
    }
    public void SetRangeMod(float amount)
    {
        rangeMod = amount;
        record.CheckModifications(magCapacityMod, damageMod, rangeMod, reloadMod, hipfireMod, recoilMod, durabilityMod);
    }
    public void SetReloadMod(float amount)
    {
        reloadMod = amount;
        record.CheckModifications(magCapacityMod, damageMod, rangeMod, reloadMod, hipfireMod, recoilMod, durabilityMod);
    }
    public void SetHipfireMod(float amount)
    {
        hipfireMod = amount;
        record.CheckModifications(magCapacityMod, damageMod, rangeMod, reloadMod, hipfireMod, recoilMod, durabilityMod);
    }
    public void SetRecoilMod(float amount)
    {
        recoilMod = amount;
        record.CheckModifications(magCapacityMod, damageMod, rangeMod, reloadMod, hipfireMod, recoilMod, durabilityMod);
    }
    public void SetDurabilityMod(int amount)
    {
        durabilityMod = amount;
        record.CheckModifications(magCapacityMod, damageMod, rangeMod, reloadMod, hipfireMod, recoilMod, durabilityMod);
    }
    public void SetType(SmithingUpgradeType mod)
    {
        type = mod;
    }
    public void SetName(string str)
    {
        name = str;
    }
    public SmithingUpgrades()
    {
        record = new();
        SetName("Default Initialization");
        SetMagCapacityMod(0);
        SetDamageMod(0);
        SetRangeMod(0);
        SetReloadMod(0);
        SetHipfireMod(0);
        SetRecoilMod(0);
        SetDurabilityMod(0);
        SetCatridge(CatridgeType.Standard);
    }
    public SmithingUpgrades(string name, int magCap, float damage, float range, float reload, float hipfire, float recoil, int durability, CatridgeType catridge)
    {
        SetName(name);
        SetMagCapacityMod(magCap);
        SetDamageMod(damage);
        SetRangeMod(range);
        SetReloadMod(reload);
        SetHipfireMod(hipfire);
        SetRecoilMod(recoil);
        SetDurabilityMod(durability);
        SetCatridge(catridge);
        record = new();
    }
    #endregion
};
[System.Serializable]
public enum SmithingUpgradeType
{
    MagazineCapacity = 1,
    Damage = 2,
    Range = 3,
    Catridge = 4,
    Durability = 5,
    Reload = 6,
    Hipfire = 7,
    Recoil = 8
}
[System.Serializable]
public enum CatridgeType
{
    Standard = 1,
    HighPowered = 2,
    ArmorPiercing = 3,
    AntiMaterial = 4,
    Sonic = 5,
    Phosphorus = 6,
    HighCaliber = 7
};
[System.Serializable]
public class ModificationRecord
{
    public bool m_Damage;
    public bool m_MagazineCapacity;
    public bool m_Range;
    public bool m_Catridge;
    public bool m_Durability;
    public bool m_Reload;
    public bool m_Hipfire;
    public bool m_Recoil;
    public void CheckModifications(int magCapacity, float damage, float range, float reload, float hipfire, float recoil, int durability)
    {
        if (magCapacity != 0) m_MagazineCapacity = true;
        else m_MagazineCapacity = false;
        if (damage != 0) m_Damage = true;
        else m_Damage = false;
        if (range != 0) m_Range = true;
        else m_Range = false;
        if (reload != 0) m_Reload = true;
        else m_Reload = false;
        if (hipfire != 0) m_Hipfire = true;
        else m_Hipfire = false;
        if (recoil != 0) m_Recoil = true;
        else m_Recoil = false;
        if (durability != 0) m_Durability = true;
        else m_Durability = false;
    }
    public ModificationRecord()
    {
        CheckModifications(0, 0, 0, 0, 0, 0, 0);
    }
}

[System.Serializable]
public struct SmithingResources
{
    public int iron;
    public int mechanisms;
    public int composites;
    public int tools;
    public int experience;
}
#endregion