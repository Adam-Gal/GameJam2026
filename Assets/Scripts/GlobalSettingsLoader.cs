using UnityEngine;
using System.IO;

public class GlobalSettingsLoader : MonoBehaviour
{
    private void Awake()
    {
        LoadAndApplySettings();
    }

    private void LoadAndApplySettings()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "settings.json");

        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SettingsData data = JsonUtility.FromJson<SettingsData>(json);

            AudioListener.volume = data.volume;

            Debug.Log($"Global settings loaded: Volume = {data.volume}");
        }
        else
        {
            AudioListener.volume = 1f;
            Debug.Log("No settings file found, using default volume");
        }
    }
}