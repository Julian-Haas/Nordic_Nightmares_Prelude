using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class s_PlayerCollider : MonoBehaviour
{
    // refactor
    public bool _inSafeZone = true;
    public bool _inShadow = true;
    // refactor
    public GameObject InGameUI;
    public Material material;
    public VisualEffect _visible, _invisible;
    public GameObject _visibleObject, _invisibleObject;
    public List<Interactable> closeInteractables = new List<Interactable>();
    private Interactable lastClosestInteractable;
    public string CurrentGround = "Regular";
    private Rigidbody _rb;
    public float VELOCITY = 0.0f;
    private SoundManager _soundManager;
    private Player_Ground_Texture_Check _textureCheck;
    public bool _alreadyCloseToAFire = false;
    bool _wasAlreadyCloseToABridge = false;
    bool _wasAlreadyCloseToWater = false;
    bool _hasAlreadyCollectedAPlank = false;
    bool _hasAlreadyEnteredAShadow = false;
    bool _hasAlreadyEnteredAShrine = false;
    bool _hasAlreadySeenGuidancePost = false;
    [SerializeField] GameObject GuidanceTooltip;
    [SerializeField] Animator _EyeAnimator;
    private void Start() {
        _soundManager = GameObject.Find("SoundManager").GetComponentInChildren<SoundManager>();
        InGameUI = GameObject.Find("InGame_Canvas");
        lastClosestInteractable = (Interactable) Interactable.FindObjectOfType(typeof(Interactable));
        _rb = GetComponent<Rigidbody>();
        _textureCheck = GetComponent<Player_Ground_Texture_Check>();
    }
    public void GatheredPlank() {
        _hasAlreadyCollectedAPlank = true;
    }
    private void Update() {
        if(Time.timeScale == 1) {
            if(_rb.velocity.magnitude >= 0.000001f) { // && !_inSafeZone ???
                VELOCITY = _rb.velocity.magnitude;
                _textureCheck.CheckGroundTexture();
            }
            updateClosestInteractable();
        }
        GuidanceTooltip.transform.rotation = new Quaternion(0.109381668f,-0.875426114f,0.234569758f,0.408217877f);
    }
    private void updateClosestInteractable() {
        Interactable objectToSwap;
        for(int i = 0; i < closeInteractables.Count - 1; i++) {
            if((this.transform.position - closeInteractables[i].transform.position).sqrMagnitude > (this.transform.position - closeInteractables[i + 1].transform.position).sqrMagnitude) {
                if(i == 0) {
                    closeInteractables[0].GetComponentInParent<Interactable>()?.DisplayInteractionText(false);
                    closeInteractables[1].GetComponentInParent<Interactable>()?.DisplayInteractionText(true);
                }
                objectToSwap = closeInteractables[i];
                closeInteractables[i] = closeInteractables[i + 1];
                closeInteractables[i + 1] = objectToSwap;
            }
        }
    }
    public void interact(bool started) {
        if(closeInteractables.Count > 0) {
            if(!closeInteractables[0].Interact(started)) {
                leaveColliderOfInteractable(closeInteractables[0]);
            }
        }
    }
    public bool IsHidden() {
        return true;
    }
    public void leaveColliderOfInteractable(Interactable InteractableToLeave) {
        InteractableToLeave.GetComponentInParent<Interactable>()?.DisplayInteractionText(false);
        closeInteractables.Remove(InteractableToLeave.GetComponentInParent<Interactable>());
        if(closeInteractables.Count == 1) {
            closeInteractables[0].GetComponentInParent<Interactable>()?.DisplayInteractionText(true);
        }
    }
    void OnTriggerEnter(Collider other) {
        switch(other.gameObject.tag) {
            case "Naddi":
                DeathManager.Instance.PlayerDies(true);
                break;
            case "Finish":
                _soundManager.musicInstance.SetParameter("Sanity",1.0f);
                _soundManager.musicInstance.SetParameter("Level",0.5f);
                _soundManager.musicInstance.SetParameter("NaddiR",1.0f);
                _soundManager.musicInstance.SetParameter("NaddiHunt",0.0f);
                _soundManager.musicInstance.SetParameter("Level",9);
                InGameUI.GetComponent<InGame_UI>().Win();
                break;
            case "Shrine":
                if(!_hasAlreadyEnteredAShrine) {
                    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I feel safe in here.");
                    _hasAlreadyEnteredAShrine = true;
                }
                other.transform.GetComponentInParent<S_Shrine>().EnterShrine();
                _EyeAnimator.SetTrigger("IsHidden");
                break;
            case "GuidancePost":
                if(!_hasAlreadySeenGuidancePost) {
                    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("One way might be shorter, but also more dangerous.");
                    _hasAlreadySeenGuidancePost = true;
                }
                break;
            case "Shadow":
                if(!_hasAlreadyEnteredAShadow) {
                    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I can hide in here, but it also scares me.");
                    _hasAlreadyEnteredAShadow = true;
                }
                _soundManager.PlaySound3D("event:/SFX/PlayerHide",this.transform.position);
                _EyeAnimator.SetTrigger("IsHidden");
                break;
            case "SavePoint":
                _EyeAnimator.SetTrigger("IsHidden");
                break;
            case "DeathZone":
                DeathManager.Instance.PlayerDies(false);
                break;
            case "Interactable":
                closeInteractables.Add(other.GetComponentInParent<Interactable>());
                if(closeInteractables.Count == 1) {
                    other.GetComponentInParent<Interactable>()?.DisplayInteractionText(true);
                    string typeOfOther = other.GetComponentInParent<Interactable>()?.getType();
                    switch(typeOfOther) {
                        case "torch":
                            if(!_alreadyCloseToAFire) {
                                this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("Maybe I should kindle this fire?");
                            }
                            break;
                        case "plank":
                            if(!_hasAlreadyCollectedAPlank) {
                                this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("This could be useful.");
                            }
                            break;
                        case "bridge":
                            if(!_wasAlreadyCloseToABridge) {
                                this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("Maybe there is a way to repair this bridge.");
                                _wasAlreadyCloseToABridge = true;
                            }
                            break;
                        case "seashell":

                            break;
                        default:
                            break;
                    }
                }
                break;
            case "nearWater":
                if(!_wasAlreadyCloseToWater) {
                    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I can't swim, I will die if I step into the water.");
                    _wasAlreadyCloseToWater = true;
                }
                break;
            case "Triggerable":
                other.GetComponentInParent<Triggerable>().Trigger();
                break;
            default:
                break;
        }
    }
    void OnTriggerExit(Collider other) {
        switch(other.gameObject.tag) {
            case "Shrine":
                other.transform.GetComponentInParent<S_Shrine>().LeaveShrine();
                _EyeAnimator.SetTrigger("IsExposed");
                break;
            case "Shadow":
                _soundManager.PlaySound3D("event:/SFX/PlayerUnhide",this.transform.position);
                _EyeAnimator.SetTrigger("IsExposed");
                break;
            case "Interactable":
                leaveColliderOfInteractable(other.transform.GetComponentInParent<Interactable>());
                break;
            default:
                break;
        }
    }
}