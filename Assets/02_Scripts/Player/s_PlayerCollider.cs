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
    public string CurrentGround = "Regular";
    private Rigidbody _rb;
    public float VELOCITY = 0.0f;
    private Player_Ground_Texture_Check _textureCheck;
    bool _wasAlreadyCloseToWater = false;
    bool _hasAlreadyEnteredAShrine = false;
    bool _hasAlreadySeenGuidancePost = false;
    [SerializeField] GameObject GuidanceTooltip;
    [SerializeField] Animator _EyeAnimator;
    public static s_PlayerCollider Instance;
    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
    }
    private void Start() {
        InGameUI = GameObject.Find("InGame_Canvas");
        _rb = GetComponent<Rigidbody>();
        _textureCheck = GetComponent<Player_Ground_Texture_Check>();
    }
    private void Update() {
        if(Time.timeScale == 1) {
            if(_rb.velocity.magnitude >= 0.000001f) { // && !_inSafeZone ???
                VELOCITY = _rb.velocity.magnitude;
                _textureCheck.CheckGroundTexture();
            }
        }
        GuidanceTooltip.transform.rotation = new Quaternion(0.109381668f,-0.875426114f,0.234569758f,0.408217877f);
    }
    void OnTriggerEnter(Collider other) {
        switch(other.gameObject.tag) {
            case "Naddi":
                DeathManager.Instance.PlayerDies(true);
                break;
            case "Finish":
                SoundManager.Instance.musicInstance.SetParameter("Sanity",1.0f);
                SoundManager.Instance.musicInstance.SetParameter("Level",0.5f);
                SoundManager.Instance.musicInstance.SetParameter("NaddiR",1.0f);
                SoundManager.Instance.musicInstance.SetParameter("NaddiHunt",0.0f);
                SoundManager.Instance.musicInstance.SetParameter("Level",9);
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
            case "SavePoint":
                _EyeAnimator.SetTrigger("IsHidden");
                break;
            case "DeathZone":
                DeathManager.Instance.PlayerDies(false);
                break;
            case "Interactable":
                InteractableManager.Instance.AddInteractable(other.GetComponentInParent<Interactable>());
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
            case "Interactable":
                InteractableManager.Instance.RemoveInteractable(other.transform.GetComponentInParent<Interactable>());
                break;
            default:
                break;
        }
    }
}


//    bool _hasAlreadyEnteredAShadow = false;

//    case "Shadow":
//    if(!_hasAlreadyEnteredAShadow) {
//        this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I can hide in here, but it also scares me.");
//        _hasAlreadyEnteredAShadow = true;
//    }
//    SoundManager.Instance.PlaySound3D("event:/SFX/PlayerHide",this.transform.position);
//    _EyeAnimator.SetTrigger("IsHidden");
//    break;

//case "Shadow":
//    SoundManager.Instance.PlaySound3D("event:/SFX/PlayerUnhide",this.transform.position);
//    _EyeAnimator.SetTrigger("IsExposed");
//    break;