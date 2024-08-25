using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    Resolution[] resolutions;

    public Dropdown resolutionDropdown;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        var QLevel = PlayerPrefs.GetInt("GraphicsQuality");

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        int QualityIndex = 0;

        // Check if the "GraphicsQuality" player preference exists
        if (PlayerPrefs.HasKey("GraphicsQuality"))
        {
            QualityIndex = PlayerPrefs.GetInt("GraphicsQuality", 0);
            QualitySettings.SetQualityLevel(QualityIndex);

            Debug.Log("Set quality to level " + QualitySettings.GetQualityLevel());
            //qualityDropdown.value = QualityIndex;
        }
        else
        {
            // Set the default quality level for the device type
            if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Handheld)
            {
                QualityIndex = 0;
            }

            else if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
            {
                QualityIndex = 2;
            }

            // Save the player preference for GraphicsQuality
            PlayerPrefs.SetInt("GraphicsQuality", QualityIndex);
            Debug.Log("Set quality to level " + QualitySettings.GetQualityLevel());
            PlayerPrefs.Save();

            // Set the quality level
            QualitySettings.SetQualityLevel(QualityIndex);
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
