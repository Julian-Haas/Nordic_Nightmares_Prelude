using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bridge_backup : Interactable
{
    public GameObject Bridgebody1, Bridgebody2, Bridgebody3, _player;
    private short _bridgestate = 0;
    public GameObject _colliderToDelete;
    private Inventory _inventory;
    [SerializeField] SoundManager _soundmanager; // FMOD SoundManager
    public Slider _slider;
    public float _buildspeed = 0.02f;
    private bool _triedToInteractBefore = false;

    void Start() {
        _type = "bridge";
        _inventory = GameObject.Find("Inventory").GetComponentInChildren<Inventory>();
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<SoundManager>();
        Bridgebody2.SetActive(false);
        Bridgebody3.SetActive(false);
        _player = GameObject.Find("PlayerAnimated");
        //_slider.value = 0.5f;
        //Debug.Log("_slider.value: " + _slider.value);

    }

    public override bool Interact(bool started) {
        if(_slider.value < 0.01f) {
            if(_inventory.TryToUsePlank()) {
                _slider.value += _buildspeed;
                return true;
            }
            else {
                if(!_triedToInteractBefore) {
                    _player.GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("I need planks to repair it.");
                    _triedToInteractBefore = true;
                    return true;
                }
                else {
                    _player.GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("I still need to find a plank to repair it.");
                    return true;
                }
            }
        }
        if(_slider.value >= 1f) {
            switch(_bridgestate) {
                case 0:
                    Bridgebody1.SetActive(false);
                    Bridgebody2.SetActive(true);
                    _bridgestate = 1;
                    _soundmanager.PlaySound2D("event:/SFX/BridgeCompleted");
                    _player.GetComponent<PlayerControl>().PlayInteractAnimation();
                    _slider.value = 0.0f;
                    _player.GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("I need another plank to fully repair it.");
                    return true;
                case 1:
                    Bridgebody2.SetActive(false);
                    Bridgebody3.SetActive(true);
                    _bridgestate = 2;
                    DisplayInteractionText(false);
                    Destroy(_colliderToDelete);
                    _soundmanager.PlaySound2D("event:/SFX/BridgeCompleted");
                    _player.GetComponent<PlayerControl>().PlayInteractAnimation();
                    _player.GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("Now I can cross it.");
                    return true;
                default:
                    return true;
            }
        }
        _slider.value += _buildspeed;
        return true;
    }
}