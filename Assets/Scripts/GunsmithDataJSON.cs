using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GunsmithDataJSON
{
    [System.Serializable]
    public struct WeaponSmithingData
    {
        #region Original Values
        public float o_Damage
        {
            get
            {
                return GlobalDatabase.singleton.allWeaponDatas[weaponIndex].damage;
            }
        }
        public int o_MagazineCapacity
        {
            get
            {
                return GlobalDatabase.singleton.allWeaponDatas[weaponIndex].maxAmmoPerMag;
            }
        }
        public float o_Range
        {
            get
            {
                return GlobalDatabase.singleton.allWeaponDatas[weaponIndex].range;
            }
        }
        public CatridgeType o_Catridge
        {
            get
            {
                return CatridgeType.Standard;
            }
        }
        public int o_Durability
        {
            get
            {
                return GlobalDatabase.singleton.allWeaponDatas[weaponIndex].maxDurability;
            }
        }
        public float o_Reload
        {
            get
            {
                return GlobalDatabase.singleton.allWeaponDatas[weaponIndex].reloadTime;
            }
        }
        public float o_Hipfire
        {
            get
            {
                return 0f;
            }
        }
        public float o_Recoil
        {
            get
            {
                return 0f;
            }
        }
        #endregion
        public int weaponIndex;
        public SmithingUpgrades upgrades;
    };
    [System.Serializable]
    public struct SmithingUpgrades
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
        #endregion
    };
    #region Backend Types
    [System.Serializable]
    public enum SmithingUpgradeType
    {
        MagazineCapacity,
        Damage,
        Range,
        Catridge,
        Durability,
        Reload,
        Hipfire,
        Recoil
    }
    [System.Serializable]
    public enum CatridgeType
    {
        Standard,
        HighPowered,
        ArmorPiercing,
        AntiMaterial,
        Sonic,
        Phosphorus,
        HighCaliber
    };
    [System.Serializable]
    public struct ModificationRecord
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
    }
    #endregion
    public List<WeaponSmithingData> weaponSmithings = new();
    internal GunsmithDataJSON()
    {
        weaponSmithings = new();
    }
}
