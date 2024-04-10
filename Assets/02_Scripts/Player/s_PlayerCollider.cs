using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;

public class s_PlayerCollider : MonoBehaviour, IPlayerAction
{
    [Tooltip("Sanity Influence Rates in Percent")]
    public int General = 5, InShadow = 15, SafeZone = 5, HealingZone = 15;
    public bool _inHealingZone = false, _inSafeZone = false, _isMad = false, _inShadow = false;
    [SerializeField] public float _influence = 0.0f, _sanity = 100.0f;
    private float _sanityShift = 0.0f;
    public GameObject InGameUI;
    private InGame_UI _UI_Instance;
    public Material material;
    public VisualEffect _visible, _invisible;
    public GameObject _visibleObject, _invisibleObject;
    private Animator _hiddenStatusVFX;
    public List<Interactable> closeInteractables = new List<Interactable>();
    private Interactable lastClosestInteractable;
    public string CurrentGround = "Regular";
    [Header("Regular Ground Noise")]
    public float QuietVolume;
    public float MediumVolume; 
    public float LoudVolume;
    private Rigidbody _rb;
    public float VELOCITY = 0.0f;
    private bool _sanityEmptySoundPlayed = false;
    private bool _sanityLowSoundPlaying = false;
    private s_SoundManager _soundManager;
    private Player_Ground_Texture_Check _textureCheck;
    //private NoiseEmitter _emitter;
    private Slider _sanitySlider;
    private NaddiTrackTarget _track;
    [SerializeField] float _cooldownOfSanityWarnings = 5.0f;
    bool _sanityOverlayExplained = false;
    public bool _alreadyCloseToAFire = false;
    bool _wasAlreadyCloseToABridge = false;
    bool _wasAlreadyCloseToWater= false;
    bool _hasAlreadyEnteredASafeZone = false;
    bool _hasAlreadyCollectedAPlank = false;
    bool _hasAlreadyEnteredAShadow = false;
    bool _hasAlreadyEnteredAShrine = false;
    bool _hasAlreadySeenGuidancePost = false;
    [SerializeField] GameObject GuidanceTooltip;
    [SerializeField] Animator _EyeAnimator;
    private DeathManager _deathManager;

    private void Awake()
    {
        _deathManager = this.GetComponent<DeathManager>();
        _soundManager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        _sanity = 100.0f;
        InGameUI = GameObject.Find("InGame_Canvas");
        _UI_Instance = InGameUI.GetComponent<InGame_UI>();
        _influence += (float)General;
        lastClosestInteractable = (Interactable)Interactable.FindObjectOfType(typeof(Interactable));
        //_emitter = GetComponentInChildren<NoiseEmitter>();
        _rb = GetComponent<Rigidbody>();
        _textureCheck = GetComponent<Player_Ground_Texture_Check>();
        _sanitySlider = GameObject.Find("SanitySlider")?.GetComponent<Slider>();
        _track = GetComponentInChildren<NaddiTrackTarget>();
    }

    public void GatheredPlank()
    {
        _hasAlreadyCollectedAPlank = true;
    }

    private void Update()
    {
        if(Time.timeScale == 1)
        {
            if (_rb.velocity.magnitude >= 0.000001f && !_inSafeZone)
            {
                VELOCITY = _rb.velocity.magnitude;
                //_emitter.MakeSound();
                _textureCheck.CheckGroundTexture();
                _track.LeaveTrackPoint();
            }
            sanityUpdate();
            updateClosestInteractable();
        }
        GuidanceTooltip.transform.rotation = new Quaternion(0.109381668f, -0.875426114f, 0.234569758f, 0.408217877f);
    }
    private void updateClosestInteractable()
    {
        Interactable objectToSwap;
        for (int i = 0; i < closeInteractables.Count - 1; i++)
        {
            if ((this.transform.position - closeInteractables[i].transform.position).sqrMagnitude > (this.transform.position - closeInteractables[i + 1].transform.position).sqrMagnitude)
            {
                if (i == 0)
                {
                    closeInteractables[0].GetComponentInParent<Interactable>()?.DisplayInteractionText(false);
                    closeInteractables[1].GetComponentInParent<Interactable>()?.DisplayInteractionText(true);
                }
                objectToSwap = closeInteractables[i];
                closeInteractables[i] = closeInteractables[i + 1];
                closeInteractables[i+1] = objectToSwap;
            }
        }
    }
    public void interact(bool started)
    {
        if (closeInteractables.Count > 0)
        {
            if(!closeInteractables[0].Interact(started))
            {
                leaveColliderOfInteractable(closeInteractables[0]);
            }
        }
    }
    void sanityUpdate()
    {
        if(_inHealingZone)
        {
            _influence = -HealingZone;
        }
        else if (_inSafeZone)
        {
            _influence = -SafeZone;
        }
        else if (_inShadow) 
        {
            _influence = InShadow;
        }
        else
        {
            _influence = General;
        }
        _sanityShift = -1 * (_influence / 100 ) * Time.deltaTime;
        if (_sanity <= 70.0f && !_sanityOverlayExplained)
        {
            this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I shouldn't stay far from light for too long.");
            _sanityOverlayExplained = true;
        }
        _sanity += _sanityShift;
        _sanity = Mathf.Clamp(_sanity, 0.0f, 100.0f);
        float _sanityMat;
        if (_sanity < 70.0f)
        {
            _sanityMat = 0.85f - ((_sanity) / 85.0f);
        }
        else
        {
            _sanityMat = 0.0f;
        }
        _sanityMat = Mathf.Clamp(_sanityMat, 0.0f, 1.0f);
        material.SetFloat("_FadeInMadness", _sanityMat);
        _sanityShift = 0.0f;
        WorldStateData.Instance.UpdatePlayerSanity(_sanity);
        _soundManager.musicInstance.SetParameter("Sanity", _sanity/100.0f);
        _soundManager.ambientInstance.SetParameter("Zoom", _sanityMat);
    }


    public bool IsHidden()
    {
        return _inShadow;
    }
    public void leaveColliderOfInteractable(Interactable InteractableToLeave)
    {
        InteractableToLeave.GetComponentInParent<Interactable>()?.DisplayInteractionText(false);
        closeInteractables.Remove(InteractableToLeave.GetComponentInParent<Interactable>());
        if (closeInteractables.Count == 1)
        {
            closeInteractables[0].GetComponentInParent<Interactable>()?.DisplayInteractionText(true);
        }
    }
    public void ExtinguishFire()
    {
        _inHealingZone = false;
    }
    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Naddi":
                _deathManager.PlayerDies(true);
                break;
            case "Finish":
                _soundManager.musicInstance.SetParameter("Sanity", 1.0f);
                _soundManager.musicInstance.SetParameter("Level", 0.5f);
                _soundManager.musicInstance.SetParameter("NaddiR", 1.0f);
                _soundManager.musicInstance.SetParameter("NaddiHunt", 0.0f);
                _soundManager.musicInstance.SetParameter("Level", 9);
                InGameUI.GetComponent<InGame_UI>().Win();
                break;
            case "HealingZone":
                _inHealingZone = true;
                break;
            case "Shrine":
                if (!_hasAlreadyEnteredAShrine)
                {
                    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I feel safe in here.");
                    _hasAlreadyEnteredAShrine = true;
                }
                other.transform.GetComponentInParent<S_Shrine>().EnterShrine();
                _inSafeZone = true;
                _EyeAnimator.SetTrigger("IsHidden");
                break;
            case "GuidancePost":
                if (!_hasAlreadySeenGuidancePost)
                {
                    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("One way might be shorter, but also more dangerous.");
                    _hasAlreadySeenGuidancePost = true;
                }
                break;
            case "Shadow":
                if (!_hasAlreadyEnteredAShadow)
                {
                    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I can hide in here, but it also scares me.");
                    _hasAlreadyEnteredAShadow = true;
                }
                _soundManager.PlaySound3D("event:/SFX/PlayerHide", this.transform.position);
                _inShadow = true; 
                _EyeAnimator.SetTrigger("IsHidden");
                break;
            case "SavePoint":
                _inShadow = true;
                _EyeAnimator.SetTrigger("IsHidden");
                break;
            case "DeathZone":
                _deathManager.PlayerDies(false);
                break;
            case "Interactable":
                closeInteractables.Add(other.GetComponentInParent<Interactable>());
                if (closeInteractables.Count == 1)
                {
                    other.GetComponentInParent<Interactable>()?.DisplayInteractionText(true);; 
                    string typeOfOther = other.GetComponentInParent<Interactable>()?.getType();
                    switch (typeOfOther)
                    {
                        case "torch":
                            if (!_alreadyCloseToAFire)
                            {
                                this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("Maybe I should kindle this fire?");
                            }
                            break;
                        case "plank":
                            if (!_hasAlreadyCollectedAPlank)
                            {
                                this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("This could be useful.");
                            }
                            break;
                        case "bridge":
                            if (!_wasAlreadyCloseToABridge)
                            {
                                this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("Maybe there is a way to repair this bridge.");
                                _wasAlreadyCloseToABridge = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case "nearWater":
                if (!_wasAlreadyCloseToWater)
                {
                    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("I can't swim, I will die if I step into the water.");
                    _wasAlreadyCloseToWater = true;
                }
                break;
            default:
                break;
        }
    }
    void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "HealingZone":
                _inHealingZone = false;
                break;
            case "Shrine":
                other.transform.GetComponentInParent<S_Shrine>().LeaveShrine();
                _inSafeZone = false;
                _EyeAnimator.SetTrigger("IsExposed");
                break;
            case "Shadow":
                _inShadow = false;
                _soundManager.PlaySound3D("event:/SFX/PlayerUnhide", this.transform.position);
                _EyeAnimator.SetTrigger("IsExposed");
                break;
            case "Interactable":
                leaveColliderOfInteractable(other.transform.GetComponentInParent<Interactable>());
                break;
            default:
                break;    
        }
    }
    public bool CheckIsHidden(GameObject enemy)
    {
        return _inShadow;
    }    
    bool JuliansTestBool = false;
    public void JuliansTestFunktion()
    {
        Cursor.visible = false;
        Debug.Log("p gedrückt");
        if (!JuliansTestBool)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            JuliansTestBool = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            JuliansTestBool = false;
        }
    }
}