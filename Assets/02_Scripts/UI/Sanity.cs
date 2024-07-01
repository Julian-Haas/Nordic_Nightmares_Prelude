using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Sanity : MonoBehaviour
{
    public static Sanity Instance {
        get; private set;
    }
    [SerializeField] Material material;
    [SerializeField] float _sanityDrainNormal = 1.0f;
    [SerializeField] float _sanityGainTorch = 15.0f;
    [SerializeField] float _sanityGainSafeZone = 15.0f;
    private float _sanity = 100.0f;
    private float _sanityChange;
    private bool _sanityEmptySoundPlayed = false;
    //private bool _sanityLowSoundPlaying = false;
    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(Instance);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }
    private void Start() {
        _sanityChange = _sanityDrainNormal;
    }
    private void Update() {
        if(Time.timeScale == 1) {
            sanityUpdate();
        }
    }
    private void SanityEmpty() {
        if(!_sanityEmptySoundPlayed) {
            //_soundManager.PlaySound2D("event:/SFX/SanityEmpty");
            _sanityEmptySoundPlayed = true;
        }
    }
    void sanityUpdate() {
        _sanity = Mathf.Clamp(_sanity - (_sanityChange * Time.deltaTime),0.0f,100.0f);
        //if(_sanity <= 70.0f && !_sanityOverlayExplained) {
        //    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I shouldn't stay far from light for too long.");
        //    _sanityOverlayExplained = true;
        //}
        float _sanityMat;
        if(_sanity < 70.0f) {
            _sanityMat = 0.85f - ((_sanity) / 85.0f);
        }
        else {
            _sanityMat = 0.0f;
        }
        _sanityMat = Mathf.Clamp(_sanityMat,0.0f,1.0f);
        material.SetFloat("_FadeInMadness",_sanityMat);
        //_soundManager.musicInstance.SetParameter("Sanity",_sanity / 100.0f);
        //_soundManager.ambientInstance.SetParameter("Zoom",_sanityMat);
    }

    public void CloseToTorch(bool CloseToTorch) {
        if(CloseToTorch) {
            _sanityChange = _sanityGainTorch;
        }
        else {
            _sanityChange = _sanityDrainNormal;
        }
    }
    public void InActiveSafeZone(bool InActiveSafeZone) {
        if(InActiveSafeZone) {
            _sanityChange = _sanityGainSafeZone;
        }
        else {
            _sanityChange = _sanityDrainNormal;
        }
    }
    public void ResetSanity() {
        _sanity = 100.0f;
    }
}