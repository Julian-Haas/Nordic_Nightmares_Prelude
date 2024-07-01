using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : Interactable
{
    [SerializeField] GameObject lightOfTorch;
    [SerializeField] GameObject healingZoneOfTorch;
    [SerializeField] GameObject fireOfTorch;
    private s_PlayerCollider _playerCollider;
    [SerializeField] bool torchActive = false;
    bool _alreadyKindledAFire = false;
    void Start() {
        _type = "torch";
        _playerCollider = GameObject.Find("PlayerAnimated").GetComponent<s_PlayerCollider>();
        lightOfTorch.SetActive(false);
        healingZoneOfTorch.SetActive(false);
        fireOfTorch.SetActive(false);
    }
    public override void Interact() {
        if(!torchActive) {
            if(!_alreadyKindledAFire) {
                _alreadyKindledAFire = true;
            }
            lightOfTorch.SetActive(true);
            healingZoneOfTorch.SetActive(true);
            fireOfTorch.SetActive(true);
            torchActive = true;
            SoundManager.Instance.RegisterEventEmitter(this.transform.gameObject,"event:/SFX/Torch");
            _playerCollider.gameObject.GetComponent<PlayerControl>().PlayInteractAnimation();
        }
        else {
            Debug.Log("test");
            SoundManager.Instance.UnregisterEventEmitter(this.transform.gameObject,"event:/SFX/Torch");
            _playerCollider.gameObject.GetComponent<PlayerControl>().PlayInteractAnimation();
            lightOfTorch.SetActive(false);
            healingZoneOfTorch.SetActive(false);
            fireOfTorch.SetActive(false);
            torchActive = false;
        }
    }
}
