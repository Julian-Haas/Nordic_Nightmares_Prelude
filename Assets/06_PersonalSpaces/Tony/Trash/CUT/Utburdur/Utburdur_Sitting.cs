using UnityEditor;
using UnityEngine;

public class Utburdur_Sitting : MonoBehaviour
{
    public UtburdurLandingSpot _currentSpot, _newSpot;
    public float HearingRange = 5.0f, FlyingSpeed = 3.0f, Arrived = 0.5f, Distance = 0.0f;
    public bool  HeardSmth = false,  AlertNaddi = false, TravelToNewSpot = false;
    private Vector3 _direction, _cautiousArea;
    public GameObject _target, _naddi;
    private WorldStateData _worldState;
    private Animator _animator;
    private s_SoundManager _soundManager;
    private float _idleTimer = 121.0f;
    
    private void Awake()
    {
        _worldState = WorldStateData.Instance;
        _worldState.AddSittingUtburdur(this);
    }

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _currentSpot = _worldState.SwitchLandingSpot();
        transform.position = _currentSpot.transform.position;
        _naddi = GameObject.Find("Naddi").transform.GetChild(0).gameObject;
        _animator.SetBool("IsFlying", false);
        _soundManager = GameObject.Find("SoundManager").GetComponent<s_SoundManager>();
    }

    void Update()
    {
        if (AlertNaddi && !TravelToNewSpot)
        {
            ToNaddi();
        }
        else if (!AlertNaddi && TravelToNewSpot)
        {
            ToNewSpot();
        }
        else if(!HeardSmth && _idleTimer >= 120.0f)
        {
            _soundManager.PlaySound3D("event:/SFX/UtburdurIdle", transform.position);
            _idleTimer = 0.0f;
        }
        _idleTimer += Time.deltaTime;
    }

    //Checks if created noise is close enough for Utburdur to hear,
    //if so, get noise emitters positionn, start reporting process to Naddi & getting to a new landing spot
    public void CheckIfUtburdurCanHearNoise(NoiseEmitter emitter)
    {
        float emitDist = Vector3.Distance(emitter.transform.position, transform.position);
        if (emitter._radius + HearingRange >= emitDist && !HeardSmth)
        {
            Debug.Log("Utburdur heard smth");
            _cautiousArea = emitter.transform.position;
            _soundManager.PlaySound3D("event:/SFX/UtburdurScreech", transform.position);
            HeardSmth = true;
            SwitchSpot();
        }
    }

    //reserve new landing spot (prevents other startled Utburdur from landing there)
    //free own spot and add it to according list in worldState
    //set Naddi as target to report position of noiseEmitter
    private void SwitchSpot() {
        _newSpot = _worldState.SwitchLandingSpot();
        _worldState.AddLandingSpot(_currentSpot);
        _currentSpot = _newSpot;
        _target = _naddi;
        AlertNaddi = true;
    }

    //move towards Naddi, when close supply noiseEmitters position
    //start travel to new landingspot
    private void ToNaddi() {
        FlyToTarget();
        if(Distance <= Arrived)
        {
            AlertNaddi = false;
            _target = _currentSpot.transform.gameObject;
            _naddi.GetComponent<NaddiAwareness>()?.UtburdurAlert(_cautiousArea);
            TravelToNewSpot = true;
        }
    }

    //move to new landingSpot, when close, stop moving & land
    private void ToNewSpot() {
        FlyToTarget();
        if (Distance <= 1.0f) {
            _animator.SetBool("IsFlying", false);
        }
        if (Distance <= Arrived)
        {
            TravelToNewSpot = false;
            transform.position = _currentSpot.transform.position;
            HeardSmth = false;
            _idleTimer = 121.0f;
        }
    }

    //move Utburdur towards current target according to speed & passed time
    private void FlyToTarget()
    {
        _direction = (_target.transform.position - transform.position).normalized;
        transform.position += _direction * FlyingSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * FlyingSpeed);
        Distance = Vector3.Distance(_target.transform.position, transform.position);
        _animator.SetBool("IsFlying",true);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Draw Hearing Area
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, HearingRange, 3.0f);
    }
#endif
}