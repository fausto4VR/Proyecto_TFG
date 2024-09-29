using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using Unity.VisualScripting;

public class SettingsLogic : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject volumeSlider;
    [SerializeField] private GameObject resolutionDropdown;
    [SerializeField] private GameObject qualityDropdown;
    [SerializeField] private GameObject screenModeToggle;
    [SerializeField] private int defaultQuality = 5;
    [SerializeField] private float defaultVolume = 0.6f;
    [SerializeField] private bool defaultScreenMode = true;

    private float gameAudioVolume;
    private int gameQuality;
    private bool gameScreenMode;
    private Resolution[] resolutions;
    private Resolution gameResolution;
    private Resolution defaultResolution;

    void Start()
    {
        resolutions = Screen.resolutions;
        defaultResolution = resolutions[^1];
        LoadSettings();
        uploadUIValues();
    }

    private void LoadSettings()
    {
        if(PlayerPrefs.HasKey("gameAudioVolume"))
        {
            gameAudioVolume = PlayerPrefs.GetFloat("gameAudioVolume");
        }
        else
        {
            gameAudioVolume = defaultVolume;
            PlayerPrefs.SetFloat("gameAudioVolume", gameAudioVolume);
        }

        if(PlayerPrefs.HasKey("gameQuality"))
        {
            gameQuality = PlayerPrefs.GetInt("gameQuality");
        }
        else
        {
            gameQuality = defaultQuality;
            PlayerPrefs.SetInt("gameQuality", gameQuality);
        }

        if(PlayerPrefs.HasKey("gameScreenMode"))
        {
            gameScreenMode = transformIntToBool(PlayerPrefs.GetInt("gameScreenMode"));
        }
        else
        {
            gameScreenMode = defaultScreenMode;
            PlayerPrefs.SetInt("gameScreenMode", transformBoolToInt(gameScreenMode));
        }

        if(PlayerPrefs.HasKey("gameResolution"))
        {
            gameResolution = transformStringToResolution(PlayerPrefs.GetString("gameResolution"));
        }
        else
        {
            gameResolution = defaultResolution;
            PlayerPrefs.SetString("gameResolution", transformResolutionToString(gameResolution));
        }
    }

    private void uploadUIValues()
    {
        volumeSlider.GetComponent<UnityEngine.UI.Slider>().value = gameAudioVolume;
        qualityDropdown.GetComponent<TMP_Dropdown>().value = gameQuality;
        screenModeToggle.GetComponent<UnityEngine.UI.Toggle>().isOn = gameScreenMode;
        
        TMP_Dropdown resolutionDropdownOptions = resolutionDropdown.GetComponent<TMP_Dropdown>(); 
        resolutionDropdownOptions.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " - " + (int)resolutions[i].refreshRateRatio.value 
                + " Hz";
            options.Add(option);

            if(resolutions[i].width == gameResolution.width && resolutions[i].height == gameResolution.height 
                && (uint) resolutions[i].refreshRateRatio.value == (uint) gameResolution.refreshRateRatio.value)
            {
                currentResolutionIndex = i;
            }
        }   
        
        resolutionDropdownOptions.AddOptions(options);
        resolutionDropdownOptions.value = currentResolutionIndex; 
    }

    private bool transformIntToBool(int numberValue)
    {
        if(numberValue == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int transformBoolToInt(bool boolValue)
    {
        if(boolValue)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private string transformResolutionToString(Resolution resolutionValue)
    {
        return resolutionValue.width + "x" + resolutionValue.height + "x" + resolutionValue.refreshRateRatio.value;
    }

    private Resolution transformStringToResolution(string stringValue)
    {
        char separator = 'x';
        string[] words = stringValue.Split(separator);
        RefreshRate gameRefreshRateRatio = new RefreshRate();
        gameRefreshRateRatio.denominator = 1;

        string numberString = words[2];
        float numberFloat = float.Parse(numberString);
        gameRefreshRateRatio.numerator = (uint) numberFloat;

        Resolution resolutionTransform = new Resolution
        {
            width = int.Parse(words[0]),
            height = int.Parse(words[1]),
            refreshRateRatio = gameRefreshRateRatio
        };

        return resolutionTransform;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetString("gameResolution", transformResolutionToString(resolution));
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("gameAudioVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex+1); 
        PlayerPrefs.SetInt("gameQuality", qualityIndex);
    }

    public void SetScreenMode(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("gameScreenMode", transformBoolToInt(isFullScreen));
    }

    public void ResetDefaultValues()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
        uploadUIValues();
        Screen.fullScreen = defaultScreenMode;
    }
}
