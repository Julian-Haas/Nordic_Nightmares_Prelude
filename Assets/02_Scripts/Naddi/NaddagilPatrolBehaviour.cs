using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class NaddagilPatrolBehaviour : MonoBehaviour
{
    [SerializeField, Header("Private Dependicys")]
    private Naddagil _naddagil;

    public MoveOnSpline SplineAnimate { get; private set; }

    [Header("Public Flags")]
    public bool StartedPatrol = false;
    private bool isPaused = false; 

    private SplineContainer _spline;
    private bool allReadyPaused = false;
    private int _pauseIndex = 0; 

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
            _spline = _naddagil.PatrolPath.GetActivePatrolPath();
            SplineAnimate.SetSpline(ref _spline); 
            _naddagil.AttackBehaviour.KilledPlayer = false;
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
        if (SplineAnimate.CheckIfShouldPause(_pauseIndex) && !allReadyPaused)
        {
            isPaused = true; 
            allReadyPaused = true;
            StartCoroutine(PauseYield());
        }
        if(!isPaused)
        SplineAnimate.Play(); 
      
    }

    void InitSplineAnimate()
    {
        SplineAnimate = gameObject.AddComponent<MoveOnSpline>();
    }

    public void DeactivatePatrol()
    {
        Vector3 currentPos = transform.position;
        SplineAnimate.Stop();
        SplineAnimate.enabled = false;
        StartedPatrol = false;
        transform.position = currentPos;
    }

    private IEnumerator PauseYield()
    {
        SplineAnimate.Pause();
        isPaused = true; 
        _naddagil.StateMachiene.SetState(NaddiStates.Idle); 
        yield return new WaitForSeconds(2);
        _naddagil.StateMachiene.SetState(NaddiStates.Patrol);
        SplineAnimate.Resume();
        isPaused=false; 
    }

    public void SetAllreadyPaused(bool val)
    {
        allReadyPaused = val; 
    }

    public void SetPauseIndex(int val)
    {
        _pauseIndex = val; 
    }
}