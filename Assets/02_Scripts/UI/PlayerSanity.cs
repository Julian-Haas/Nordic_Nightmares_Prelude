using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerSanity : MonoBehaviour
{

    public UnityEvent OnSanityChanged;

    [Tooltip("Sanity Influence Rates in Percent")]
    public int General = 5, Cover = 15, SafeZone = 5, HealingZone = 15;
    public bool _inCover = false, _isHidden = false;
    public float _influence = 0.0f, _sanity, _sanityShift = 0.0f;

    private Slider _sanitySlider;
    private s_SoundManager _soundManager;
    private bool _sanityEmptySoundPlayed = false, _sanityLowSoundPlaying = false;
    public Material material;

    private void Start() {
        _soundManager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        _sanity = 100.0f;
        _sanityShift += (float) General;
        _sanitySlider = GameObject.Find("SanitySlider")?.GetComponent<Slider>();
    }

    private void Update() {
        if(Time.timeScale == 1) {
            sanityUpdate();
        }
    }


    private void SanityEmpty() {
        if(!_sanityEmptySoundPlayed) {
            _soundManager.PlaySound2D("event:/SFX/SanityEmpty");
            _sanityEmptySoundPlayed = true;
        }
    }

    //private void SanityLow()
    //{
    //    if (_sanityLowSoundPlaying)
    //    {
    //        _soundManager.SetParameterToEventEmitter(this.gameObject, "event:/SFX/LowSanity", "Sanity", _sanity);          
    //    }
    //}

    void sanityUpdate() {
        if(_sanity <= 100.0f && _sanity > 30.0f) {
            if(_sanityLowSoundPlaying) {
                _soundManager.UnregisterEventEmitter(this.gameObject,"event:/SFX/LowSanity");
                _sanityLowSoundPlaying = false;
            }
            _sanityShift = -1 * _influence * Time.deltaTime;
        }
        if(_sanity <= 30.0f && _sanity > 0.0f) {
            if(!_sanityLowSoundPlaying) {
                _soundManager.RegisterEventEmitter(this.gameObject,"event:/SFX/LowSanity");
                _sanityLowSoundPlaying = true;
            }
            _soundManager.SetParameterToEventEmitter(this.gameObject,"event:/SFX/LowSanity","Sanity",_sanity / 100.0f);
            _sanityShift = -1 * _influence * Time.deltaTime;
            //SanityLow();
        }
        else if(_sanity <= 0.0f) {
            SanityEmpty();
            _sanityShift = -1 * _influence * Time.deltaTime;
        }
        _sanity += _sanityShift;
        _sanity = Mathf.Clamp(_sanity,0.0f,100.0f);

        float _sanityMat = 1.0f - (_sanity / 100.0f);
        _sanityMat = Mathf.Clamp(_sanityMat,0.0f,0.65f);
        material.SetFloat("_FadeInOverlay",_sanityMat);

        _sanitySlider.value = _sanity;
    }
}