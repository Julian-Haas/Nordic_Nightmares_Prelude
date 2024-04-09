using UnityEngine;


/*
STATES
## - BaseState (unnumbered because not a state that will be entered)
00 - RegularState aka Idle (patrols along path, pauses, no POIs)
01 - SuspiciousState (slows down, more attentive , pauses, inspects POIs)
02 - AlertState(knows smth is off, heightendet senses, no pauses, inspects POIs, inspects CAs)
03 - Aware (knows player is around, actively searches for them, no pauses, inspects CAs)
04 - Hunt (knows players location, hunts them, no pauses, no POIs, no CAs, follows Player)

10 - Pause (takes breaks at certain points, idles for a bit before continuing)

20 - InspectPOI() (stops on Path, inspects Point of Interest)
21 - InspectCA() (leaves Path, inspects Cautious Area)
 
 */



 public class NaddiStateMachine : StateMachine
{
    public float SPEED;
    public string ActiveState;
    public float timeIdled = 0.0f;

    [Header("Movement Speed")]
    public float RegularSpeed = 5.0f;
    public float SuspiciousSpeed = 3.0f;
    public float AlertSpeed = 4.0f;
    public float AwareSpeed = 2.0f;
    public float HuntSpeed = 10.0f;
    public float RotationSpeed = 10.0f;

    [Header("Distance when hunting but player is in SafeZone")]
    public float HuntDistance = 15.0f;

    [Header("PAUSE & INSPECTION TIMES IN SECONDS")]
    [Header("Idles at Pause points (set along path on")]
    [Tooltip("Pause time when Regular")]
    public float PauseRegular = 10.0f;
    [Tooltip("Pause time when Suspicious")]
    public float PauseSuspicious = 5.0f;
    [Header("POI = Point of Interest (set along path)")]
    [Tooltip("Inspect time when Suspicious")]
    public float InspectPOISuspicious = 10.0f;
    [Tooltip("Inspect time when Alert")]
    public float InspectPOIAlert = 5.0f;
    [Header("CA = Cautious Area (where last Noise / PlayerActivity was registered)")]
    [Tooltip("Inspect time when Alert")]
    public float InspectCAAlert = 10.0f;
    [Tooltip("Inspect time when Aware")]
    public float InspectCAAware = 5.0f;

    public GameObject ConspicousArea;
    public Pathways currentPath, newPath;
    public bool _switchToNewPath = false, _registeredSomething = false, _collisionPathInterruption = false;

    private NaddiAwareness _awareness;
    private Animator _animator;
    public int PauseCounter = 0;

    private s_SoundManager _soundManager;

    [Header("Force States for testing")]
    public bool TestChoosenState = false;
    [Range(0,4)] public int ChoosenState;
    public string CurrentState;

    //start on path0 & with regular state
    private void Start()
    {
        _awareness = GetComponent<NaddiAwareness>();
        _animator = GetComponentInChildren<Animator>();
        WorldStateData.Instance.SetNaddiStateMachine(this);
        WorldStateData.Instance.SwitchNaddisPath(0);
        currentPath = newPath;
        SwitchState(new N_00_RegularState(this));
        _soundManager = GameObject.Find("SoundManager").GetComponent<s_SoundManager>();
    }

    //depending on Player position, register which path Naddi should be on (actual switch happens in NaddiBaseState)
    public void CallForLocationSwitch(Pathways nextPath)
    {
        newPath = nextPath;
        _switchToNewPath = true;
    }

    //process information and set CA accordingly for possible inspection
    public void RegisterInput(Vector3 position) {
        ConspicousArea.transform.position = position; 
        _registeredSomething = true;
    }

    //get which state should be active, according to awareness
    public NaddiBaseState GetCurrentStateByAwareness() { return _awareness.GetStateByAwareness(); }

    //compare current state against state by awareness, change if needed
    // FOR TESTING: if TestChoosenState is true, compare to choosen state, change if needed
    public void SwitchStateByAwareness(NaddiBaseState current) {
        if (!TestChoosenState) { 
        NaddiBaseState compareState = _awareness.GetStateByAwareness();
            if (current._state != compareState._state) {
                CurrentState = compareState._state.ToString();
                SwitchState(compareState);
            }
        }
        else if (TestChoosenState && currentState.ToString() != CheckChoosenState().ToString()) {
            CurrentState = CheckChoosenState()._state.ToString();
            SwitchState(CheckChoosenState());
        }
    }

    //get collision & call optional methods in current state
    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    //FOR TESTING: provide ChoosenState for comparision
    private NaddiBaseState CheckChoosenState() {
        NaddiBaseState choosenState = null;
        switch (ChoosenState)
        {
            case 0:
                choosenState = new N_00_RegularState(this);
                break;
            case 1:
                choosenState = new N_01_SuspiciousState(this);
                break;
            case 2:
                choosenState = new N_02_AlertState(this);
                break;
            case 3:
                choosenState = new N_03_AwareState(this);
                break;
            case 4:
                choosenState = new N_04_HuntState(this);
                break;
            default:
                break;
        }
        return choosenState;
    }

    public void PlayMovingOrIdleAnimation(NaddiBaseState.NaddiStates state, float speed)
    {
        switch (state)
        {
            case NaddiBaseState.NaddiStates.Pause:
                _animator.SetBool("IsMoving", false);
                _animator.SetFloat("Velocity", 0.0f);
                break;
            case NaddiBaseState.NaddiStates.InspectPOI:
                _animator.SetBool("IsMoving", false);
                _animator.SetFloat("Velocity", 0.0f);
                //_soundManager.PlaySound3D("event:/SFX/NaddiIdle", transform.position);
                break;
            case NaddiBaseState.NaddiStates.InspectCA:
                _animator.SetBool("IsMoving", false);
                _animator.SetFloat("Velocity", 0.0f);
                //_soundManager.PlaySound3D("event:/SFX/NaddiIdle", transform.position);
                break;

            case NaddiBaseState.NaddiStates.Regular:
                _animator.SetBool("IsMoving", true);
                _animator.SetFloat("Velocity", speed);
                break;
            case NaddiBaseState.NaddiStates.Suspicious:
                _animator.SetBool("IsMoving", true);
                _animator.SetFloat("Velocity", speed);
                break;
            case NaddiBaseState.NaddiStates.Alert:
                _animator.SetBool("IsMoving", true);
                _animator.SetFloat("Velocity", speed);
                //_soundManager.PlaySound3D("event:/SFX/NaddiAlert", transform.position);
                break;
            case NaddiBaseState.NaddiStates.Aware:
                _animator.SetBool("IsMoving", true);
                //_animator.SetFloat("Velocity", speed);
                break;
            case NaddiBaseState.NaddiStates.Hunt:
                _animator.SetBool("IsMoving", true);
                _animator.SetFloat("Velocity", speed);
                //_soundManager.PlaySound3D("event:/SFX/NaddiAttack1", transform.position);
                break;
            default:
                _animator.SetBool("IsMoving", true);
                _animator.SetFloat("Velocity", speed);
                break;
        }
    }

    public void PlaySound(string soundFile) {

        _soundManager.PlaySound3D(soundFile, transform.position);
    }

    public void PlayAnimationSwitchLocation()
    {

    }
}
