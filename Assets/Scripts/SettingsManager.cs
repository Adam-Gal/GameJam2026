using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeButton;

    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "settings.json");

        // Hide panel at start
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Start()
    {
        // Setup button listeners
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);

        // Setup slider listener
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Load saved settings
        LoadSettings();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnVolumeChanged(float value)
    {
        // Set the master volume
        AudioListener.volume = value;

        // Save immediately when changed
        SaveSettings();
    }

    private void SaveSettings()
    {
        SettingsData data = new SettingsData
        {
            volume = volumeSlider != null ? volumeSlider.value : 1f
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"Settings saved to: {saveFilePath}");
    }

    private void LoadSettings()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SettingsData data = JsonUtility.FromJson<SettingsData>(json);

            // Apply loaded volume
            if (volumeSlider != null)
            {
                volumeSlider.value = data.volume;
            }

            AudioListener.volume = data.volume;

            Debug.Log($"Settings loaded from: {saveFilePath}");
        }
        else
        {
            // Default settings
            if (volumeSlider != null)
            {
                volumeSlider.value = 1f;
            }
            AudioListener.volume = 1f;

            Debug.Log("No settings file found, using defaults");
        }
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OpenSettings);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseSettings);

        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
}

[System.Serializable]
public class SettingsData
{
    public float volume = 1f;
}