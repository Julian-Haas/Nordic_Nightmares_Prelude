using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SavedSettings
{

    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _sfxVolume;

    private string _filePath = Application.persistentDataPath + "/settings.json";


    public float MasterVolume {
        get {
            return _masterVolume;
        }

        set {
            _masterVolume = value;
            //s_SoundManager.SoundManagerInstance.MasterBusSetVolume(_masterVolume);
        }
    }

    public float MusicVolume {
        get {
            return _musicVolume;
        }

        set {
            _musicVolume = value;
            //s_SoundManager.SoundManagerInstance.MusicBusSetVolume(_musicVolume);
        }
    }

    public float SfxVolume {
        get {
            return _sfxVolume;
        }

        set {
            _sfxVolume = value;
            //s_SoundManager.SoundManagerInstance.SFXBusSetVolume(_sfxVolume);
        }
    }

    public SavedSettings() {
        _masterVolume = 0.5f;
        _musicVolume = 0.5f;
        _sfxVolume = 0.5f;
    }

    public SavedSettings LoadSettings() {
        SavedSettings settings;
        if(File.Exists(_filePath)) {
            string json = File.ReadAllText(_filePath);
            settings = JsonUtility.FromJson<SavedSettings>(json);
        }
        else {
            settings = new SavedSettings();
            settings.SaveSettings();
        }
        ApplySettings();
        return settings;
    }

    // changes should be activly saved and not automaticaly when value changes
    public void SaveSettings() {

        string json = JsonUtility.ToJson(this);
        File.WriteAllText(_filePath,json);
    }

    private void ApplySettings() {
        //s_SoundManager.SoundManagerInstance.MasterBusSetVolume(_masterVolume);
        //s_SoundManager.SoundManagerInstance.MusicBusSetVolume(_musicVolume);
        //s_SoundManager.SoundManagerInstance.SFXBusSetVolume(_sfxVolume);
    }
}
