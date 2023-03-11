using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System.IO;
using PrototypeLib.Modules.FileOperations.IO;
namespace InfoTypes
{
    namespace InRoomPreview
    {
        using UnityEngine.UI;
        public struct MapPreviewInfo
        {
            public string mapName;
            public Sprite mapIcon;
            internal MapPreviewInfo(string n, Sprite i)
            {
                mapName = n;
                mapIcon = i;
            }
        }
        public struct StatisticsPreviewInfo
        {
            public string gamemode;
            public int maxPlayers;
            public int roomCode;
            public bool visibility;
            internal StatisticsPreviewInfo(MenuManager.Gamemodes gm, int mp, int rc, bool v)
            {
                gamemode = "";
                switch (gm)
                {
                    case MenuManager.Gamemodes.FFA:
                        gamemode = "Free For All";
                        break;
                    case MenuManager.Gamemodes.TDM:
                        gamemode = "Team Deathmatch";
                        break;
                    case MenuManager.Gamemodes.CTF:
                        gamemode = "Capture The Flag";
                        break;
                    case MenuManager.Gamemodes.DZ:
                        gamemode = "Drop Zones";
                        break;
                }
                maxPlayers = mp;
                roomCode = rc;
                visibility = v;
            }
            internal StatisticsPreviewInfo(string gm, int mp, int rc, bool v)
            {
                gamemode = gm;
                maxPlayers = mp;
                roomCode = rc;
                visibility = v;
            }
        }
        public struct LoadoutPreviewInfo
        {
            public string w_Name1, w_Name2;
            public Sprite w_Icon1, w_Icon2, e_Icon1, e_Icon2;
            internal LoadoutPreviewInfo(WeaponData w1, WeaponData w2, EquipmentData e1, EquipmentData e2)
            {
                w_Name1 = "Primary - " + w1.itemName;
                w_Name2 = "Secondary - " + w2.itemName;
                w_Icon1 = w1.itemIcon;
                w_Icon2 = w2.itemIcon;
                e_Icon1 = e1.itemIcon;
                e_Icon2 = e2.itemIcon;
            }
        }
    }
}
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
    public class ParameterData
    {
        public string identifier;
        public enum ParameterType
        {
            Null,
            Integer,
            Float,
            Boolean,
            String
        }
        public ParameterType type;
        public Type reference;
        public object value;
        public ParameterData(string identifier, ParameterType type)
        {
            this.identifier = identifier;
            this.type = type;
            switch (type)
            {
                case ParameterType.Integer:
                    reference = typeof(int);
                    break;
                case ParameterType.Boolean:
                    reference = typeof(bool);
                    break;
                case ParameterType.Float:
                    reference = typeof(float);
                    break;
                case ParameterType.String:
                    reference = typeof(string);
                    break;
            }

        }
        public ParameterData()
        {
            identifier = "-null";
            type = ParameterType.Null;
            switch (type)
            {
                case ParameterType.Integer:
                    reference = typeof(int);
                    break;
                case ParameterType.Boolean:
                    reference = typeof(bool);
                    break;
                case ParameterType.Float:
                    reference = typeof(float);
                    break;
                case ParameterType.String:
                    reference = typeof(string);
                    break;
            }
        }
    }
    public static class LauncherParametersDatabase
    {
        public static ParameterData Username_ID { get { return new("-username", ParameterData.ParameterType.String); } }
    }
    public struct LauncherParameters
    {
        internal static LauncherParameters empty;
        private List<ParameterData> args;
        internal LauncherParameters(List<ParameterData> paramArgs)
        {
            args = new();
            args = paramArgs;
        }
        internal LauncherParameters(string param)
        {
            args = new();
            int st = 0;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] == '-') st = i;
                if (param[i] == ' ')
                {
                    int gt = 0;
                    string det = "", par = "";
                    ParameterData pd = new();
                    for (int j = st; j < i - 1; j++)
                    {
                        if (param[i] == ':') gt = i;
                        else
                        {
                            if (gt != 0)
                            {
                                det += param[i];
                            }
                            else
                            {
                                par += param[i];
                            }
                        }
                    }
                    if (det.Length <= 0 || gt == 0)
                    {
                        pd.reference = null;
                        pd.type = ParameterData.ParameterType.Null;
                        pd.value = null;
                    }
                    else
                    {
                        if (det.Contains('.'))
                        {
                            bool c_ch = false;
                            for (int g = 0; g < det.Length; g++)
                            {
                                if (g >= 'a' && g <= 'z')
                                {
                                    c_ch = true;
                                    break;
                                }
                            }
                            if (c_ch)
                            {
                                pd.reference = typeof(float);
                                pd.type = ParameterData.ParameterType.Float;
                                pd.value = float.Parse(det);
                            }
                        }
                        else
                        {
                            bool c_ch = false;
                            for (int g = 0; g < det.Length; g++)
                            {
                                if (g >= 'a' && g <= 'z')
                                {
                                    c_ch = true;
                                    break;
                                }
                            }
                            if (c_ch)
                            {
                                pd.reference = typeof(string);
                                pd.type = ParameterData.ParameterType.String;
                                pd.value = det;
                            }
                            else if (det == "true" || det == "false")
                            {
                                pd.reference = typeof(bool);
                                pd.type = ParameterData.ParameterType.Boolean;
                                pd.value = bool.Parse(det);
                            }
                            else
                            {
                                pd.reference = typeof(int);
                                pd.type = ParameterData.ParameterType.Integer;
                                pd.value = int.Parse(det);
                            }
                        }
                    }
                    if (par.Length > 1)
                    {
                        pd.identifier = par;
                    }
                    else
                    {
                        continue;
                    }
                    args.Add(pd);
                }
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
            UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
            LoadoutDataJSON loadoutJsonData = FileOps<LoadoutDataJSON>.ReadFile(UserSystem.LoadoutDataPath);
            // AppearancesDataJSON appearanceData = FileOps<AppearancesDataJSON>.ReadFile(UserSystem.AppearancesConfigPath);
            WeaponValidation result = WeaponValidation.Valid;
            if (!jsonData.shopData.availableWeaponIndexes.Contains(weaponIndex))
            {
                if (!jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex))
                {
                    if (!jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex))
                    {
                        Debug.Log("Returned " + WeaponValidation.NoRegistry.ToString());
                        if (GlobalDatabase.Instance.allWeaponDatas[weaponIndex].unlockingLevel > UserSystem.LocalUserLevel)
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
                        if (GlobalDatabase.Instance.allWeaponDatas[weaponIndex].unlockingLevel > UserSystem.LocalUserLevel)
                        {
                            if (correctValidation)
                            {
                                if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex)) jsonData.shopData.ownedWeaponIndexes.Remove(weaponIndex);
                                if (jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex)) jsonData.shopData.unlockedWeaponIndexes.Remove(weaponIndex);
                                jsonData.shopData.availableWeaponIndexes.Add(weaponIndex);
                                for (int i = 0; i < loadoutJsonData.Slots.Count; i++)
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
                    if (GlobalDatabase.Instance.allWeaponDatas[weaponIndex].unlockingLevel > UserSystem.LocalUserLevel)
                    {
                        if (correctValidation)
                        {
                            Debug.Log("Owned Index Count Before " + jsonData.shopData.ownedWeaponIndexes.Count);
                            if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex)) jsonData.shopData.ownedWeaponIndexes.Remove(weaponIndex);
                            if (jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex)) jsonData.shopData.unlockedWeaponIndexes.Remove(weaponIndex);
                            jsonData.shopData.availableWeaponIndexes.Add(weaponIndex);
                            //UserDatabase.Instance.WriteInputDataToJSON(jsonData);
                            for (int i = 0; i < loadoutJsonData.Slots.Count; i++)
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
                if (GlobalDatabase.Instance.allWeaponDatas[weaponIndex].unlockingLevel > UserSystem.LocalUserLevel)
                {
                    if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex) || jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex))
                    {
                        bool flag = false;
                        if (correctValidation)
                        {
                            if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex)) { jsonData.shopData.ownedWeaponIndexes.Remove(weaponIndex); flag = true; }
                            if (jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex)) { jsonData.shopData.unlockedWeaponIndexes.Remove(weaponIndex); flag = true; }
                            for (int i = 0; i < loadoutJsonData.Slots.Count; i++)
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
            if (correctValidation)
            {
                FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
                FileOps<LoadoutDataJSON>.WriteFile(loadoutJsonData, UserSystem.LoadoutDataPath);
            }
            Debug.Log("Returned " + result.ToString());
            return result;
        }
    }
    public static class UserSystem
    {
        public static int LocalUserLevel { get { return FileOps<UserDataJSON>.ReadFile(UserDataPath).userLevel; } }
        public static string UserDataConfigKey { get { return "UserDataConfig.json"; } }
        public static string LoadoutDataConfigKey { get { return "LoadoutDataConfig.json"; } }
        public static string RewardDataConfigKey { get { return "RewardConfig.json"; } }
        public static string SettingsOptionsKey { get { return "SettingsOptions.json"; } }
        public static string AppearancesConfigKey { get { return "AppearancesConfig.json"; } }
        public static string GunsmithConfigKey { get { return "GunsmithConfig.json"; } }
        public static string UserDataPath { get { return Path.Combine(Application.persistentDataPath, UserDataConfigKey); } }
        public static string LoadoutDataPath { get { return Path.Combine(Application.persistentDataPath, LoadoutDataConfigKey); } }
        public static string RewardDataPath { get { return Path.Combine(Application.persistentDataPath, RewardDataConfigKey); } }
        public static string SettingsOptionsPath { get { return Path.Combine(Application.persistentDataPath, SettingsOptionsKey); } }
        public static string AppearancesConfigPath { get { return Path.Combine(Application.persistentDataPath, AppearancesConfigKey); } }
        public static string GunsmithPath { get { return Path.Combine(Application.persistentDataPath, GunsmithConfigKey); } }
    }
    public static class Database
    {
        public static int FindWeaponDataIndex(WeaponData data)
        {
            for (int i = 0; i < GlobalDatabase.Instance.allWeaponDatas.Count; i++)
            {
                if (GlobalDatabase.Instance.allWeaponDatas[i] == data) return i;
            }
            return -1;
        }
        public static EquipmentData FindEquipmentData(int index)
        {
            if (index < GlobalDatabase.Instance.allWeaponDatas.Count)
            {
                return GlobalDatabase.Instance.allEquipmentDatas[index];
            }
            return null;
        }
        public static int FindEquipmentDataIndex(EquipmentData data)
        {
            for (int i = 0; i < GlobalDatabase.Instance.allEquipmentDatas.Count; i++)
            {
                if (GlobalDatabase.Instance.allEquipmentDatas[i] == data) return i;
            }
            return -1;
        }
    }
    public static class GunsmithSystem
    {
        public static GunsmithDataJSON.WeaponSmithingData FindWeaponSmithingData(int weaponIndex)
        {
            GunsmithDataJSON data = FileOps<GunsmithDataJSON>.ReadFile(UserSystem.GunsmithPath);
            for (int i = 0; i < data.weaponSmithings.Count; i++)
            {
                if (weaponIndex == data.weaponSmithings[i].weaponIndex) return data.weaponSmithings[i];
            }
            return null;
        }
        public static bool VerifyWeaponSmithingData(GunsmithDataJSON.WeaponSmithingData data, bool correctValidation = true)
        {
            GunsmithDataJSON temp = FileOps<GunsmithDataJSON>.ReadFile(UserSystem.GunsmithPath);
            if (FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).shopData.ownedWeaponIndexes.Contains(data.weaponIndex))
                return true;
            else
            {
                if (correctValidation)
                {
                    if (temp.weaponSmithings.Contains(data))
                    {
                        temp.weaponSmithings.Remove(data);
                        FileOps<GunsmithDataJSON>.WriteFile(temp, UserSystem.GunsmithPath);
                    }
                }
            }
            return false;
        }
        public static bool AddWeaponToData(WeaponData data)
        {
            bool success = false;
            if (FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).shopData.ownedWeaponIndexes.Contains(data.GlobalWeaponIndex))
            {
                if (FindWeaponSmithingData(data.GlobalWeaponIndex) == null)
                {
                    GunsmithDataJSON jsonData = FileOps<GunsmithDataJSON>.ReadFile(UserSystem.GunsmithPath);
                    jsonData.weaponSmithings.Add(new GunsmithDataJSON.WeaponSmithingData(data.GlobalWeaponIndex));
                    FileOps<GunsmithDataJSON>.WriteFile(jsonData, UserSystem.GunsmithPath);
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
                GunsmithDataJSON jsonData = FileOps<GunsmithDataJSON>.ReadFile(UserSystem.GunsmithPath);
                jsonData.weaponSmithings.Remove(temp);
                FileOps<GunsmithDataJSON>.WriteFile(jsonData, UserSystem.GunsmithPath);
                success = true;
            }
            return success;
        }
    }
    public static class CosmeticSystem
    {
        public static bool VerifyWeaponAppearanceData(WeaponAppearanceMeshData data, bool correctValidation = true)
        {
            WeaponAppearance temp = new WeaponAppearance(data);
            AppearancesDataJSON jsonData = FileOps<AppearancesDataJSON>.ReadFile(UserSystem.AppearancesConfigPath);
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
                    isValid = true;
                else
                {
                    jsonData.availableWeaponAppearances.Add(temp);
                    if (correctValidation) FileOps<AppearancesDataJSON>.WriteFile(jsonData, UserSystem.AppearancesConfigPath);
                }
            }
            return isValid;
        }
        public static bool ValidateLoadoutCosmetics(bool correctValidation = true)
        {
            bool isValid = true;
            LoadoutDataJSON loadoutJsonData = FileOps<LoadoutDataJSON>.ReadFile(UserSystem.LoadoutDataPath);
            AppearancesDataJSON appearanceData = FileOps<AppearancesDataJSON>.ReadFile(UserSystem.AppearancesConfigPath);
            for (int i = 0; i < 8; i++)
            {
                WeaponAppearance t1 = new();
                WeaponAppearance t2 = new();
                //Debug.LogWarning("Slots Length: " + loadoutJsonData.Slots.Length);
                if (loadoutJsonData.Slots[i].WeaponSkin1 != -1) t1 = new(GlobalDatabase.Instance.allWeaponAppearanceDatas[loadoutJsonData.Slots[i].WeaponSkin1]);
                if (loadoutJsonData.Slots[i].WeaponSkin2 != -1) t2 = new(GlobalDatabase.Instance.allWeaponAppearanceDatas[loadoutJsonData.Slots[i].WeaponSkin2]);
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
                FileOps<AppearancesDataJSON>.WriteFile(appearanceData, UserSystem.AppearancesConfigPath);
                FileOps<LoadoutDataJSON>.WriteFile(loadoutJsonData, UserSystem.LoadoutDataPath);
            }
            return isValid;
        }
        public static WeaponAppearanceMeshData FindWeaponAppearanceMeshData(WeaponAppearance appearance)
        {
            return FindWeaponAppearanceMeshData(appearance.appearanceIndex, appearance.weaponIndex);
        }
        public static WeaponAppearanceMeshData FindWeaponAppearanceMeshData(int appearanceIndex, int weaponIndex)
        {
            for (int i = 0; i < GlobalDatabase.Instance.allWeaponAppearanceDatas.Count; i++)
            {
                if (i == appearanceIndex && GlobalDatabase.Instance.allWeaponAppearanceDatas[i].weaponData.GlobalWeaponIndex == weaponIndex) return GlobalDatabase.Instance.allWeaponAppearanceDatas[i];
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