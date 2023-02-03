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
        public static WeaponValidation ValidateWeapon(WeaponData data, bool correctValidation)
        {
            return ValidateWeapon(data.GlobalWeaponIndex, correctValidation);
        }
        public static WeaponValidation ValidateWeapon(int weaponIndex, bool correctValidation)
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"));
            string loadout = File.ReadAllText(Path.Combine(Application.persistentDataPath, "LoadoutDataConfig.json"));
            UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
            LoadoutDataJSON loadoutJsonData = JsonUtility.FromJson<LoadoutDataJSON>(loadout);
            // AppearancesDataJSON appearanceData = CosmeticSystem.AppearancesJsonData;
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
        public static string GunsmithConfigKey
        {
            get
            {
                return "GunsmithConfig.json";
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
    public static class GunsmithSystem
    {
        public static string GunsmithConfigFilePath
        {
            get
            {
                string path = Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey);
                if (File.Exists(path)) return path;
                else File.Create(path).Close();
                return null;
            }
        }
        public static void WriteToConfig(GunsmithDataJSON gunsmithDataJSON)
        {
            string jsonString = JsonUtility.ToJson(gunsmithDataJSON, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey), jsonString);
        }
        public static void WriteToConfig(string jsonString)
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey), jsonString);
        }
        public static GunsmithDataJSON GunsmithJsonData
        {
            get
            {
                if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey)))
                {
                    GunsmithDataJSON data = new();
                    data = GlobalDatabase.singleton.emptyGunsmithDataJSON;

                    string jsonString = JsonUtility.ToJson(data, true);
                    if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey))) File.CreateText(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey)).Close();
                    File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey), jsonString);

                }
                if (File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey)))
                {
                    string tempJson = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey));
                    if (string.IsNullOrEmpty(tempJson) || string.IsNullOrWhiteSpace(tempJson))
                    {
                        GunsmithDataJSON data = new();
                        data = GlobalDatabase.singleton.emptyGunsmithDataJSON;

                        string jsonString = JsonUtility.ToJson(data, true);
                        if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey))) File.CreateText(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey)).Close();
                        File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey), jsonString);
                    }
                }
                string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.GunsmithConfigKey));
                GunsmithDataJSON jsonData = JsonUtility.FromJson<GunsmithDataJSON>(json);
                return jsonData;
            }
        }
        public static GunsmithDataJSON.WeaponSmithingData FindWeaponSmithingData(int weaponIndex)
        {
            GunsmithDataJSON data = GunsmithJsonData;
            for (int i = 0; i < data.weaponSmithings.Count; i++)
            {
                if (weaponIndex == data.weaponSmithings[i].weaponIndex) return data.weaponSmithings[i];
            }
            return null;
        }
        public static bool VerifyWeaponSmithingData(GunsmithDataJSON.WeaponSmithingData data, bool correctValidation = true)
        {
            if (UserSystem.UserJsonData.shopData.ownedWeaponIndexes.Contains(data.weaponIndex))
            {
                return true;
            }
            else
            {
                if (correctValidation)
                {
                    if (GunsmithJsonData.weaponSmithings.Contains(data))
                    {
                        GunsmithJsonData.weaponSmithings.Remove(data);
                        WriteToConfig(GunsmithJsonData);
                    }
                }
            }
            return false;
        }
        public static bool AddWeaponToData(WeaponData data)
        {
            bool success = false;
            if (UserSystem.UserJsonData.shopData.ownedWeaponIndexes.Contains(data.GlobalWeaponIndex))
            {
                if (FindWeaponSmithingData(data.GlobalWeaponIndex) == null)
                {
                    GunsmithDataJSON jsonData = GunsmithJsonData;
                    GunsmithDataJSON.WeaponSmithingData temp = new();
                    temp.weaponIndex = data.GlobalWeaponIndex;
                    jsonData.weaponSmithings.Add(temp);
                    WriteToConfig(jsonData);
                    success = true;
                }
            }
            return success;
        }
        public static bool RemoveWeaponFromData(WeaponData data)
        {
            bool success = false;
            GunsmithDataJSON.WeaponSmithingData temp = FindWeaponSmithingData(data.GlobalWeaponIndex);
            if (temp != null)
            {
                GunsmithDataJSON jsonData = GunsmithJsonData;
                jsonData.weaponSmithings.Remove(temp);
                WriteToConfig(jsonData);
                success = true;
            }
            return success;
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
                if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey)))
                {
                    AppearancesDataJSON data = new();
                    data = GlobalDatabase.singleton.emptyAppearancesDataJSON;

                    string jsonString = JsonUtility.ToJson(data, true);
                    if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey))) File.CreateText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey)).Close();
                    File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey), jsonString);

                }
                if (File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey)))
                {
                    string tempJson = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey));
                    if (string.IsNullOrEmpty(tempJson) || string.IsNullOrWhiteSpace(tempJson))
                    {
                        AppearancesDataJSON data = new();
                        data = GlobalDatabase.singleton.emptyAppearancesDataJSON;

                        string jsonString = JsonUtility.ToJson(data, true);
                        if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey))) File.CreateText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey)).Close();
                        File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey), jsonString);
                    }
                }
                string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.AppearancesConfigKey));
                AppearancesDataJSON jsonData = JsonUtility.FromJson<AppearancesDataJSON>(json);
                return jsonData;
            }
        }
        public static bool VerifyWeaponAppearanceData(int weaponAppearanceMeshDataIndex, bool correctValidation = true)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponAppearanceDatas.Count; i++)
            {
                if (weaponAppearanceMeshDataIndex == i) return VerifyWeaponAppearanceData(GlobalDatabase.singleton.allWeaponAppearanceDatas[i], correctValidation);
            }
            return false;
        }
        public static bool VerifyWeaponAppearanceData(WeaponAppearanceMeshData data, bool correctValidation = true)
        {
            AppearancesDataJSON.WeaponAppearance temp;
            AppearancesDataJSON jsonData = CosmeticSystem.AppearancesJsonData;
            temp.appearanceIndex = data.WeaponAppearanceMeshDataIndex;
            temp.weaponIndex = data.weaponData.GlobalWeaponIndex;
            bool isValid = false;
            if (jsonData.unlockedWeaponAppearances.Contains(temp))
            {
                isValid = true;
                if (jsonData.availableWeaponAppearances.Contains(temp))
                {
                    jsonData.availableWeaponAppearances.Remove(temp);
                    isValid = false;
                }
            }
            else
            {
                if (jsonData.availableWeaponAppearances.Contains(temp))
                {
                    isValid = true;
                }
                else
                {
                    jsonData.availableWeaponAppearances.Add(temp);
                    if (correctValidation) CosmeticSystem.WriteToConfig(jsonData);
                }
            }
            return isValid;
        }
        public static bool ValidateLoadoutCosmetics(bool correctValidation = true)
        {
            bool isValid = true;
            LoadoutDataJSON loadoutJsonData = WeaponSystem.LoadoutJsonData;
            AppearancesDataJSON appearanceData = CosmeticSystem.AppearancesJsonData;
            for (int i = 0; i < 8; i++)
            {
                AppearancesDataJSON.WeaponAppearance t1 = new();
                AppearancesDataJSON.WeaponAppearance t2 = new();
                //Debug.LogWarning("Slots Length: " + loadoutJsonData.Slots.Length);
                if (loadoutJsonData.Slots[i].WeaponSkin1 != -1) t1 = CosmeticSystem.RevertWeaponAppearanceMeshData(GlobalDatabase.singleton.allWeaponAppearanceDatas[loadoutJsonData.Slots[i].WeaponSkin1]);
                if (loadoutJsonData.Slots[i].WeaponSkin2 != -1) t2 = CosmeticSystem.RevertWeaponAppearanceMeshData(GlobalDatabase.singleton.allWeaponAppearanceDatas[loadoutJsonData.Slots[i].WeaponSkin2]);
                if (loadoutJsonData.Slots[i].WeaponSkin1 != -1)
                {
                    if (!appearanceData.unlockedWeaponAppearances.Contains(t1))
                    {
                        isValid = false;
                        loadoutJsonData.Slots[i].WeaponSkin1 = -1;
                        if (!appearanceData.availableWeaponAppearances.Contains(t1))
                        {
                            appearanceData.availableWeaponAppearances.Add(t1);
                        }
                    }
                    else
                    {
                        if (appearanceData.availableWeaponAppearances.Contains(t1))
                        {
                            isValid = false;
                            appearanceData.availableWeaponAppearances.Remove(t1);
                        }
                    }
                }

                if (loadoutJsonData.Slots[i].WeaponSkin2 != -1)
                {
                    if (!appearanceData.unlockedWeaponAppearances.Contains(t2))
                    {
                        isValid = false;
                        loadoutJsonData.Slots[i].WeaponSkin2 = -1;
                        if (!appearanceData.availableWeaponAppearances.Contains(t2))
                        {
                            appearanceData.availableWeaponAppearances.Add(t2);
                        }
                    }
                    else
                    {
                        if (appearanceData.availableWeaponAppearances.Contains(t2))
                        {
                            isValid = false;
                            appearanceData.availableWeaponAppearances.Remove(t2);
                        }
                    }
                }
            }
            if (correctValidation)
            {
                CosmeticSystem.WriteToConfig(appearanceData);
                WeaponSystem.WriteToLoadoutConfig(loadoutJsonData);
            }
            return isValid;
        }
        public static AppearancesDataJSON.WeaponAppearance RevertWeaponAppearanceMeshData(WeaponAppearanceMeshData data)
        {
            AppearancesDataJSON.WeaponAppearance temp;
            temp.appearanceIndex = data.WeaponAppearanceMeshDataIndex;
            temp.weaponIndex = data.weaponData.GlobalWeaponIndex;
            return temp;
        }
        public static WeaponAppearanceMeshData FindWeaponAppearanceMeshData(AppearancesDataJSON.WeaponAppearance appearance)
        {
            return FindWeaponAppearanceMeshData(appearance.appearanceIndex, appearance.weaponIndex);
        }
        public static WeaponAppearanceMeshData FindWeaponAppearanceMeshData(int appearanceIndex, int weaponIndex)
        {
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponAppearanceDatas.Count; i++)
            {
                if (i == appearanceIndex && GlobalDatabase.singleton.allWeaponAppearanceDatas[i].weaponData.GlobalWeaponIndex == weaponIndex) return GlobalDatabase.singleton.allWeaponAppearanceDatas[i];
            }
            return null;
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