using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Slider fieldOfViewSlider;
    public Text fieldOfViewValue;
    public Toggle fullscreenToggle;

    public void WriteSettingsOptionsToJSON()
    {
        SettingsOptionsJSON data = new SettingsOptionsJSON();
        data.Volume = volumeSlider.value;
        data.FieldOfView = fieldOfViewSlider.value;
        data.Fullscreen = fullscreenToggle.isOn;
        data.MouseSensitivity = sensitivitySlider.value;
        data.QualityIndex = qualityDropdown.value;
        data.ResolutionIndex = resolutionDropdown.value;

        //Debug.Log("Persistent Data Path: " + Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"), json);
        //Debug.LogWarning("Writing Settings Options To Files...");
    }
    public void InitializeSettingsOptionsToJSON()
    {
        SettingsOptionsJSON data = new();
        data.Volume = 0;
        data.FieldOfView = 75f;
        data.Fullscreen = true;
        data.MouseSensitivity = 100f;
        data.QualityIndex = 1;
        data.ResolutionIndex = -1;

        //Debug.Log("Persistent Data Path: " + Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
        string json = JsonUtility.ToJson(data, true);
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"))) File.CreateText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json")).Close();
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"), json);
        //Debug.LogWarning("Writing Settings Options To Files...");
    }
    public void ReadSettingsOptionsFromJSON()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"))) InitializeSettingsOptionsToJSON();
        if (File.Exists(Path.Combine(Application.persistentDataPath, "SettingsOptions.json")))
        {
            string tempJson = File.ReadAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
            if (string.IsNullOrEmpty(tempJson) || string.IsNullOrWhiteSpace(tempJson))
            {
                InitializeSettingsOptionsToJSON();
            }
        }
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
        //Debug.LogWarning("Reading Settings Options To Files...");
        SettingsOptionsJSON jsonData = JsonUtility.FromJson<SettingsOptionsJSON>(json);
        SetFieldOfView(jsonData.FieldOfView);
        SetVolume(jsonData.Volume);
        SetFullscreen(jsonData.Fullscreen);
        SetSensitivity(jsonData.MouseSensitivity);
        SetQuality(jsonData.QualityIndex);
        SetResolution(jsonData.ResolutionIndex);
    }
    public void SetVolume(float volume)
    {
        volumeSlider.value = volume;
        audioMixer.SetFloat("MasterVolume",volume);
        //WriteSettingsOptionsToJSON();
    }

    public void SetSensitivity(float sensitivity)
    {
        sensitivitySlider.value = sensitivity;
        //WriteSettingsOptionsToJSON();
    }
    public void SetFieldOfView(float fov)
    {
        fieldOfViewValue.text = fov.ToString();
        fieldOfViewSlider.value = fov;
        //WriteSettingsOptionsToJSON();
    }
    public void SetQuality(int qualityIndex)
    {
        qualityDropdown.value = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityDropdown.RefreshShownValue();
        //WriteSettingsOptionsToJSON();
    }
    public void SetFullscreen(bool isFullscreen)
    {
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;
        //WriteSettingsOptionsToJSON();
    }
    public void SetResolution(int resolutionIndex)
    {
        //Debug.LogWarning("Resolution Index: " + resolutionIndex);
        //Debug.LogWarning("Available Resolutions: " + resolutions.Length);
        resolutionDropdown.value = resolutionIndex;
        resolutions = Screen.resolutions;
        Resolution resolution = resolutions[resolutionIndex == -1 ? resolutions.Length - 1 : resolutionIndex >= resolutions.Length ? resolutions.Length - 1 : resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        //WriteSettingsOptionsToJSON();
        resolutionDropdown.RefreshShownValue();
    }

    
    public object GetValue(string key)
    {
        object returner = new object();
        switch (key)
        {
            case "Mouse Sensitivity":
                returner = sensitivitySlider.value;
                //Debug.Log("Set Player Sensitivity Value on Spawn ");
                break;
            default:
                return null;
        }
        return returner;
    }

    [HideInInspector] public Resolution[] resolutions;
    private void Awake()
    {
        SettingsMenuAwakeFunction();
        ReadSettingsOptionsFromJSON();
    }
    public void SettingsMenuAwakeFunction()
    {
        volumeSlider.minValue = -80;
        volumeSlider.maxValue = 0;
        sensitivitySlider.minValue = 0f;
        sensitivitySlider.maxValue = 300f;
        fieldOfViewSlider.minValue = 75f;
        fieldOfViewSlider.maxValue = 100f;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resOptions = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate;
            resOptions.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
        qualityDropdown.RefreshShownValue();
        SetDefaultOptionValues();
    }
    public void SetDefaultOptionValues()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"))) InitializeSettingsOptionsToJSON();
        if (File.Exists(Path.Combine(Application.persistentDataPath, "SettingsOptions.json")))
        {
            string tempJson = File.ReadAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
            if (string.IsNullOrEmpty(tempJson) || string.IsNullOrWhiteSpace(tempJson))
            {
                InitializeSettingsOptionsToJSON();
            }
        }
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
        //Debug.LogWarning("Reading Settings Options To Files...");
        SettingsOptionsJSON jsonData = JsonUtility.FromJson<SettingsOptionsJSON>(json);

        SetVolume(jsonData.Volume);

        SetQuality(jsonData.QualityIndex);

        SetFullscreen(jsonData.Fullscreen);

        if (jsonData.ResolutionIndex == -1) SetResolution(resolutions.Length - 1);
        else SetResolution(jsonData.ResolutionIndex);

        SetSensitivity(jsonData.MouseSensitivity);

        SetFieldOfView(jsonData.FieldOfView);
        qualityDropdown.RefreshShownValue();
        resolutionDropdown.RefreshShownValue();
    }
}
