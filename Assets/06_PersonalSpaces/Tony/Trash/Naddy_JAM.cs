using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class Naddy_JAM : MonoBehaviour //, IEnemyAction
{
/*
    public Vector3 Position; 
    public float RegularSpeed = 5.0f, HuntSpeed = 10.0f, RotationSpeed = 10.0f, DetectionZoneRadius = 5.0f, DetectionZoneDistance = 1.0f, IsSuspicious = 80.0f; 
    public int AwarenessGrowth = 15, AwarenessReduction = 5;

    public GameObject Player, DetectionCone;
    private SplineAnimate _splineScript;
    private Slider Awareness;
    private float _awareness = 0.0f;
    public bool Alert = false, UtbudurAlert = false, Madness = false, SplinePlay = true;
    private Vector3 _direction, _detectionZoneCenter, _leftPath;


    void Start()
    {        
        GlobalData.Instance.Enemy = this;
        Awareness = GameObject.Find("AwarenessSlider").GetComponent<Slider>();
        _splineScript = GetComponent<SplineAnimate>();
        DetectionZoneUpdate();
    }

    void Update()
    {
        SplinePlay = _splineScript.IsPlaying;
        DetectionZoneUpdate();
        AwarenessUpdate();
        if (_awareness >= IsSuspicious || Madness)
        {
            _splineScript.Pause();
            _leftPath = transform.position;
            FollowPlayer();
        }
        else if(_awareness < IsSuspicious && !_splineScript.IsPlaying)
        {
            ReturnToPath();
        }
        Position = transform.position;
    }

    void ReturnToPath()
    {
        _direction = (_leftPath - transform.position).normalized;
        transform.position += (RegularSpeed * Time.deltaTime * _direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * RotationSpeed);
        Position = transform.position;
        if(Vector3.Distance(transform.position, _leftPath)<= 0.5f)
        {
            _splineScript.Play();
        }
    }

    void DetectionZoneUpdate() {
        Vector3 tmp = transform.position + transform.forward.normalized * DetectionZoneDistance;
        _detectionZoneCenter = new Vector3(tmp.x,0,tmp.z);
        DetectionConeUpdate();
        if (!UtbudurAlert)
        {
            DetectPlayer();
        }
    }

    void DetectionConeUpdate()
    {
        DetectionCone.transform.localScale = new Vector3(DetectionZoneRadius, 0.5f, DetectionZoneRadius) * 2;
        DetectionCone.transform.position = _detectionZoneCenter;
    }

    void DetectPlayer()
    {
        Vector3 playerTmp = new Vector3(Player.transform.position.x, 0, Player.transform.position.z);
        if (Vector3.Distance(_detectionZoneCenter, playerTmp) < DetectionZoneRadius && !Player.GetComponent<Player_Test>().IsHidden())
        {
            Alert = true;
        }
        else {
            Alert = false;
        }
    }

    void AwarenessUpdate()
    {
        if (Alert && _awareness < 100.0f)
        {
            _awareness += AwarenessGrowth * Time.deltaTime;
        }
        else if(!Alert && _awareness > 0.0f)
        {
            _awareness -= AwarenessReduction * Time.deltaTime;
        }
        Awareness.value = _awareness;        
    }


    void FollowPlayer()
    {
        if (Alert)
        {
            _direction = (Player.transform.position - transform.position).normalized;
            transform.position += (HuntSpeed * Time.deltaTime * _direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * RotationSpeed);
        }
        else { }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_detectionZoneCenter, 0.25f);
        //Handles.color = new Color(0,0,1,0.25f);
        //Handles.DrawSolidDisc(_detectionZoneCenter, new Vector3(0, 0.1f, 0), DetectionZoneRadius);
    }

    void IEnemyAction.AlertPlayerPosition()
    {
        UtbudurAlert = true;
        Alert = true;
    }

    public void LostPlayerPosiiton()
    {    
        UtbudurAlert = false;
        DetectPlayer();
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
 */
}