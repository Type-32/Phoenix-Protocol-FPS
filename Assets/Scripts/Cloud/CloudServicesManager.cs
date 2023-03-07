using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PrototypeLib.OnlineServices.UnityCloudServices;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;
public class CloudServicesManager : MonoBehaviour
{
    /*
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    void Start()
    {
        // Configure the Unity Authentication service
        var authService = AuthenticationService.Instance;
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
    void OnLoginComplete(AuthenticationService.LoginResult result)
    {
        if (result.Status == AuthenticationService.LoginStatus.Success)
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
        var cloudSaveClient = CloudSaveService.Instance;
        cloudSaveClient.Initialize();

        // Get the user's data from Cloud Save
        cloudSaveClient.GetDataAsync(OnGetDataComplete);
    }

    // Callback function for getting data
    void OnGetDataComplete(CloudSaveService.GetDataResult result)
    {
        if (result.Status == CloudSaveService.GetDataStatus.Success)
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
        var cloudSaveClient = CloudSaveService.Instance;
        cloudSaveClient.Initialize();

        // Convert the custom data type to a JSON string
        string json = JsonUtility.ToJson(userData);

        // Save the JSON string to Cloud Save
        cloudSaveClient.SaveDataAsync(json, OnSaveDataComplete);
    }

    // Callback function for saving data
    void OnSaveDataComplete(CloudSaveService.SaveDataResult result)
    {
        if (result.Status == CloudSaveService.SaveDataStatus.Success)
        {
            Debug.Log("User data saved successfully");
        }
        else
        {
            Debug.LogError("User data save failed: " + result.Message);
        }
    }*/
}
