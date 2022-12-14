using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using LauncherManifest;
using UserConfiguration;

public class UserDatabase : MonoBehaviour
{
    public static UserDatabase Instance;
    public UserDataJSON emptyUserDataJSON;
    public int levelLimiter = 800;
    private void Awake()
    {
        Instance = this;
        levelLimiter = 800;
    }
    private void Start()
    {
        ReadUserDataFromJSON();
    }
    public void ReadUserDataFromJSON()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey))) InitializeUserDataToJSON();
        if (File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey)))
        {
            string tempJson = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey));
            if (string.IsNullOrEmpty(tempJson) || string.IsNullOrWhiteSpace(tempJson))
            {
                InitializeUserDataToJSON();
            }
        }
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey));
        //Debug.LogWarning("Reading User Data To Files...");
        UserDataJSON jsonData = emptyUserDataJSON;
        jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        MenuManager.instance.SetUserGUIData(PlayerPrefs.GetString("Username"), jsonData.userLevel, (float)jsonData.userLevelXP, jsonData.userCoins);
        Debug.Log((jsonData.userLevelXP / (jsonData.userLevel * UserDatabase.Instance.levelLimiter)));
        if (!jsonData.hasInitialized)
        {
            string content = "";
            content = "You have unlocked your first three weapons:\n-AK-47\n-M16\n-Beretta\nYou can go equip them in your loadouts now.";
            MenuManager.instance.AddModalWindow("Unlocked", content);
            jsonData.hasInitialized = true;
            WriteInputDataToJSON(jsonData);
        }
    }
    public void WriteUserDataToJSON()
    {
        UserDataJSON data = new();
        data = emptyUserDataJSON;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey), json);
        //Debug.LogWarning("Writing User Data To Files...");
    }
    public void WriteInputDataToJSON(UserDataJSON data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey), json);
        //Debug.LogWarning("Writing User Data To Files...");
    }
    public void InitializeUserDataToJSON()
    {
        UserDataJSON data = new();
        data = emptyUserDataJSON;

        string json = JsonUtility.ToJson(data, true);
        if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey))) File.CreateText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey)).Close();
        File.WriteAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey), json);
        //Debug.LogWarning("Initializing User Data To Files...");
    }
    public void AddUserCurrency(int amount)
    {
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey));
        UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        jsonData.userCoins += amount;
        if (RoomManager.Instance.currentSceneIndex == 0)
        {
            MenuManager.instance.UpdateCoin(jsonData.userCoins);
        }
        WriteInputDataToJSON(jsonData);
    }
    public bool AddUserLevelXP(int amount)
    {
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey));
        UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        int levelLim = jsonData.userLevel * UserDatabase.Instance.levelLimiter;
        string unlockedContent = "";
        bool ret = false;
        if (jsonData.userLevelXP + amount >= levelLim)
        {
            ret = true;
            jsonData.userLevelXP = jsonData.userLevelXP + amount - levelLim;
            jsonData.userLevel++;
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allWeaponDatas[i].unlockingLevel <= jsonData.userLevel)
                {
                    unlockedContent = unlockedContent + "-" + GlobalDatabase.singleton.allWeaponDatas[i].itemName + "\n";
                }
            }
            if (RoomManager.Instance.currentSceneIndex != 0)
            {
                MenuManager.PopupData tmp;
                tmp.title = "Level Up";
                tmp.content = "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent);
                tmp.queueType = MenuManager.PopupQueue.OnMainMenuLoad;
                if (!MenuManager.instance.queuedModalWindows.Contains(tmp)) MenuManager.instance.QueueModalWindow(tmp.title, tmp.content, MenuManager.PopupQueue.OnMainMenuLoad);
            }
            else MenuManager.instance.AddModalWindow("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent));
            WriteInputDataToJSON(jsonData);
        }
        else
        {
            if (RoomManager.Instance.currentSceneIndex != 0) MenuManager.instance.QueueModalWindow("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent), MenuManager.PopupQueue.OnMainMenuLoad);
            else MenuManager.instance.AddModalWindow("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent));
            jsonData.userLevelXP += amount;
            WriteInputDataToJSON(jsonData);
        }
        return ret;
    }
    public int GetUserXPValue()
    {
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey));
        UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        return jsonData.userLevelXP;
    }
    public int GetUserXPLevelValue()
    {
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey));
        UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        return jsonData.userLevel;
    }
    public int GetUserCoinValue()
    {
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.UserDataConfigKey));
        UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        return jsonData.userCoins;
    }
}
