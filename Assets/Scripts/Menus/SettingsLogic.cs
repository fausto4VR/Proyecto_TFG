using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using System.Globalization;

public class SettingsLogic : MonoBehaviour
{
    [Header("UI Objects Section")]
    [SerializeField] private GameObject volumeSlider;
    [SerializeField] private GameObject resolutionDropdown;
    [SerializeField] private GameObject qualityDropdown;
    [SerializeField] private GameObject screenModeToggle;

    [Header("Variable Section")]
    [SerializeField] private int defaultQuality = 5;
    [SerializeField] private float defaultVolume = 0.6f;
    [SerializeField] private bool defaultScreenMode = true;
    
    [Header("Audio Section")]
    [SerializeField] private GameObject audioSourcesManager;
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] resolutions;
    private Resolution gameResolution;
    private Resolution defaultResolution;
    private float gameAudioVolume;
    private int gameQuality;
    private bool gameScreenMode;
    
    // REVISAR AUDIO
    private AudioSource resetButtonsAudioSource;


    void Start()
    { 
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        resetButtonsAudioSource = audioSources[1];

        resolutions = Screen.resolutions;
        defaultResolution = resolutions[^1];

        LoadSettings();
        UploadUIValues();
        SetConfigurationValues();
    }

    // Método para reiniciar los valores de configuración
    public void ResetDefaultValues()
    {
        resetButtonsAudioSource.Play();

        PlayerPrefs.DeleteAll();

        LoadSettings();
        UploadUIValues();
        SetConfigurationValues();
    }

    // Método para cargar los datos de configuración
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
            gameScreenMode = TransformIntToBool(PlayerPrefs.GetInt("gameScreenMode"));
        }
        else
        {
            gameScreenMode = defaultScreenMode;
            PlayerPrefs.SetInt("gameScreenMode", TransformBoolToInt(gameScreenMode));
        }

        if(PlayerPrefs.HasKey("gameResolution"))
        {
            gameResolution = TransformStringToResolution(PlayerPrefs.GetString("gameResolution"));
        }
        else
        {
            gameResolution = defaultResolution;
            PlayerPrefs.SetString("gameResolution", TransformResolutionToString(gameResolution));
        }
    }    

    // Método para mostrar en la UI los valores de configuración asignados
    private void UploadUIValues()
    {
        var slider = volumeSlider.GetComponent<UnityEngine.UI.Slider>();
        var qualityDrop = qualityDropdown.GetComponent<TMP_Dropdown>();
        var screenToggle = screenModeToggle.GetComponent<UnityEngine.UI.Toggle>();
        var resolutionDrop = resolutionDropdown.GetComponent<TMP_Dropdown>();

        slider.value = gameAudioVolume;
        qualityDrop.value = gameQuality;
        screenToggle.isOn = gameScreenMode;

        resolutionDrop.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            var res = resolutions[i];
            string option = $"{res.width} x {res.height} - {(int)res.refreshRateRatio.value} Hz";
            options.Add(option);

            if (res.width == gameResolution.width && res.height == gameResolution.height &&
                (uint)res.refreshRateRatio.value == (uint)gameResolution.refreshRateRatio.value)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDrop.AddOptions(options);
        resolutionDrop.value = currentResolutionIndex;
        resolutionDrop.RefreshShownValue();
    }

    // Método para aplicar los parametros de configuración correctamente
    private void SetConfigurationValues()
    {
        SetVolume(gameAudioVolume);
        SetQuality(gameQuality);
        SetScreenMode(gameScreenMode);

        for (int i = 0; i < resolutions.Length; i++)
        {
            var res = resolutions[i];
            if (res.width == gameResolution.width && res.height == gameResolution.height &&
                (uint)res.refreshRateRatio.value == (uint)gameResolution.refreshRateRatio.value)
            {
                SetResolution(i);
                break;
            }
        }
    }

    // Método para configurar la resolución desde el dropdown
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
        PlayerPrefs.SetString("gameResolution", TransformResolutionToString(resolution));
    }

    // Método para configurar el volumen desde el slider
    public void SetVolume(float volume)
    {
        // Se ajusta el volumen en decibelios de forma logarítmica para una percepción auditiva más natural
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("gameAudioVolume", volume);
    }

    // Método para configurar la calidad desde el dropdown
    public void SetQuality(int qualityIndex)
    {
        // Se incrementa el índice a 1 porque en el dropdown no se incluye la primera calidad (muy baja)
        QualitySettings.SetQualityLevel(qualityIndex + 1); 
        PlayerPrefs.SetInt("gameQuality", qualityIndex);
    }

    // Método para configurar si está en modo pantalla completa desde el toggle
    public void SetScreenMode(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("gameScreenMode", TransformBoolToInt(isFullScreen));
    }

    // Método auxiliar para transformar un tipo int (0 o 1) a bool
    private bool TransformIntToBool(int numberValue)
    {
        if(numberValue == 1) return true;
        else return false;
    }

    // Método auxiliar para transformar un tipo bool a int (0 o 1)
    private int TransformBoolToInt(bool boolValue)
    {
        if(boolValue) return 1;
        else return 0;
    }

    // Método auxiliar para transformar una resolución de pantalla a un tipo string
    private string TransformResolutionToString(Resolution resolutionValue)
    {
        return resolutionValue.width + "x" + resolutionValue.height + "x" + 
            resolutionValue.refreshRateRatio.value.ToString(CultureInfo.InvariantCulture);
    }

    // Método auxiliar para transformar un tipo string a una resolución de pantalla
    private Resolution TransformStringToResolution(string stringValue)
    {
        string[] parts = stringValue.Split('x');
        if (parts.Length < 3)
        {
            Debug.LogError("Formato de resolución no válido en el panel de configuración: " + stringValue);
            return Screen.currentResolution;
        }

        if (!int.TryParse(parts[0], out int width) || 
            !int.TryParse(parts[1], out int height))
        {
            Debug.LogError("Error al parsear la resolución en el panel de configuración: " + stringValue);
            return Screen.currentResolution;
        }

        float refreshRateFloat;
        try
        {
            refreshRateFloat = float.Parse(parts[2], CultureInfo.InvariantCulture);
        }
        catch
        {
            Debug.LogError("Error al parsear el refreshRate en el panel de configuración: " + parts[2]);
            return Screen.currentResolution;
        }

        return new Resolution
        {
            width = width,
            height = height,
            refreshRateRatio = new RefreshRate { numerator = (uint)refreshRateFloat, denominator = 1 }
        };
    }
}
