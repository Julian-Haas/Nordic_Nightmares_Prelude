using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;


public class NaddiDebugMenu : MonoBehaviour
{

    private enum DebugMenuStates
    {
        Patrol,
        LookForPlayer,
        Chase
    }
    [SerializeField]
    private NaddiHearing _hearing;
    [SerializeField]
    private NaddiStateMaschine naddiStateMachine; 
    [SerializeField]
    private TMP_Dropdown NaddiStatesDD;
    [SerializeField]
    private Slider AttackTriggerSlider;
    [SerializeField]
    private Slider LookForPlayerTriggerSlider;
    [SerializeField]
    private Slider SpeedSlider;
    [SerializeField]
    private Slider ViewRangeSlider;
    [SerializeField]
    private Slider HearingRangeSlider;
    [SerializeField]
    private Slider HalfConeRadiusSlider;
    [SerializeField]
    private TextMeshProUGUI ATValTXT;
    [SerializeField]
    private TextMeshProUGUI LFPTValTXT;
    [SerializeField]
    private TextMeshProUGUI ViewRangeValTXT;
    [SerializeField]
    private TextMeshProUGUI ConeRadiusValTXT;
    [SerializeField]
    private TextMeshProUGUI HearingRangeValTXT;
    [SerializeField]
    private Naddi _naddi;
    [SerializeField]
    private NaddiViewField _viewField; 
    [SerializeField]
    private NaddiValueStorage val;
    [SerializeField]
    public TextMeshProUGUI SpeedText;
    [SerializeField]
    private SaveManager saveManager;

    // Start is called before the first frame update
    private void Awake()
    {
        saveManager.LoadData();
    }
    void Start()
    {
        NaddiStatesDD.AddOptions(new List<string>(DebugMenuStates.GetNames(typeof(DebugMenuStates))));
        AttackTriggerSlider.value = _hearing.AttackTrigger;
        LookForPlayerTriggerSlider.value = _hearing.LookForPlayerTrigger;
        HalfConeRadiusSlider.value = _viewField.HalfAngleDegree;
        SpeedSlider.value = _naddi.Speed;
        ViewRangeSlider.value = _viewField.ConeRadius;
        HearingRangeSlider.value = _hearing.MaxDistance;

    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
        SetAttackTriggerVal();

    }

    public void OnDropdownValueChanged()
    {
        // Hole den ausgew?hlten Wert aus dem Dropdown-Men?
        switch (NaddiStatesDD.value)
        {
            case 0:
                naddiStateMachine.StartDigging();
                break;
            case 1:
                naddiStateMachine.LookForPlayer();
                break;
            case 2:
                naddiStateMachine.FoundPlayer();
                break;
        }
    }
    public void ApplyAttackTrigger()
    {
        val.AttackTriggerVal = AttackTriggerSlider.value;
        saveManager.SafeData(); 
    }

    public void RevertAttackTrigger()
    {
        AttackTriggerSlider.value=val.AttackTriggerVal;
        _hearing.AttackTrigger = AttackTriggerSlider.value;
        ATValTXT.text = "Value: " + AttackTriggerSlider.value.ToString();
    }

    public void SetAttackTriggerVal() 
    {
        _hearing.AttackTrigger = AttackTriggerSlider.value; 
        ATValTXT.text = "Value: " + AttackTriggerSlider.value.ToString(); 
    }

    public void SetLookForPlayerTrigger()
    {
        _hearing.LookForPlayerTrigger = LookForPlayerTriggerSlider.value;
        LFPTValTXT.text = "Value: " + LookForPlayerTriggerSlider.value;
    }


    public void ApplyLookTrigger()
    {
        val.LookForPlayerVal = LookForPlayerTriggerSlider.value;
        saveManager.SafeData();
    }

    public void RevertLookTrigger()
    {
        LookForPlayerTriggerSlider.value = val.LookForPlayerVal;
        _hearing.LookForPlayerTrigger = LookForPlayerTriggerSlider.value;
        LFPTValTXT.text = "Value: " + LookForPlayerTriggerSlider.value.ToString();
    }

    public void SetNaddiSpeed()
    {
        _naddi.Speed = SpeedSlider.value;
        SpeedText.text = "Value: " + SpeedSlider.value;
    }


    public void ApplyNaddiSpeed()
    {
        val.NaddiSpeed = SpeedSlider.value;
        saveManager.SafeData(); 
    }

    public void RevertNaddiSpeed()
    {
        SpeedSlider.value = val.NaddiSpeed;
        _naddi.Speed = SpeedSlider.value;
        SpeedText.text = "Value: " + SpeedSlider.value.ToString();
    }


    public void SetNaddiViewRange()
    {
        _viewField.ConeRadius = ViewRangeSlider.value;
        ViewRangeValTXT.text = "Value: " + ViewRangeSlider.value;
    }


    public void ApplyNaddiViewRange()
    {
        val.ViewRange = ViewRangeSlider.value;
        saveManager.SafeData(); 
    }

    public void RevertNaddiViewRange()
    {
        ViewRangeSlider.value = val.ViewRange;
        _viewField.ConeRadius = ViewRangeSlider.value;
        ViewRangeValTXT.text = "Value: " + ViewRangeSlider.value.ToString();
    }

    public void SetNaddiHearRange()
    {
        _hearing.MaxDistance = HearingRangeSlider.value;
        HearingRangeValTXT.text = "Value: " + HearingRangeSlider.value.ToString();
    }


    public void ApplyNaddiHearRange()
    {
        val.HearingRange = HearingRangeSlider.value;
        saveManager.SafeData(); 
    }

    public void RevertNaddiHearRange()
    {
        HearingRangeSlider.value = val.HearingRange;
        _hearing.MaxDistance = HearingRangeSlider.value;
        HearingRangeValTXT.text = "Value: " + HearingRangeSlider.value.ToString();
    }
    public void SetNaddiHalfConeRadius()
    {
        _viewField.HalfAngleDegree = HalfConeRadiusSlider.value;
        ConeRadiusValTXT.text = "Value: " + HalfConeRadiusSlider.value.ToString();
    }


    public void ApplyNaddiHalfConeRadius()
    {
        val.HalfViewRadius = HalfConeRadiusSlider.value;
        saveManager.SafeData(); 
    }

    public void RevertNaddiHalfConeRadius()
    {
        HalfConeRadiusSlider.value = val.HalfViewRadius;
        _viewField.HalfAngleDegree = HalfConeRadiusSlider.value;
        ConeRadiusValTXT.text = "Value: " + HearingRangeSlider.value.ToString();
    }

}
