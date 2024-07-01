using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance {
        get; private set;
    }
    private bool _isHavingPlank;
    private bool _isHavingSeashell;
    private bool _gatheredFirstPlank;
    private GameObject _plank = null;
    public Animator _animator;
    //private bool _alreadyCollectedAPlank = false;
    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        else {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    void Start() {
        _plank = s_PlayerCollider.Instance.transform.Find("A_Character_03").transform.Find("Rig_Player").transform.Find("Mch_snap_Wood").transform.Find("Wood_property").transform.Find("PlankFeedback").gameObject;
        _plank.SetActive(false);
        _animator = _plank.GetComponent<Animator>();
    }
    public bool TryToUsePlank() {
        if(_isHavingPlank) {
            _isHavingPlank = false;
            _plank.SetActive(false);
            s_PlayerCollider.Instance.GetComponent<PlayerControl>().PlayInteractAnimation();
            return true;
        }
        else {
            return false;
        }
    }
    public bool TryToGatherPlank() {
        if(!_isHavingPlank) {
            _isHavingPlank = true;
            SoundManager.Instance.PlaySound2D("event:/SFX/PickUpPlank");
            s_PlayerCollider.Instance.GetComponent<PlayerControl>().PlayInteractAnimation();
            _plank.SetActive(true);
            _animator.SetTrigger("IsFeedback");
            if(!_gatheredFirstPlank) {
                //_alreadyCollectedAPlank = true;
                //    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("This could be useful.");
            }
            return true;
        }
        else {
            SoundManager.Instance.PlaySound2D("event:/SFX/InventoryFull");
            return false;
        }
    }
    public bool TryToPickUpSeashell() {
        if(!_isHavingSeashell) {
            _isHavingSeashell = true;
            s_PlayerCollider.Instance.GetComponent<PlayerControl>().PlayInteractAnimation();
            _animator.SetTrigger("IsFeedback");
            //implement sound
            //implement vfx
            return true;
        }
        else {
            SoundManager.Instance.PlaySound2D("event:/SFX/InventoryFull");
            return false;
        }
    }
    public bool TryToUseSeashell() {
        if(_isHavingSeashell) {
            _isHavingSeashell = false;
            return true;
        }
        else {
            return false;
        }
    }
}