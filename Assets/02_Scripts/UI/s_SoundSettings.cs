using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class s_SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider _masterVolumeSlider, _sfxVolumeSlider, _musicVolumeSlider = null;
    [SerializeField] private TextMeshProUGUI MasterVolumeTextUI, SfXVolumeTextUI, MusicVolumeTextUI = null;
    [SerializeField] private float maxSliderValue = 100.0f;
    SoundManager _soundManager = null;
    //private float MasterVolume, SFXVolume, MusicVolume = 1;
    private void Awake()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        //MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        //SFXVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        //MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");

        //MasterVolume = AudioListener.volume;
        //SFXVolume = AudioListener.volume;
        //MusicVolume = AudioListener.volume;
    }

    private void Start()
    {
        _masterVolumeSlider.value = _soundManager._masterVolume;
        _musicVolumeSlider.value = _soundManager._musicVolume;
        _sfxVolumeSlider.value = _soundManager._sfxVolume;
    }

    public void ChangeSFXVolume(float sliderValue)
    {
        _soundManager.SFXBusSetVolume(sliderValue);
        SfXVolumeTextUI.text = (100.0f*sliderValue).ToString("0");
    }

    public void ChangeMusicVolume(float sliderValue)
    {
        _soundManager.MusicBusSetVolume(sliderValue);
        MusicVolumeTextUI.text = (100.0f * sliderValue).ToString("0");
    }

    public void ChangeMasterVolume(float sliderValue)
    {
        _soundManager.MasterBusSetVolume(sliderValue);
        MasterVolumeTextUI.text = (100.0f * sliderValue).ToString("0");
    }

    public void saveVolumeSettings()
    {
        //PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
        //PlayerPrefs.SetFloat("SFXVolume", 0.8f);
        //PlayerPrefs.SetFloat("MusicVolume", 0.8f);
        //PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
        //PlayerPrefs.SetFloat("SFXVolume", AudioListener.volume);
        //PlayerPrefs.SetFloat("MusicVolume", AudioListener.volume);
        //PlayerPrefs.Save();
    }
}
