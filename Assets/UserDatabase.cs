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
        MainMenuUIManager.instance.SetUserGUIData(PlayerPrefs.GetString("Username"), jsonData.userLevel, jsonData.userLevelXP / (jsonData.userLevel * 500), jsonData.userCoins);
        
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
}
