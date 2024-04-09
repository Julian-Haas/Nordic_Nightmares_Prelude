using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundmanager : MonoBehaviour
{


    private void Awake() // ist start sinnvoller?
    {
        LoadValues();
    }

    void LoadValues()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
            //PlayerPrefs.SetFloat("MusicVolume", 0.1f);
            Debug.Log("Master volume existiert");
            Debug.Log(PlayerPrefs.GetFloat("MasterVolume"));
        }
        else
        {
            PlayerPrefs.SetFloat("MasterVolume", 1.0f);
            AudioListener.volume = 1f;
            Debug.Log("Master volume existiert nicht");

        }


        //if (PlayerPrefs.HasKey("SFXVolume"))
        //{
        //    AudioListener.volume = PlayerPrefs.GetFloat("SFXVolume");
        //    //PlayerPrefs.SetFloat("MusicVolume", 0.2f);
        //}
        //else
        //{
        //    PlayerPrefs.SetFloat("SFXVolume", 1.0f);
        //    AudioListener.volume = 1f;
        //}

        //if (PlayerPrefs.HasKey("MusicVolume"))
        //{
        //    AudioListener.volume = PlayerPrefs.GetFloat("MusicVolume");
        //    //PlayerPrefs.SetFloat("MusicVolume", 0.3f);
        //}
        //else
        //{
        //    PlayerPrefs.SetFloat("MusicVolume", 1.0f);
        //    AudioListener.volume = 1f;
        //} 
    }
}
