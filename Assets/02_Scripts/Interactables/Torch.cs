using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : Interactable
{    
    [SerializeField] GameObject lightOfTorch;
    [SerializeField] GameObject healingZoneOfTorch;
    [SerializeField] GameObject fireOfTorch;
    [SerializeField] s_SoundManager _soundmanager; // FMOD SoundManager
    private s_PlayerCollider _playerCollider;

    bool _hasAlreadyKindledAFire= false;

    private NoiseEmitter _noise;

    void Start()
    {
        _type = "torch";
        _playerCollider = GameObject.Find("PlayerAnimated").GetComponent<s_PlayerCollider>();
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        lightOfTorch.SetActive(false);
        healingZoneOfTorch.SetActive(false);
        fireOfTorch.SetActive(false);
        _noise = gameObject.GetComponentInChildren<NoiseEmitter>();
        
    }

    public override bool Interact(bool started)
    {
        if (started)
        {

            if (!lightOfTorch.activeInHierarchy)
            {
                if (!_hasAlreadyKindledAFire)
                {
                    _playerCollider._alreadyCloseToAFire = true;
                    _hasAlreadyKindledAFire = true;
                }
                WorldStateData.Instance.UpdateInteractableState(_index, 1);
                _soundmanager.RegisterEventEmitter(this.transform.gameObject, "event:/SFX/Torch");
                _playerCollider.gameObject.GetComponent<PlayerControl>().PlayInteractAnimation();
                lightOfTorch.SetActive(true);
                healingZoneOfTorch.SetActive(true);
                fireOfTorch.SetActive(true);
                _noise.MakeSound(2);
                _state = 1;
                UpdateState();
            }
            else
            {
                _playerCollider.ExtinguishFire();
                WorldStateData.Instance.UpdateInteractableState(_index, 0);
                _soundmanager.UnregisterEventEmitter(this.transform.gameObject, "event:/SFX/Torch");
                _playerCollider.gameObject.GetComponent<PlayerControl>().PlayInteractAnimation();
                lightOfTorch.SetActive(false);
                healingZoneOfTorch.SetActive(false);
                fireOfTorch.SetActive(false);
                _noise.MakeSound(1);
                _state = 0;
                UpdateState();
            }
        }
        return true;
    }
}