using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UserDatabase : MonoBehaviour
{
    public static UserDatabase Instance;
    public UserDataJSON emptyUserDataJSON;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ReadUserDataFromJSON();
    }
    public void ReadUserDataFromJSON()
    {
        if (!File.Exists(Application.persistentDataPath + "/UserDataConfig.json")) InitializeUserDataToJSON();
        string json = File.ReadAllText(Application.persistentDataPath + "/UserDataConfig.json");
        Debug.LogWarning("Reading User Data To Files...");
        UserDataJSON jsonData = emptyUserDataJSON;
        jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        //Debug.Log(jsonData.userLevel);
        MainMenuUIManager.instance.SetUserGUIData(PlayerPrefs.GetString("Username"), jsonData.userLevel, (float)jsonData.userLevelXP, jsonData.userCoins);
        Debug.Log((jsonData.userLevelXP / (jsonData.userLevel * 500)));
        if (!jsonData.hasInitialized)
        {
            string content = "";
            content = "You have unlocked your first three weapons:\n-AK-47\n-M16\n-Beretta\nYou can go equip them in your loadouts now.";
            MainMenuUIManager.instance.AddPopup("Unlocked", content);
            jsonData.hasInitialized = true;
            WriteInputDataToJSON(jsonData);
        }
    }
    public void WriteUserDataToJSON()
    {
        UserDataJSON data = new();
        data = emptyUserDataJSON;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/UserDataConfig.json", json);
        Debug.LogWarning("Writing User Data To Files...");
    }
    public void WriteInputDataToJSON(UserDataJSON data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/UserDataConfig.json", json);
        Debug.LogWarning("Writing User Data To Files...");
    }
    public void InitializeUserDataToJSON()
    {
        UserDataJSON data = new();
        data = emptyUserDataJSON;

        string json = JsonUtility.ToJson(data, true);
        //File.Create(Application.persistentDataPath + "/UserDataConfig.json");
        File.WriteAllText(Application.persistentDataPath + "/UserDataConfig.json", json);
        Debug.LogWarning("Initializing User Data To Files...");
    }
    public void AddUserLevelXP(int amount)
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/UserDataConfig.json");
        UserDataJSON jsonData = JsonUtility.FromJson<UserDataJSON>(json);
        int levelLim = jsonData.userLevel * 500;
        string unlockedContent = "";
        if (jsonData.userLevelXP + amount >= levelLim)
        {
            jsonData.userLevelXP = jsonData.userLevelXP + amount - levelLim;
            jsonData.userLevel++;
            for(int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allWeaponDatas[i].unlockingLevel <= jsonData.userLevel)
                {
                    unlockedContent = unlockedContent + "-" + GlobalDatabase.singleton.allWeaponDatas[i].itemName + "\n";
                }
            }
            if (RoomManager.Instance.currentSceneIndex != 0)
            {
                MainMenuUIManager.PopupData tmp;
                tmp.title = "Level Up";
                tmp.content = "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent);
                tmp.queueType = MainMenuUIManager.PopupQueue.OnMainMenuLoad;
                if (!MainMenuUIManager.instance.queuedPopups.Contains(tmp)) MainMenuUIManager.instance.AddQueuedPopup(tmp.title, tmp.content, MainMenuUIManager.PopupQueue.OnMainMenuLoad);
            }
            else MainMenuUIManager.instance.AddPopup("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent));
            WriteInputDataToJSON(jsonData);
        }
        else
        {
            if (RoomManager.Instance.currentSceneIndex != 0) MainMenuUIManager.instance.AddQueuedPopup("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent), MainMenuUIManager.PopupQueue.OnMainMenuLoad);
            else MainMenuUIManager.instance.AddPopup("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent));
            jsonData.userLevelXP += amount;
            WriteInputDataToJSON(jsonData);
        }
    }
}
