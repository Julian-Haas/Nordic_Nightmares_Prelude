using UnityEngine;
using UnityEngine.Splines;

public class Pathways : MonoBehaviour
{
    public int _islandIndex;
    private MoveAlongSpline _moveAlongSpline;
    private GameObject _pathFollower;
    private NaddiTrackTarget _track;

    //set island index according to worldStateData, set speed to 0.0f & prepare tracking
    private void Awake()
    {
        _islandIndex = WorldStateData.Instance.AddPath(this);
        _pathFollower = transform.parent.Find("PathFollower").gameObject;
        _moveAlongSpline = _pathFollower.GetComponent<MoveAlongSpline>();
        _moveAlongSpline.moveSpeed = 0.0f;
        _track = _pathFollower.GetComponent<NaddiTrackTarget>();
    }

    //set speed & return pathFollower
    public GameObject GetPathFollower(float speed) { 
        _moveAlongSpline.moveSpeed = speed;
        return _pathFollower; 
    }

    //set speed & start moving pathfollower on spline
    public GameObject StartPathWalking(float speed) {
        _moveAlongSpline.moveSpeed = speed;
        return _pathFollower;
    }

    //pause moving along spline by setting speed to 0.0f
    public void StopPathWalking() {
        _moveAlongSpline.moveSpeed = 0.0f;
    }

    //continue moving along spline & leaving tracks
    public void ContinueOnPath()
    {
        _moveAlongSpline.ManualUpdate();
        _track.LeaveTrackPoint();
    }

    //if Player enters pathArea, call Naddi to switch path accordingly
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            WorldStateData.Instance.SwitchNaddisPath(_islandIndex);
        }
    }

    //if Player exits pathArea, set speed to zero to stop movement
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _moveAlongSpline.moveSpeed = 0;
        }
    }
}