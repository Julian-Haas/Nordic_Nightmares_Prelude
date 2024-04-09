using System.Collections.Generic;
using UnityEditor;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Naddi : MonoBehaviour //, IEnemyAction
{
    /*
    //to do::
    // - list of interactable states in level ==>> GLOBAL DATA
    // - list of last known interactable states in level to Naddi
    // - comparision of interactable states memory to current
    // - implement hearing ranges
    // - correct hearing best area according to angle offset
    // - list ob pathes to follow ==>> GLOBAL DATA
    // - switch pathes according to players position
    // - set up state machine for behavioral changes
    // - implement different state behaviors

    // Interactables need : private int _state, _index 
    // Interactables need : public void UpdateState(), public int GetIndex()

    public VisualEffect Alerted;
    [Header("Movement Speed")]
    public float RegularSpeed = 5.0f; 
    public float HuntSpeed = 10.0f; 
    public float RotationSpeed = 10.0f;
    [Header("Sight related parameters")]
    public float ShortDistance = 1.0f;
    [Range(0.0f, 180.0f)] public float ShortAngle = 135.0f; 
    public float MiddleDistance = 2.5f;
    [Range(0.0f, 180.0f)] public float MiddleAngle = 90.0f; 
    public float LongDistance = 5.0f;
    [Range(0.0f, 180.0f)] public float LongAngle = 15.0f; 
    [Header("Hearing related parameters")]
    public float BestRange = 0.5f;
    [Range(0.0f, 0.5f)] public float BestAngle = 0.5f; 
    public float MiddleRange = 2.5f; 
    public float WideRange = 5.0f; 

    [Header("Awareness Increase / Decrease in %")]
    public int AwarenessGrowthLight = 1; 
    public int AwarenessGrowthMedium = 5; 
    public int AwarenessGrowthIntense = 15;
    public int AwarenessGrowthOnEnvironmentalChange = 15;
    public int AwarenessGrowthOnUtburdurAlert = 25;
    public int AwarenessGrowthNatureTemporaryChange = 10;
    public int AwarenessGrowthNaturePermanentChange = 25;
    public int AwarenessReduction = 5;
    [Header("Awareness %-tage needed for change of State")]
    public float IsSuspicious = 30.0f;
    public float IsAlert = 65.0f;
    public float IsAware = 80.0f;
    public float IsHunting = 95.0f;
    [Header("State Display")]
    public bool Alert = false; 
    public bool UtbudurAlert = false; 
    public bool Madness = false; 
    public bool SplinePlay = true;

    private GlobalData _globalData;
    public List<int> _memoryInteractableStates;
    private GameObject _player, _target;
    private Pathways _currentPath;
    public float _awareness = 0.0f, _awarenessShift = 0.0f, _awarenessMultiplier = 0.0f, _speed = 5.0f, _playerDistance = 100.0f, _playerAngle;
    private Vector3 _direction = new Vector3(0.0f,0.0f,0.0f);

    private Color   _shortDistance = new Color(1.0f, 0.0f, 0.0f, 0.25f),
                    _middleDistance = new Color(0.75f, 0.0f, 1.0f, 0.25f),
                    _longDistance = new Color(0.0f, 0.0f, 1.0f, 0.25f),
                    _bestRange = new Color(1.0f, 0.5f, 0.0f, 1),
                    _middleRange = new Color(1.0f, 1.0f, 0.0f, 1),
                    _wideRange =  new Color(1.0f, 1.0f, 0.75f, 1); 

    void Start()
    {        
        _globalData = GlobalData.Instance;
        _globalData.Enemy = this;
        //_memoryInteractableStates = _globalData.ReturnInitialInteractableStates();
        GetInitialInteractableStates();
        _player = GameObject.Find("Player");
        _speed = RegularSpeed;
        _globalData.SwitchPath(0);
        Alerted.Play();
        Alerted.pause = true;
    }

    void Update()
    {
        CheckInteractables();
        CheckPlayerPosition();
        AwarenessUpdate();
        AwarenessCheck();
        FollowTarget();
    }
    
    void GetInitialInteractableStates()
    {
        List<int> init = _globalData.ReturnInitialInteractableStates();
        for (int i = 0; i < init.Count; i++)
        {
            _memoryInteractableStates.Add(init[i]);
        }
    }

    void CheckInteractables()
    {
        Collider[] coll = Physics.OverlapSphere(transform.position, MiddleDistance, -1);
        foreach (var hit in coll)
        {
            if (hit.CompareTag("Interactable") && Time.timeScale > 0)
            {
                int ind = hit.GetComponentInParent<Interactable>().ReturnIndex();
                if(_globalData.InteractableGetState(ind) != _memoryInteractableStates[ind])
                {
                    _awarenessShift += AwarenessGrowthOnEnvironmentalChange;
                    _memoryInteractableStates[ind] = _globalData.InteractableGetState( ind);
                }
            }
        }
    }

    private void CheckPlayerPosition()
    {
        _playerDistance = Vector3.Distance(transform.position, _player.transform.position);
        if(_playerDistance <= LongDistance)
        {
            CheckPlayerVisibility();
        }
    }

    private void CheckPlayerVisibility() {
        Vector3 _naddiPosition = new Vector3(transform.position.x,0,transform.position.z);
        Vector3 _playerPosition = new Vector3(_player.transform.position.x,0,_player.transform.position.z);
        Vector3 _forwardDirection = new Vector3(_direction.x, 0, _direction.z);
        _playerAngle = Vector3.Angle(_forwardDirection, (_playerPosition -_naddiPosition));
        if (_playerDistance <= ShortDistance && _playerAngle <= ShortAngle && CheckLineOfSight("short distance")) {
            _awarenessMultiplier += AwarenessGrowthIntense;
        }
        else if(_playerDistance <= MiddleDistance && _playerAngle <= MiddleAngle && CheckLineOfSight("middle distance")) {
            _awarenessMultiplier += AwarenessGrowthMedium; 
        }
        else if( _playerAngle <= LongAngle && CheckLineOfSight("long distance")) {
            _awarenessMultiplier += AwarenessGrowthLight;
            //Debug.Log("Seing Player in Long Distance!!");
        }

    }

    private bool CheckLineOfSight(string distance) {
        RaycastHit hitinfo;
        bool clearLineOfSight = !Physics.Linecast(transform.position, _player.transform.position,out hitinfo, -1);
        if (hitinfo.collider.gameObject.name == "Player")
        {
            //Debug.Log("Player seen in " + distance + " !!!");
            return true;
        }
        else
        {
            Debug.Log(hitinfo.collider.gameObject.name);
            return false;
        }
    }

    public void CheckHearing(NoiseEmitter emitter) {
        //Debug.Log("Naddi tries to hear " + emitter.transform.gameObject.GetComponentInParent<Transform>().name + emitter.Quiet);
        //Debug.Log(emitter.Quiet);

        float emitDist =Vector3.Distance(emitter.transform.position, transform.position);
        if(emitter.transform.parent.name == "Player")
        {
            if(emitter._radius + BestRange >= emitDist)
            {
                Debug.Log("Naddi heard " + emitter.transform.parent.name + " close by!");
                _awarenessMultiplier += AwarenessGrowthIntense;
            }
            else if(emitter._radius + MiddleRange >= emitDist)       
            {
                Debug.Log("Naddi heard " + emitter.transform.parent.name + " in middle distance!");
                _awarenessMultiplier += AwarenessGrowthMedium;

            }
            else if (emitter._radius + WideRange >= emitDist) { 
                Debug.Log("Naddi heard " + emitter.transform.parent.name + " far away");
                _awarenessMultiplier += AwarenessGrowthLight;
            }
        }
        else
        {
            if (emitter._radius + BestRange >= emitDist)
            {
                Debug.Log("Naddi heard " + emitter.transform.parent.name + " close by!");
                _awarenessShift += AwarenessGrowthIntense;
            }
            else if (emitter._radius + MiddleRange >= emitDist)
            {
                Debug.Log("Naddi heard " + emitter.transform.parent.name + " in middle distance!");
                _awarenessShift += AwarenessGrowthMedium;
            }
            else if (emitter._radius + WideRange >= emitDist)
            {
                Debug.Log("Naddi heard " + emitter.transform.parent.name + " far away");
                _awarenessShift += AwarenessGrowthLight;
            }
        }
    }

    void AwarenessUpdate()
    {
        _awareness = _globalData.GetAwareness();
        if (_awarenessMultiplier != 0.0f && _awareness < 100.0f)
        {
            _awarenessShift += _awarenessMultiplier * Time.deltaTime;
        }
        else if (_awarenessMultiplier == 0 && _awareness > 0.0f)
        {
            _awarenessShift += -AwarenessReduction * Time.deltaTime;
        }
        _globalData.UpdateAwareness(_awarenessShift);
        _awarenessShift = 0.0f;
        _awarenessMultiplier = 0.0f;
    }

    void AwarenessCheck()
    {
        if ((_awareness >= IsHunting && !_globalData.Player.CheckIsHidden(transform.parent.gameObject) || Madness))
        {
            Alerted.pause = false;
            _currentPath.StopPathWalking();
            _target = _player;
            _speed = HuntSpeed;
        }
        else if (_awareness >= IsHunting && (_globalData.Player.CheckIsHidden(transform.parent.gameObject) || (!Alert && !UtbudurAlert)))
        {
            _speed = 1.0f;
        }
        else if (_awareness < IsHunting && !_currentPath.IsPaused())
        {
            Alerted.pause = true;
            _target = _currentPath.GetPathFollower();
            _speed = RegularSpeed;
            if (Vector3.Distance(_target.transform.position, transform.position) <= 1.0f)
            {
                _currentPath.ContinuePathWalking();
                _speed = RegularSpeed;
            }
        }
    }

    void FollowTarget()
    {
        _direction = (_target.transform.position - transform.position).normalized;
        transform.position += (_speed * Time.deltaTime * _direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * RotationSpeed);
    }

    public void UtburdurAlert()
    {
        _awarenessShift += AwarenessGrowthOnUtburdurAlert;
    }

    public void NatureSense(bool TemporaryChange)
    {
        if (TemporaryChange)
        {
            _awarenessShift += AwarenessGrowthNatureTemporaryChange;
        }
        else
        {
            _awarenessShift += AwarenessGrowthNaturePermanentChange;
        }
    }

    public void PlayerInView()
    {
        Alert = true;
    }

    public void PlayerOutOfView()
    {
        Alert = false;
    }

    void IEnemyAction.AlertPlayerPosition()
    {
        UtbudurAlert = true;
        Alert = true;
    }

    public void LostPlayerPosiiton()
    {    
        UtbudurAlert = false;
        Alert = false;
    }

    public void IsMad()
    {
        Madness = true;
        _awareness = 100.0f;
    }

    public void NotMad()
    {
        Madness = false;
    }

    public void SwitchLocation(Pathways newPath, int index = -1)
    {
        _currentPath = newPath;
        _target = _currentPath.StartPathWalking(_speed);
        newPath.SPEED = _speed;
        transform.position = _target.transform.position;
        transform.rotation = _target.transform.rotation;
    }

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
    }

    private void DrawHandles( Color color, float angle, float radius)
    {
        Handles.color = color;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle , radius);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle , radius);
    }
#endif
     */
}