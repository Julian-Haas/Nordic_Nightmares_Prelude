using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : Interactable
{
    [SerializeField] GameObject lightOfTorch;
    [SerializeField] GameObject healingZoneOfTorch;
    [SerializeField] GameObject fireOfTorch;
    [SerializeField] SoundManager _soundmanager; // FMOD SoundManager
    private s_PlayerCollider _playerCollider;
    bool torchActive = false;
    bool _alreadyKindledAFire = false;
    void Start() {
        _type = "torch";
        _playerCollider = GameObject.Find("PlayerAnimated").GetComponent<s_PlayerCollider>();
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<SoundManager>();
        lightOfTorch.SetActive(false);
        healingZoneOfTorch.SetActive(false);
        fireOfTorch.SetActive(false);
    }
    public override void Interact() {
        if(!torchActive) {
            if(!_alreadyKindledAFire) {
                _alreadyKindledAFire = true;
            }
            _soundmanager.RegisterEventEmitter(this.transform.gameObject,"event:/SFX/Torch");
            _playerCollider.gameObject.GetComponent<PlayerControl>().PlayInteractAnimation();
            lightOfTorch.SetActive(true);
            healingZoneOfTorch.SetActive(true);
            fireOfTorch.SetActive(true);
            torchActive = true;
        }
        else {
            _soundmanager.UnregisterEventEmitter(this.transform.gameObject,"event:/SFX/Torch");
            _playerCollider.gameObject.GetComponent<PlayerControl>().PlayInteractAnimation();
            lightOfTorch.SetActive(false);
            healingZoneOfTorch.SetActive(false);
            fireOfTorch.SetActive(false);
            torchActive = false;
        }
    }
}
