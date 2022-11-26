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
            UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
            if (!jsonData.shopData.availableWeaponIndexes.Contains(weaponIndex))
            {
                if (!jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex))
                {
                    if (!jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex))
                    {
                        return WeaponValidation.NoRegistry;
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
                            }
                            return WeaponValidation.FalseUnlockRegistry;
                        }
                    }
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
                        }
                        return WeaponValidation.FalseUnlockRegistry;
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
                        }
                        return (!flag ? WeaponValidation.FalseUnlockRegistry : WeaponValidation.ErrorUnlockRegistry);
                    }
                }
                else
                {
                    bool flag = false;
                    if (correctValidation)
                    {
                        if (jsonData.shopData.ownedWeaponIndexes.Contains(weaponIndex)) { jsonData.shopData.ownedWeaponIndexes.Remove(weaponIndex); flag = true; }
                        if (jsonData.shopData.unlockedWeaponIndexes.Contains(weaponIndex)) { jsonData.shopData.unlockedWeaponIndexes.Remove(weaponIndex); flag = true; }
                    }
                    return (!flag ? WeaponValidation.Valid : WeaponValidation.ErrorUnlockRegistry);
                }
            }
            if (correctValidation) WriteToUserConfig(jsonData);
            return WeaponValidation.Valid;
        }
        public static void WriteToUserConfig(UserDataJSON userData)
        {
            string json = JsonUtility.ToJson(userData, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"), json);
        }
        public static void WriteToUserConfig(string jsonString)
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "UserDataConfig.json"), jsonString);
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
    }
}