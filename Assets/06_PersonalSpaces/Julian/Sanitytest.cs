using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sanitytest : MonoBehaviour
{

    [SerializeField] s_SoundManager _soundmanager; // FMOD SoundManager
    [SerializeField] GameObject _gameObject;
    private float test = 0f;
    // Start is called before the first frame update
    void Start()
    {
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    _soundmanager.MusicBusSetVolume(0.2f);
        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    _soundmanager.MusicBusSetVolume(1f);
        //}
        //_soundmanager.RegisterEventEmitter(_gameObject, "event:/SFX/LowSanity");
        ////_soundmanager.SetParameterToEventEmitter(_gameObject, "event:/SFX/LowSanity", "Sanity", 0f);
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    _soundmanager.SetParameterToEventEmitter(_gameObject, "event:/SFX/LowSanity", "Sanity", -0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    _soundmanager.SetParameterToEventEmitter(_gameObject, "event:/SFX/LowSanity", "Sanity", 0.1f);
        //}
        
    }
}
