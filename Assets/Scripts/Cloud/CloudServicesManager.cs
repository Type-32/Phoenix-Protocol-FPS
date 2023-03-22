using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;
public class CloudServicesManager : MonoBehaviour
{
    /*
    //! Deprecated
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

    /*
    public static CloudServicesManager Instance;
    private async void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        // Cloud Save needs to be initialized along with the other Unity Services that
        // it depends on (namely, Authentication), and then the user must sign in.
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        AuthenticationService.Instance.SignedOut += OnSignedOut;
        AuthenticationService.Instance.SignInFailed += OnSignErrorProvided;
        Debug.Log($"Unity services initialization: {UnityServices.State}");

        //Shows if a cached session token exist
        Debug.Log($"Cached Session Token Exist: {AuthenticationService.Instance.SessionTokenExists}");

        // Shows Current profile
        Debug.Log(AuthenticationService.Instance.Profile);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        /*

        await ForceSaveSingleData("primitive_key", "value!");

        SampleObject outgoingSample = new SampleObject
        {
            AmazingFloat = 13.37f,
            SparklingInt = 1337,
            SophisticatedString = "hi there!"
        };
        await ForceSaveObjectData("object_key", outgoingSample);
        SampleObject incomingSample = await RetrieveSpecificData<SampleObject>("object_key");
        Debug.Log($"Loaded sample object: {incomingSample.AmazingFloat}, {incomingSample.SparklingInt}, {incomingSample.SophisticatedString}");

        await ForceDeleteSpecificData("object_key");
        await ListAllKeys();
        await RetrieveEverything();
        
    }*/
    /*
    private void OnSignedIn()
    {
        //Shows how to get a playerID
        Debug.Log($"PlayedID: {AuthenticationService.Instance.PlayerId}");

        //Shows how to get an access token
        Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

        const string successMessage = "Sign in anonymously succeeded!";
        Debug.Log(successMessage);
    }
    private void OnSignedOut()
    {
        Debug.Log("Signed Out!");
    }
    private void OnSignErrorProvided(RequestFailedException errorResponse)
    {
        Debug.LogError("RequestFailedException returned with the following message: " + errorResponse.Message);
    }
    private async Task ListAllKeys()
    {
        try
        {
            var keys = await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();

            Debug.Log($"Keys count: {keys.Count}\n" +
                $"Keys: {String.Join(", ", keys)}");
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }
    private async Task ForceSaveSingleData(string key, string value)
    {
        try
        {
            Dictionary<string, object> oneElement = new Dictionary<string, object>();

            // It's a text input field, but let's see if you actually entered a number.
            if (Int32.TryParse(value, out int wholeNumber))
            {
                oneElement.Add(key, wholeNumber);
            }
            else if (Single.TryParse(value, out float fractionalNumber))
            {
                oneElement.Add(key, fractionalNumber);
            }
            else
            {
                oneElement.Add(key, value);
            }

            await CloudSaveService.Instance.Data.ForceSaveAsync(oneElement);

            Debug.Log($"Successfully saved {key}:{value}");
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }
    private async Task ForceSaveObjectData<T>(string key, T value)
    {
        try
        {
            // Although we are only saving a single value here, you can save multiple keys
            // and values in a single batch.
            Dictionary<string, object> oneElement = new Dictionary<string, object>
                {
                    { key, value }
                };

            await CloudSaveService.Instance.Data.ForceSaveAsync(oneElement);

            Debug.Log($"Successfully saved {key}:{value}");
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }
    private async Task<T> RetrieveSpecificData<T>(string key)
    {
        try
        {
            var results = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key });

            if (results.TryGetValue(key, out string value))
            {
                return JsonUtility.FromJson<T>(value);
            }
            else
            {
                Debug.Log($"There is no such key as {key}!");
            }
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }

        return default;
    }
    private async Task RetrieveEverything()
    {
        try
        {
            // If you wish to load only a subset of keys rather than everything, you
            // can call a method LoadAsync and pass a HashSet of keys into it.
            var results = await CloudSaveService.Instance.Data.LoadAllAsync();

            Debug.Log($"Elements loaded!");

            foreach (var element in results)
            {
                Debug.Log($"Key: {element.Key}, Value: {element.Value}");
            }
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }
    private async Task ForceDeleteSpecificData(string key)
    {
        try
        {
            await CloudSaveService.Instance.Data.ForceDeleteAsync(key);

            Debug.Log($"Successfully deleted {key}");
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }*/
}
