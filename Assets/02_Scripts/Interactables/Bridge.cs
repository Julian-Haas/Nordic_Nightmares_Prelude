using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bridge : Interactable
{
    public GameObject Bridgebody, _Hole1, _Hole2, _player;
    private short _bridgestate = 0;
    public GameObject _colliderToDelete;
    private Inventory _inventory;
    [SerializeField] SoundManager _soundmanager; // FMOD SoundManager
    public Slider _slider;
    public float _buildspeed = 0.02f;
    private bool _triedToInteractBefore = false;
    public Animator _animator;

    void Start() {
        _type = "bridge";
        _inventory = GameObject.Find("Inventory").GetComponentInChildren<Inventory>();
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<SoundManager>();
        _Hole1.SetActive(false);
        _Hole2.SetActive(false);
        _player = GameObject.Find("PlayerAnimated");
        //_slider.value = 0.5f;
        //Debug.Log("_slider.value: " + _slider.value);

    }

    public override void Interact() {
        if(_bridgestate != 2) {
            if(_inventory.TryToUsePlank()) {
                switch(_bridgestate) {
                    case 0:
                        _Hole1.SetActive(true);
                        _bridgestate = 1;
                        _soundmanager.PlaySound2D("event:/SFX/BridgeStep");
                        _player.GetComponent<PlayerControl>().PlayInteractAnimation();
                        _slider.value = 0.0f;
                        _player.GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("I need another plank to fully repair it.");
                        break;
                    case 1:
                        _Hole2.SetActive(true);
                        _bridgestate = 2;
                        DisplayInteractionText(false);
                        Destroy(_colliderToDelete);
                        _soundmanager.PlaySound2D("event:/SFX/BridgeCompleted");
                        _player.GetComponent<PlayerControl>().PlayInteractAnimation();
                        _player.GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("Now I can cross it.");
                        _animator.SetTrigger("IsFeedback");
                        break;
                    default:
                        break;
                }
            }
            else {
                if(!_triedToInteractBefore) {
                    _player.GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("I need planks to repair it.");
                    _triedToInteractBefore = true;
                }
                else {
                    _player.GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("I still need to find a plank to repair it.");

                }
            }
        }
    }
}