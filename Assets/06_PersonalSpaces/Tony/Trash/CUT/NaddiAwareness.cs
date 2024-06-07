using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class NaddiAwareness : MonoBehaviour
{
    //to do::
    // - set up state machine for behavioral changes
    // - implement different state behaviors

    public float DistancePlayer = 0.0f;


    public float ViewingAngle = 0.0f;
    public float PlayerDistance = 0.0f;
    public Material material;

    [SerializeField] VisualEffect Alerted;
    [Header("Sight related parameters")]
    [SerializeField] float ShortDistance = 1.0f;
    [Range(0.0f, 180.0f)]  public float ShortAngle = 135.0f; 
    [SerializeField] float MiddleDistance = 2.5f;
    [Range(0.0f, 180.0f)] public float MiddleAngle = 90.0f; 
    [SerializeField] float LongDistance = 5.0f;
    [Range(0.0f, 180.0f)] public  float LongAngle = 15.0f; 
    [Header("Hearing related parameters")]
    [SerializeField] float BestRange = 0.5f;
    [Range(0.0f, 0.5f)] public float BestAngle = 0.5f; 
    [SerializeField] float MiddleRange = 2.5f; 
    [SerializeField] float WideRange = 5.0f; 

    [Header("Awareness Increase in %")]
    [SerializeField] int LightIncrease = 1; 
    [SerializeField] int MediumIncrease = 5; 
    [SerializeField] int IntenseIncrease = 15;
    [SerializeField] int EnvironmentalChangeIncrease = 15;
    [SerializeField] int UtburdurAlertIncrease = 25;
    [SerializeField] int PlayerMadnessIncrease = 50;
    [SerializeField] int NatureTemporaryChangeIncrease = 10;
    [SerializeField] int NaturePermanentChangeIncrease = 25;
    [Header("Awareness Decrease in %")]
    [SerializeField] int Decrease = 5;
    [Header("Awareness %-tage needed to enter State")]
    [SerializeField] float SuspiciousState = 30.0f;
    [SerializeField] float AlertState = 65.0f;
    [SerializeField] float AwareState = 80.0f;
    [SerializeField] float HuntState = 95.0f;

    private GameObject _player;
    public List<int> _memoryInteractableStates;
    private float _awareness = 0.0f, _awarenessShift = 0.0f, _awarenessMultiplier = 0.0f, _playerDistance = 100.0f, _playerAngle;
    private Vector3 _direction = new Vector3(0.0f,0.0f,0.0f);
    private NaddiStateMachine _stateMachine;
    private Slider _awarenessSlider;

    private Color   _shortDistance = new Color(1.0f, 0.0f, 0.0f, 0.25f),
                    _middleDistance = new Color(0.75f, 0.0f, 1.0f, 0.25f),
                    _longDistance = new Color(0.0f, 0.0f, 1.0f, 0.25f),
                    _bestRange = new Color(1.0f, 0.5f, 0.0f, 1),
                    _middleRange = new Color(1.0f, 1.0f, 0.0f, 1),
                    _wideRange =  new Color(1.0f, 1.0f, 0.75f, 1);

    public float CurrentAwareness = 0.0f, CurrentShift = 0.0f;


    void Start()
    {        
        _player = GameObject.Find("PlayerAnimated");
        _stateMachine = GetComponent<NaddiStateMachine>();
        WorldStateData.Instance.SetNaddiAwareness(this);
        GetInitialInteractableStates();
        Alerted.pause = true;
        _awarenessSlider = GameObject.Find("AwarenessSlider")?.GetComponent<Slider>();
    }

    void Update()
    {
        CheckInteractables();
        CheckPlayerPosition();
        AwarenessUpdate();
    }

    //Get all initial interactable states & safe them in memory 
    void GetInitialInteractableStates()
    {
        List<int> init = WorldStateData.Instance.ReturnInitialInteractableStates();
        for (int i = 0; i < init.Count; i++)
        {
            _memoryInteractableStates.Add(init[i]);
        }
    }

    //If an interactable is in reach, check if its current state is different to memory & if so, increase awareness and update memory
    void CheckInteractables()
    {
        Collider[] coll = Physics.OverlapSphere(transform.position, MiddleDistance, -1);
        foreach (var hit in coll)
        {
            if (hit.CompareTag("Interactable") && Time.timeScale > 0)
            {
                int ind = hit.GetComponentInParent<Interactable>().ReturnIndex();
                if(WorldStateData.Instance.InteractableGetState(ind) != _memoryInteractableStates[ind])
                {
                    _awarenessShift += EnvironmentalChangeIncrease;
                    _memoryInteractableStates[ind] = WorldStateData.Instance.InteractableGetState(ind);
                }
            }
        }
    }

    //Check if Player is possible in range of view (compare distance Player-Naddi against LongDistance view)
    private void CheckPlayerPosition()
    {
        _playerDistance = Vector3.Distance(transform.position, _player.transform.position);
        DistancePlayer = _playerDistance;
        if(_playerDistance <= LongDistance)
        {
            CheckPlayerVisibility();
        }
    }

    //Check if Player is in any field of sight regarding distance and angle towards Naddis forwards direction (view direction)
    private void CheckPlayerVisibility() 
    {
        Vector3 _naddiPosition = new Vector3(transform.position.x,0,transform.position.z);
        Vector3 _playerPosition = new Vector3(_player.transform.position.x,0,_player.transform.position.z);
        Vector3 _forwardDirection = new Vector3(transform.forward.x, 0, transform.forward.z);
        _playerAngle = Vector3.Angle(_forwardDirection, (_playerPosition -_naddiPosition));
        PlayerDistance = _playerDistance;
        ViewingAngle = _playerAngle;
        if (_playerDistance <= ShortDistance && _playerAngle <= ShortAngle && CheckLineOfSight()) {
            _awarenessMultiplier += IntenseIncrease;
        }
        else if(_playerDistance <= MiddleDistance && _playerAngle <= MiddleAngle && CheckLineOfSight()) {
            _awarenessMultiplier += MediumIncrease; 
        }
        else if( _playerAngle <= LongAngle && CheckLineOfSight()) {
            _awarenessMultiplier += LightIncrease;
        }
    }

    //Check if Player is not hidden or Naddis view is blocked by any 
    private bool CheckLineOfSight() 
    {
        if((_player.GetComponent<s_PlayerCollider>()._inShadow && !_player.GetComponent<s_PlayerCollider>()._isMad) || _player.GetComponent<s_PlayerCollider>()._inSafeZone) { return false; }
        else
        {
            RaycastHit hitinfo;
            bool clearLineOfSight = !Physics.Linecast(transform.position, _player.transform.position,out hitinfo, 2);
            if (!clearLineOfSight && hitinfo.transform.gameObject.tag == "Player")
            {
                clearLineOfSight = true;
            }
            else if (!clearLineOfSight && hitinfo.transform.gameObject.tag != "Player")
            {
                Debug.Log("Naddis view to player blocked by " + hitinfo.transform.gameObject.name);
            }
            return clearLineOfSight;
        }
    }

    //Check if incoming noise can be heard by Naddi in any range of hearing & if so increase awareness accordingly
    public void CheckIfNaddiCanHearNoise(NoiseEmitter emitter) 
    {
        float emitDist = Vector3.Distance(emitter.transform.position, transform.position);
        bool heardSmth = true;
        if(emitter.transform.parent.name == "PlayerAnimated")
        {
            if(emitter._radius + BestRange >= emitDist)
            {
                _awarenessMultiplier += IntenseIncrease;
            }
            else if(emitter._radius + MiddleRange >= emitDist)       
            {
                _awarenessMultiplier += MediumIncrease;

            }
            else if (emitter._radius + WideRange >= emitDist) { 
                _awarenessMultiplier += LightIncrease;
            }
            else { heardSmth = false; }
        }
        else
        {
            if (emitter._radius + BestRange >= emitDist)
            {
                _awarenessShift += IntenseIncrease;
            }
            else if (emitter._radius + MiddleRange >= emitDist)
            {
                _awarenessShift += MediumIncrease;
            }
            else if (emitter._radius + WideRange >= emitDist)
            {
                _awarenessShift += LightIncrease;
            }
            else { heardSmth = false; }
        }

        if (heardSmth) 
        { 
            _stateMachine.RegisterInput(emitter.transform.position); 
        }
    }

    //Update awareness after getting gathering & combining all possible sources of influence , reset rate of awareness change to 0 afterwards 
    void AwarenessUpdate()
    {
        if (_awarenessMultiplier != 0.0f && _awareness < 100.0f)
        {
            _awarenessShift += _awarenessMultiplier * Time.deltaTime;
        }
        else if (_awarenessMultiplier == 0 && _awareness > 0.0f)
        {
            _awarenessShift += -Decrease * Time.deltaTime;
        }
        _awareness += _awarenessShift;
        _awareness = Mathf.Clamp(_awareness, 0.0f, 100.0f); ;
        CurrentAwareness = _awareness;
        CurrentShift = _awarenessShift;
        material.SetFloat("_FadeInOverlay", _awareness/100.0f);
        _awarenessShift = 0.0f;
        _awarenessMultiplier = 0.0f;
        _awarenessSlider.value = _awareness;
    }
    
    //Check awareness against when states should change & return which state according to awareness would apply
    public NaddiBaseState GetStateByAwareness()
    {
        NaddiBaseState state = new N_00_RegularState(_stateMachine);
        if (_awareness >= HuntState )
        {
            state = new N_04_HuntState(_stateMachine);
        }
        else if (_awareness >= AwareState)
        {
            state = new N_03_AwareState(_stateMachine);
        }
        else if (_awareness >= AlertState)
        {
            state = new N_02_AlertState(_stateMachine);
        }
        else if (_awareness >= SuspiciousState)
        {
            state = new N_01_SuspiciousState(_stateMachine);
        }
        return state;
    }

    //Register input by Utburdur, increase awareness accordingly & supply position of disturbance
    public void UtburdurAlert(Vector3 position)
    {
        _awarenessShift += UtburdurAlertIncrease;
        _stateMachine.RegisterInput(position);
    }

    //Increase awareness & supply players position
    public void PlayerIsMad(Vector3 position)
    {
        _awarenessShift += PlayerMadnessIncrease;
        _stateMachine.RegisterInput(position);
    }

    //Process input from interferables, adjust awareness increase accordingly & supply position of disturbance
    public void NatureSense(bool TemporaryChange, Vector3 position)
    {
        if (TemporaryChange)
        {
            _awarenessShift += NatureTemporaryChangeIncrease;
        }
        else
        {
            _awarenessShift += NaturePermanentChangeIncrease;
        }
        _stateMachine.RegisterInput(position);
    }

    //Return Naddis position
    public Vector3 GetPosition()
    {
        return transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Draw Hearing Areas
        Handles.color = _wideRange;
        //Handles.DrawSolidDisc(transform.position, Vector3.up, WideRange);
        Handles.DrawWireDisc(transform.position, Vector3.up, WideRange, 3.0f);
        Handles.color = _middleRange;
        //Handles.DrawSolidDisc(transform.position, Vector3.up, MiddleRange);
        Handles.DrawWireDisc(transform.position, Vector3.up, MiddleRange, 3.0f);
        Handles.color = _bestRange;
        //Right-Ear
        Handles.DrawWireArc(transform.position,Vector3.up, Quaternion.Euler(0, BestAngle * -180, 0) * transform.right, BestAngle * 360f, BestRange, 5.0f);
        Vector3 _r1 = transform.position + Quaternion.Euler(0, BestAngle * -180, 0) * transform.right * BestRange;
        Vector3 _r2 = transform.position + Quaternion.Euler(0, BestAngle* 180, 0) * transform.right * BestRange; 
        Handles.DrawLine(transform.position, _r1, 5.0f);
        Handles.DrawLine(transform.position, _r2, 5.0f);        
        //Left-Ear
        Handles.DrawWireArc(transform.position,Vector3.up, - (Quaternion.Euler(0, BestAngle * -180, 0) * transform.right), BestAngle * 360f, BestRange, 5.0f);
        Vector3 _l1 = transform.position - Quaternion.Euler(0, BestAngle * -180, 0) * transform.right * BestRange;
        Vector3 _l2 = transform.position - Quaternion.Euler(0, BestAngle* 180, 0) * transform.right * BestRange; 
        Handles.DrawLine(transform.position, _l1, 5.0f);
        Handles.DrawLine(transform.position, _l2, 5.0f);
        //DrawHandles(_bestRange, BestAngle, BestRange);

        Handles.color = Color.black;
        Handles.DrawLine(transform.position, (transform.position + _direction * 5),10);
        Handles.color = Color.white;
        Handles.DrawLine(transform.position, (transform.position + transform.forward * 5), 10);

        //Draw Sight Areas
        DrawHandles(_longDistance, LongAngle, LongDistance);
        DrawHandles(_middleDistance, MiddleAngle, MiddleDistance);
        DrawHandles(_shortDistance, ShortAngle, ShortDistance);

        //Draw Sight To Player
        if(_player != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(transform.position, (_player.transform.position), 10);
        }
    }

    private void DrawHandles( Color color, float angle, float radius)
    {
        Handles.color = color;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle , radius);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle , radius);
    }
#endif
}