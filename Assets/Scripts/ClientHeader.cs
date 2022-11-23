using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace LauncherManifest
{
    public static class LauncherConfig {
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
    public static class LocalLaunchedClient{
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
                        if(verIndex != _otherVersion.verIndex)
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