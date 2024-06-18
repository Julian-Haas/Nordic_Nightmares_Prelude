using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class NaddiPatrolBehaviour : MonoBehaviour
{
    [SerializeField]
    private Naddi _naddi;
    [SerializeField]
    private PatrolPath _patrolPath;
    [HideInInspector]
    public SplineAnimate SplineAnimate;
    [SerializeField]
    private NaddiHearing _naddiHearing;
    private void Start()
    {
        InitSplineAnimate();
    }
    public void WalkOnPatrol()
    {
        _naddi.CanChasePlayer = true;
        if (_naddi.StartedPatrol == false)
        {
            NaddiUtillitys.SetFlags(ref _naddi.StartedPatrol, ref _naddi.CanChasePlayer, true, true);
            SplineAnimate.Container = _patrolPath.GetActivePatrolPath();
            _naddi.KilledPlayer = false;
            if (SplineAnimate.ElapsedTime > 0)
            {
                SplineAnimate.ElapsedTime = 0f;
            }
            Vector3 newPos = _patrolPath.CalculateDistanceForEachKnot();
            if (_naddi.PlayerWasInSafeZone && !_naddi.PlayerInSafeZone)
            {
                NaddiUtillitys.SetFlags(ref _naddi.StartedPatrol, ref _naddi.CanChasePlayer, false, false);
                _naddi.StateMachiene.FoundPlayer();
                return;
            }
            transform.position = newPos;
            _naddi.StateMachiene.GetNaddiMeshRenderer.enabled = true;


            SplineAnimate.enabled = true;
        }
        SplineAnimate.Play(); //needs to be called every frame cause unity is stupid and other wise Naddi wouldnt walk along spline
    }

    void InitSplineAnimate()
    {
        SplineAnimate = gameObject.AddComponent<SplineAnimate>();
        SplineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
        SplineAnimate.enabled = false;
        SplineAnimate.PlayOnAwake = false;
        SplineAnimate.MaxSpeed = _naddi.Speed;
    }

    public void DeactivatePatrol()
    {
        Vector3 currentPos = transform.position;
        SplineAnimate.Pause();
        SplineAnimate.enabled = false;
        SplineAnimate.ElapsedTime = 0;
        _naddi.StartedPatrol = false;
        transform.position = currentPos;
    }
}
