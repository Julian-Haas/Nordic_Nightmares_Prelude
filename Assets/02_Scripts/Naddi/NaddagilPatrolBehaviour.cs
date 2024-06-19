using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class NaddagilPatrolBehaviour : MonoBehaviour
{
    [SerializeField, Header("Private Dependicys")]
    private Naddagil _naddagil;

    [HideInInspector]
    public SplineAnimate SplineAnimate { get; private set; }

    [Header("Public Flags")]
    public bool StartedPatrol = false;

    private void Start()
    {
        InitSplineAnimate();
    }
    public void WalkOnPatrol()
    {
        _naddagil.AttackBehaviour.CanChasePlayer = true;
        if (StartedPatrol == false)
        {
            NaddagilUtillitys.SetFlags(ref StartedPatrol, ref _naddagil.AttackBehaviour.CanChasePlayer, true, true);
            SplineAnimate.Container = _naddagil.PatrolPath.GetActivePatrolPath();
            _naddagil.AttackBehaviour.KilledPlayer = false;
            if (SplineAnimate.ElapsedTime > 0)
            {
                SplineAnimate.ElapsedTime = 0f;
            }
            Vector3 newPos = _naddagil.PatrolPath.CalculateDistanceForEachKnot();

            //When the Naddi wants to restart Patrol because the Player hided in a SafeZone but comes out to early, switch to Chase State and return.
            if (_naddagil.AttackBehaviour.PlayerWasInSafeZone && !_naddagil.AttackBehaviour.PlayerInSafeZone)
            {
                NaddagilUtillitys.SetFlags(ref StartedPatrol, ref _naddagil.AttackBehaviour.CanChasePlayer, false, false);
                _naddagil.StateMachiene.FoundPlayer();
                return;
            }
            transform.position = newPos;
            _naddagil.MeshRenderer.enabled = true;
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
        SplineAnimate.MaxSpeed = _naddagil.Speed;
    }

    public void DeactivatePatrol()
    {
        Vector3 currentPos = transform.position;
        SplineAnimate.Pause();
        SplineAnimate.enabled = false;
        SplineAnimate.ElapsedTime = 0;
        StartedPatrol = false;
        transform.position = currentPos;
    }
}
