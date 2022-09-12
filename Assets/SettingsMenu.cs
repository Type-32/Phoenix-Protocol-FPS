using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public void SetVolume(float volume)
    {
        volumeSlider.value = volume;
        audioMixer.SetFloat("MasterVolume",volume);
        PlayerPrefs.SetFloat("Master Volume", volume);
    }
    public void SetQuality(int qualityIndex)
    {
        qualityDropdown.value = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality Index", qualityIndex);
        qualityDropdown.RefreshShownValue();
    }
    public void SetFullscreen(bool isFullscreen)
    {
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen Enabled", isFullscreen ? 1 : 0);
    }
    public void SetResolution(int resolutionIndex)
    {
        resolutionDropdown.value = resolutionIndex;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution Index", resolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    Resolution[] resolutions;
    private void Start()
    {
        volumeSlider.minValue = -80;
        volumeSlider.maxValue = 0;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resOptions = new List<string>();
        int currentResIndex = 0;
        for(int i = 0;i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate;
            resOptions.Add(option);
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
        qualityDropdown.RefreshShownValue();

        if (!PlayerPrefs.HasKey("Master Volume"))
        {
            SetVolume(0);
        }
        else
        {
            SetVolume(PlayerPrefs.GetFloat("Master Volume"));
        }
        if (!PlayerPrefs.HasKey("Quality Index"))
        {
            SetQuality(1);
            qualityDropdown.RefreshShownValue();
        }
        else
        {
            SetQuality(PlayerPrefs.GetInt("Quality Index"));
            qualityDropdown.RefreshShownValue();
        }
        if (!PlayerPrefs.HasKey("Master Volume"))
        {
            SetVolume(0);
        }
        else
        {
            SetVolume(PlayerPrefs.GetFloat("Master Volume"));
        }
        if (!PlayerPrefs.HasKey("Fullscreen Enabled"))
        {
            SetFullscreen(true);
        }
        else
        {
            SetFullscreen(PlayerPrefs.GetInt("Fullscreen Enabled") == 1 ? true : false);
        }
        if (!PlayerPrefs.HasKey("Resolution Index"))
        {
            SetResolution(resolutions.Length - 1);
            resolutionDropdown.RefreshShownValue();
        }
        else
        {
            SetQuality(PlayerPrefs.GetInt("Resolution Index"));
            resolutionDropdown.RefreshShownValue();
        }
    }
}
