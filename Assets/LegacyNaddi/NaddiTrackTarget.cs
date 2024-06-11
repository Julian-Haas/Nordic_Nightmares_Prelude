using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// works like a trail of scet / footprints left behind
// number of trackPoints, how frequent they will be left & which min distance is needed can be adjusted
public class NaddiTrackTarget : MonoBehaviour
{
    public int TrackPointCount = 25;
    public float TimeBetweenTrackpoints = 1.0f;
    public float MinDistanceBetweenTrackpoints = 5.0f;
    public List<Vector3> _track;
    private float _timePassed = 0.0f;
    private Vector3 _reachableTrackpoint;
    private bool _gotTracked = false;
    public List<Vector3> _blocksView;

    public void Start()
    {
        _track.Add(transform.position);
    }

    //if enough time has passed, inserts current position to beginning of list of tracks & removes last element on reaching maximum track number 
    public void LeaveTrackPoint()
    {
        _timePassed += Time.deltaTime;
        if (_timePassed >= TimeBetweenTrackpoints && Vector3.Distance(_track[0], transform.position) >= MinDistanceBetweenTrackpoints)
        {
            _track.Insert(0, transform.position);
            if(_track.Count > TrackPointCount) { _track.RemoveAt(TrackPointCount); }
            _timePassed = 0.0f;
        }
    }

    //returns first reachable track point for caller (position) 
    // if no trackpoint can be reached, returns current position
    public Vector3 ReturnReachableTrackpoint(Vector3 position)
    { 
        if(_track.Count > 0)
        {
            for (int i = 0; i < _track.Count; i++)
            {
                RaycastHit hitinfo;
                bool clearLineOfSight = !Physics.Linecast(position, _track[i], out hitinfo);
                if (clearLineOfSight)
                {
                    _reachableTrackpoint = _track[i];
                    _gotTracked = true;
                    //Debug.Log("Reachable Trackpoint: No. " + i + " at " + _track[i]);
                    return _track[i];
                }
                else
                {
                    _blocksView.Add(hitinfo.transform.position);
                    //Debug.Log("No trackpoint reachable, blocked by " + hitinfo.transform.gameObject.name); 
                }
            }
            return transform.position; 
        }
        else { 
             //Debug.Log("No trackpoint reachable, blocked by "); 
            _reachableTrackpoint = Vector3.zero;
            return transform.position; 
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Draw Target Trail
        Handles.color = Color.cyan;
        foreach(Vector3 track in _track)
        {
            Handles.DrawWireDisc(track, Vector3.up, 0.25f, 5.0f);
        }

        if(_gotTracked )
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(_reachableTrackpoint, Vector3.up, 0.25f, 7.0f);
        }
    }
#endif
}