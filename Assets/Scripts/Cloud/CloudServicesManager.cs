using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PrototypeLib.OnlineServices.UnityCloudServices;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
public class CloudServicesManager : MonoBehaviour
{
    void Start()
    {
        // Configure the Unity Authentication service
        var authService = AuthenticationClient.Instance;
        authService.Initialize();

        // Check if the user is already logged in
        if (authService.IsLoggedIn)
        {
            Debug.Log("User is already logged in");
            // Load the user's data from Cloud Save
            LoadData();
        }
        else
        {
            // Prompt the user to enter their username and password
            string username = "Enter your username";
            string password = "Enter your password";

            // Log in the user with the username and password
            authService.LoginWithUsernameAndPasswordAsync(username, password, OnLoginComplete);
        }
    }

    // Callback function for login
    void OnLoginComplete(AuthenticationClient.LoginResult result)
    {
        if (result.Status == AuthenticationClient.LoginStatus.Success)
        {
            Debug.Log("User logged in successfully");
            // Load the user's data from Cloud Save
            LoadData();
        }
        else
        {
            Debug.LogError("User login failed: " + result.Message);
        }
    }

    // Function to load the user's data from Cloud Save
    void LoadData()
    {
        // Initialize the Cloud Save client
        var cloudSaveClient = CloudSaveClient.Instance;
        cloudSaveClient.Initialize();

        // Get the user's data from Cloud Save
        cloudSaveClient.GetDataAsync(OnGetDataComplete);
    }

    // Callback function for getting data
    void OnGetDataComplete(CloudSaveClient.GetDataResult result)
    {
        if (result.Status == CloudSaveClient.GetDataStatus.Success)
        {
            Debug.Log("User data loaded successfully");
            // Do something with the user's data
            // For example, display the user's name and score
            string name = result.Data.GetString("name");
            int score = result.Data.GetInt("score");
            Debug.Log("User name: " + name);
            Debug.Log("User score: " + score);
        }
        else
        {
            Debug.LogError("User data load failed: " + result.Message);
        }
    }

    // Function to save the user's data to Cloud Save
    void SaveData()
    {
        // Initialize the Cloud Save client
        var cloudSaveClient = CloudSaveClient.Instance;
        cloudSaveClient.Initialize();

        // Create a custom data type to store the user's data
        // For example, a class that contains the user's name, score, and level
        [System.Serializable]
        public class UserData
    {
        public string name;
        public int score;
        public int level;

        public UserData(string name, int score, int level)
        {
            this.name = name;
            this.score = score;
            this.level = level;
        }
    }

    // Create an instance of the custom data type with some values
    // For example, a user named Alice with a score of 100 and a level of 5
    UserData userData = new UserData("Alice", 100, 5);

    // Convert the custom data type to a JSON string
    string json = JsonUtility.ToJson(userData);

    // Save the JSON string to Cloud Save
    cloudSaveClient.SaveDataAsync(json, OnSaveDataComplete);
    }

// Callback function for saving data
void OnSaveDataComplete(CloudSaveClient.SaveDataResult result)
{
    if (result.Status == CloudSaveClient.SaveDataStatus.Success)
    {
        Debug.Log("User data saved successfully");
    }
    else
    {
        Debug.LogError("User data save failed: " + result.Message);
    }
}
}
