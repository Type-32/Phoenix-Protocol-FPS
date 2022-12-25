using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace LauncherManifest
{
    public static class LauncherConfig
    {
        public static string CloudReleaseRepo
        {
            get
            {
                return "https://repo.smartsheep.space/api/v1/repos/CRTL_Prototype_Studios/Project_Phoenix_Files/releases";
            }
        }
        public static string CloudVersionFetchKey
        {
            get
            {
                return "name";
            }
        }
        public static string LauncherFolderPath
        {
            get
            {
                string tmp = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();
                return tmp;
            }
        }
        public static string LauncherVersionFilePath
        {
            get
            {
                string tmp = Path.Combine(LauncherFolderPath, "Version.txt");
                return tmp;
            }
        }
        public static bool CompareLocalVersionWithCache()
        {
            string localCacheVersion = PlayerPrefs.GetString("CachedLocalVersion");
            Version temp = new Version(localCacheVersion);
            return !temp.IsDifferentThan(LocalLaunchedClient.LocalGameVersion);
        }
        public static void UpdateCachedLocalVersion()
        {
            PlayerPrefs.SetString("CachedLocalVersion", LocalLaunchedClient.LocalGameVersion.ToString());
        }
    }
    public static class LocalLaunchedClient
    {
        public static Version LocalGameVersion
        {
            get
            {
                string tmp = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(), "Version.txt");
                Debug.Log("Getting Local Game Version from: " + Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(), "Version.txt"));
                return new Version((File.Exists(tmp) ? File.ReadAllText(tmp) : "0.0.0.unknown-version"));
            }
        }
    }

    public struct Version
    {
        internal static Version zero = new Version(0, 0, 0, "");
        private short major;
        private short minor;
        private short subMinor;
        private string verIndex;

        internal Version(short _major, short _minor, short _subMinor, string _verIndex)
        {
            major = _major;
            minor = _minor;
            subMinor = _subMinor;
            verIndex = _verIndex;
        }
        internal Version(string _version)
        {
            string[] _versionStrings = _version.Split('.');
            if (_versionStrings.Length != 4)
            {
                major = 0;
                minor = 0;
                subMinor = 0;
                verIndex = "rc0";
                return;
            }
            major = short.Parse(_versionStrings[0]);
            minor = short.Parse(_versionStrings[1]);
            subMinor = short.Parse(_versionStrings[2]);
            verIndex = _versionStrings[3];
        }
        internal bool IsDifferentThan(Version _otherVersion)
        {
            if (major != _otherVersion.major)
            {
                return true;
            }
            else
            {
                if (minor != _otherVersion.minor)
                {
                    return true;
                }
                else
                {
                    if (subMinor != _otherVersion.subMinor)
                    {
                        return true;
                    }
                    else
                    {
                        if (verIndex != _otherVersion.verIndex)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public override string ToString()
        {
            return $"{major}.{minor}.{subMinor}.{verIndex}";
        }
    }
}
namespace UserConfiguration
{
    public enum WeaponValidation
    {
        NoRegistry,
        FalseUnlockRegistry,
        ErrorUnlockRegistry,
        Valid
    }
    public static class WeaponSystem
    {
        public static WeaponValidation ValidateWeapon(int weaponIndex, bool correctValidation)
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
            string loadout = File.ReadAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"));
            UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
            LoadoutDataJSON loadoutJsonData = JsonUtility.FromJson<LoadoutDataJSON>(loadout);
            WeaponValidation result = WeaponValidation.Valid;
            if (!jsonData.shopData.availableWeaponIndexes.Contains(weaponIndex))
            {
                if (!jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex))
                {
                    if (!jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex))
                    {
                        Debug.Log("Returned " + WeaponValidation.NoRegistry.ToString());
                        if (GlobalDatabase.singleton.allWeaponDatas[weaponIndex].unlockingLevel > UserSystem.LocalUserLevel)
                        {
                            if (correctValidation)
                            {
                                jsonData.shopData.availableWeaponIndexes.Add(weaponIndex);
                            }
                        }
                        else
                        {
                            if (correctValidation)
                            {
                                jsonData.shopData.unlockedWeaponIndexes.Add(weaponIndex);
                            }
                        }
                        result = WeaponValidation.NoRegistry;
                    }
                    else
                    {
                        if (GlobalDatabase.singleton.allWeaponDatas[weaponIndex].unlockingLevel > UserSystem.LocalUserLevel)
                        {
                            if (correctValidation)
                            {
                                if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex)) jsonData.shopData.ownedWeaponIndexes.Remove(weaponIndex);
                                if (jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex)) jsonData.shopData.unlockedWeaponIndexes.Remove(weaponIndex);
                                jsonData.shopData.availableWeaponIndexes.Add(weaponIndex);
                                for (int i = 0; i < loadoutJsonData.Slots.Length; i++)
                                {
                                    if (loadoutJsonData.Slots[i].Weapon1 == weaponIndex)
                                    {
                                        loadoutJsonData.Slots[i].Weapon1 = 0;
                                    }
                                    if (loadoutJsonData.Slots[i].Weapon2 == weaponIndex)
                                    {
                                        loadoutJsonData.Slots[i].Weapon2 = 2;
                                    }
                                }
                            }
                            result = WeaponValidation.FalseUnlockRegistry;
                        }
                    }
                }
                else
                {
                    if (GlobalDatabase.singleton.allWeaponDatas[weaponIndex].unlockingLevel > UserSystem.LocalUserLevel)
                    {
                        if (correctValidation)
                        {
                            Debug.Log("Owned Index Count Before " + jsonData.shopData.ownedWeaponIndexes.Count);
                            if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex)) jsonData.shopData.ownedWeaponIndexes.Remove(weaponIndex);
                            if (jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex)) jsonData.shopData.unlockedWeaponIndexes.Remove(weaponIndex);
                            jsonData.shopData.availableWeaponIndexes.Add(weaponIndex);
                            //UserDatabase.Instance.WriteInputDataToJSON(jsonData);
                            for (int i = 0; i < loadoutJsonData.Slots.Length; i++)
                            {
                                if (loadoutJsonData.Slots[i].Weapon1 == weaponIndex)
                                {
                                    loadoutJsonData.Slots[i].Weapon1 = 0;
                                }
                                if (loadoutJsonData.Slots[i].Weapon2 == weaponIndex)
                                {
                                    loadoutJsonData.Slots[i].Weapon2 = 2;
                                }
                            }
                            Debug.Log("Owned Index Count After " + jsonData.shopData.ownedWeaponIndexes.Count);
                        }
                        result = WeaponValidation.FalseUnlockRegistry;
                    }
                }
            }
            else
            {
                if (GlobalDatabase.singleton.allWeaponDatas[weaponIndex].unlockingLevel > UserSystem.LocalUserLevel)
                {
                    if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex) || jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex))
                    {
                        bool flag = false;
                        if (correctValidation)
                        {
                            if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex)) { jsonData.shopData.ownedWeaponIndexes.Remove(weaponIndex); flag = true; }
                            if (jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex)) { jsonData.shopData.unlockedWeaponIndexes.Remove(weaponIndex); flag = true; }
                            for (int i = 0; i < loadoutJsonData.Slots.Length; i++)
                            {
                                if (loadoutJsonData.Slots[i].Weapon1 == weaponIndex)
                                {
                                    loadoutJsonData.Slots[i].Weapon1 = 0;
                                }
                                if (loadoutJsonData.Slots[i].Weapon2 == weaponIndex)
                                {
                                    loadoutJsonData.Slots[i].Weapon2 = 2;
                                }
                            }
                        }
                        result = (!flag ? WeaponValidation.FalseUnlockRegistry : WeaponValidation.ErrorUnlockRegistry);
                    }
                }
            }
            if (correctValidation) { UserSystem.WriteToUserConfig(jsonData); WriteToLoadoutConfig(loadoutJsonData); }
            Debug.Log("Returned " + result.ToString());
            return result;
        }
        public static void WriteToLoadoutConfig(LoadoutDataJSON loadoutData)
        {
            Debug.Log("Correcting Weapon Validation");
            string json = JsonUtility.ToJson(loadoutData, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"), json);
        }
        public static void WriteToLoadoutConfig(string jsonString)
        {
            Debug.Log("Correcting Weapon Validation");
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"), jsonString);
        }
        public static LoadoutDataJSON LoadoutJsonData
        {
            get
            {
                string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"));
                LoadoutDataJSON jsonData = JsonUtility.FromJson<LoadoutDataJSON>(json);
                return jsonData;
            }
        }
    }
    public static class UserSystem
    {
        [Tooltip("Read only. Returns the Locally Saved User XP Level.")]
        public static int LocalUserLevel
        {
            get
            {
                string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
                UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
                return jsonData.userLevel;
            }
        }
        public static UserDataJSON UserJsonData
        {
            get
            {
                string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
                UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
                return jsonData;
            }
        }
        public static void WriteToUserConfig(UserDataJSON userData)
        {
            Debug.Log("Correcting Weapon Validation");
            string json = JsonUtility.ToJson(userData, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"), json);
        }
        public static void WriteToUserConfig(string jsonString)
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"), jsonString);
        }
        public static string UserDataConfigKey
        {
            get
            {
                return "UserDataConfig.json";
            }
        }
        public static string LoadoutDataConfigKey
        {
            get
            {
                return "LoadoutDataConfig.json";
            }
        }
        public static string RewardDataConfigKey
        {
            get
            {
                return "RewardConfig.json";
            }
        }
        public static string SettingsOptionsKey
        {
            get
            {
                return "SettingsOptions.json";
            }
        }
        public static string AppearancesConfigKey
        {
            get
            {
                return "AppearancesConfig.json";
            }
        }
    }
    public static class Database
    {
        public static WeaponData FindWeaponData(int index)
        {
            if (index < GlobalDatabase.singleton.allWeaponDatas.Count)
            {
                return GlobalDatabase.singleton.allWeaponDatas[index];
            }
            return null;
        }
        public static int FindWeaponDataIndex(WeaponData data)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allWeaponDatas[i] == data) return i;
            }
            return -1;
        }
        public static EquipmentData FindEquipmentData(int index)
        {
            if (index < GlobalDatabase.singleton.allWeaponDatas.Count)
            {
                return GlobalDatabase.singleton.allEquipmentDatas[index];
            }
            return null;
        }
        public static int FindEquipmentDataIndex(EquipmentData data)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allEquipmentDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allEquipmentDatas[i] == data) return i;
            }
            return -1;
        }
        public static int FindAttachmentIndex(WeaponAttachmentData data)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponAttachmentDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allWeaponAttachmentDatas[i] == data) return i;
            }
            return -1;
        }
        public static WeaponAttachmentData FindAttachment(int index)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponAttachmentDatas.Count; i++)
            {
                if (i == index) return GlobalDatabase.singleton.allWeaponAttachmentDatas[i];
            }
            return null;
        }
        public static int FindLoadoutDataIndex(LoadoutData data)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allLoadoutDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allLoadoutDatas[i] == data) return i;
            }
            return -1;
        }
        public static LoadoutData FindLoadoutData(int index)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allLoadoutDatas.Count; i++)
            {
                if (i == index) return GlobalDatabase.singleton.allLoadoutDatas[i];
            }
            return null;
        }
        public static int FindWeaponAppearanceDataIndex(WeaponAppearanceMeshData data)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponAppearanceDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allWeaponAppearanceDatas[i] == data) return i;
            }
            return -1;
        }
        public static WeaponAppearanceMeshData FindWeaponAppearanceData(int index)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponAppearanceDatas.Count; i++)
            {
                if (i == index) return GlobalDatabase.singleton.allWeaponAppearanceDatas[i];
            }
            return null;
        }
        public static int FindPlayerCosmeticDataIndex(PlayerCosmeticData data)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allPlayerCosmeticDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allPlayerCosmeticDatas[i] == data) return i;
            }
            return -1;
        }
        public static PlayerCosmeticData FindPlayerCosmeticData(int index)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allPlayerCosmeticDatas.Count; i++)
            {
                if (i == index) return GlobalDatabase.singleton.allPlayerCosmeticDatas[i];
            }
            return null;
        }
    }
    public static class CosmeticSystem
    {
        public static string AppearancesConfigFilePath
        {
            get
            {
                string path = Path.Combine(Application.persistentDataPath, "AppearancesConfig.json");
                if (File.Exists(path)) return path;
                else
                {
                    File.Create(path).Close();
                }
                return null;
            }
        }
        public static void WriteToConfig(AppearancesDataJSON appearancesDataJSON)
        {
            string json = JsonUtility.ToJson(appearancesDataJSON, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey), json);
        }
        public static void WriteToConfig(string jsonString)
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey), jsonString);
        }
        public static AppearancesDataJSON AppearancesJsonData
        {
            get
            {
                string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey));
                AppearancesDataJSON jsonData = JsonUtility.FromJson<AppearancesDataJSON>(json);
                return jsonData;
            }
        }
    }
    public static class ShopSystem
    {
        public static string RewardConfigFilePath
        {
            get
            {
                string path = Path.Combine(Application.persistentDataPath, "RewardConfig.json");
                if (File.Exists(path)) return path;
                else
                {
                    File.Create(path).Close();
                }
                return null;
            }
        }
        public static bool AddRewardCode(string rewardCode = null)
        {
            if (rewardCode != null)
            {

            }
            else
            {

            }
            return true;
        }
    }
}