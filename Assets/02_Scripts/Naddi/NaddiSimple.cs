using UnityEditor;
using UnityEngine;

public class NaddiSimple : MonoBehaviour
{
    public float PathWalkSpeed, ChaseSpeed, RotationSpeed, StareTime;
    [SerializeField] GameObject _pathFollower, _naddisHead, _leftEllbow, _rightEllbow;
    [SerializeField] MoveAlongSpline _path;
    [SerializeField] NaddiTrackTarget _tracker;
    [SerializeField] Animator _animator;
    public NaddiTrackTarget _trackerPlayer;
    public s_PlayerCollider _player;
    public bool _pathWalking = true, _chasePlayer = false, _stareAtPlayer = false, _returnToPath = false, _huntScream = false;
    private float _stareTimer = 0.0f;
    public Vector3 _direction, _closestTrack;
    private Vector3 _end;
    private float _triggerEnterDelay = 0.0f;
    

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ManualUpdate() {
        if (_pathWalking)
        {
            FollowPath();
        }
        else if (_chasePlayer)
        {
            ChasePlayer();
        }
        else if (_stareAtPlayer)
        {
            StareAtPlayer();
        }
        else if (_returnToPath)
        {
            TrackBackToPath();
        }
        _triggerEnterDelay += Time.deltaTime;
    }

    public void ResetNaddi() {
        _pathWalking = true;
        _chasePlayer = false;
        _stareAtPlayer = false;
        _returnToPath = false;
        _triggerEnterDelay = 0.0f;
    }

    public void FollowPath() {
        //Debug.Log("Follow Path - Chase Player False ");
        _pathWalking = true;
        _chasePlayer = false;
        _stareAtPlayer = false;
        _returnToPath = false;
        _path.ManualUpdate();
        MoveTowardsTarget(_pathFollower.transform.position, PathWalkSpeed);
        
    }

    void ChasePlayer() { 
        if(_player._inShadow || _player._inSafeZone)
        {
        //Debug.Log("Chase Player - Chase Player False ");
            _pathWalking = false;
            _chasePlayer = false;
            _stareAtPlayer = true;
            _returnToPath = false;
        }

        if (CheckclearHeadSight(_player.gameObject))
        {
            _tracker.LeaveTrackPoint();
            MoveTowardsTarget(_player.transform.position, ChaseSpeed);
        }
        else if (!CheckclearHeadSight(_player.gameObject)) {
            _tracker.LeaveTrackPoint();
            _closestTrack = _trackerPlayer.ReturnReachableTrackpoint(_naddisHead.transform.position);
            MoveTowardsTarget(_closestTrack, ChaseSpeed);
        }

        if (!_huntScream)
        {
            GameObject.Find("SoundManager").GetComponent<s_SoundManager>().PlaySound3D("event:/SFX/NaddiAlert", transform.position);
            _huntScream = true;
        }
    }

    public void StareAtPlayer()
    {
        _stareTimer += Time.deltaTime;
        //Debug.Log("Staring at player for " + _stareTimer+ " seconds");
        if (!_player._inShadow && !_player._inSafeZone)
        {
        //Debug.Log("StareAtPlayer - Player Invisible - Chase Player TRUE ");
            _pathWalking = false;
            _chasePlayer = true;
            _stareAtPlayer = false;
            _returnToPath = false;
            _stareTimer = 0.0f;
        }
        else if(_stareTimer < StareTime)
        {
            _animator.SetBool("IsMoving", false);
            _animator.SetFloat("Velocity", 0.0f);
        }
        else if (_stareTimer >= StareTime)
        {
            //Debug.Log("StareAtPlayer- Time Run Out - Chase Player false ");
            _chasePlayer = false;
            _pathWalking = false;
            _stareAtPlayer = false;
            _returnToPath = true;
        }
    }

    public void TrackBackToPath() {
        //Debug.Log("TrackBackToPath - Chase Player false ");
        _pathWalking = false;
        _chasePlayer = false;
        _stareAtPlayer= false;
        _returnToPath = true;
        _huntScream = false;
        _stareTimer = 0.0f;

        if (Vector3.Distance(transform.position, _pathFollower.transform.position) <= 1.0f)
        {
            _returnToPath = false;
            _pathWalking = true;
        }
        else
        {
            if (!CheckclearHeadSight(_pathFollower))
            {
                MoveTowardsTarget(_tracker.ReturnReachableTrackpoint(_naddisHead.transform.position), PathWalkSpeed);
            }
            else
            {
                MoveTowardsTarget(_pathFollower.transform.position, PathWalkSpeed);
            }
        }
    }

    private bool CheckclearHeadSight(GameObject target)
    {

        RaycastHit hitHead;
        RaycastHit hitLeft;
        RaycastHit hitRight;
        bool clearHeadSight = !Physics.Linecast(_naddisHead.transform.position, target.transform.position, out hitHead);
        bool clearLeftEllbowSight = !Physics.Linecast( _leftEllbow.transform.position, target.transform.position, out hitLeft);
        bool clearRightEllbowSight = !Physics.Linecast(_rightEllbow.transform.position, target.transform.position, out hitRight);
        bool clearSight = false;
        if ((!clearHeadSight && hitHead.transform.gameObject.tag == target.gameObject.tag) || (!clearLeftEllbowSight && hitLeft.transform.gameObject.tag == target.gameObject.tag) || (!clearRightEllbowSight && hitRight.transform.gameObject.tag == target.gameObject.tag))
        {
            clearSight = true;
        }
        else if ((!clearHeadSight && hitHead.transform.gameObject.tag != target.gameObject.tag) || (!clearLeftEllbowSight && hitLeft.transform.gameObject.tag != target.gameObject.tag) || (!clearRightEllbowSight && hitRight.transform.gameObject.tag != target.gameObject.tag))
        {
            clearSight = false;
        }
        else { 
            _end = target.transform.position;
            return clearSight;
        }

        if(target == _player.gameObject && ( _player._inShadow || _player._inSafeZone))
        {
            clearSight = false;
        }
        return clearSight;
    }

    private void MoveTowardsTarget(Vector3 target, float speed)
    {
        Vector3 targetPos = new Vector3(target.x, transform.position.y, target.z);
        _direction = (targetPos - transform.position).normalized;
        transform.position += (speed * Time.deltaTime * _direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * RotationSpeed);
        _animator.SetBool("IsMoving", true);
        _animator.SetFloat("Velocity", speed / 10.0f);
    }

    public void HeardPlayer()
    {
        _pathWalking = false;
        _chasePlayer = true;
        _stareAtPlayer = false;
        _returnToPath = false;
    }


    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player" && _triggerEnterDelay >= 1.0f  && !_player._inShadow && !_player._inSafeZone )
        { 
           // Debug.Log("OnTriggerEnter - Chase Player TRUE ");
            _pathWalking = false;
            _chasePlayer = true;
            _stareAtPlayer = false;
            _returnToPath = false;

        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_player != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(_naddisHead.transform.position, _end, 10);
            Handles.DrawLine(_leftEllbow.transform.position, _end, 10);
            Handles.DrawLine(_rightEllbow.transform.position, _end, 10);
            Handles.DrawWireDisc(_closestTrack, Vector3.up, 1.0f, 10.0f);
        }
    }
#endif
}