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
    private bool isPaused = false; 

    private SplineContainer _spline; 
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
            SplineAnimate.Container = _spline;
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
        if (CheckIfShouldPause())
        {
            isPaused = true;
            StartCoroutine(PauseYield());
        }
        if (!isPaused)
        {
            SplineAnimate.Play(); 
        }
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

    private IEnumerator PauseYield()
    {
        yield return new WaitForSeconds(2);
        SplineAnimate.MaxSpeed = _naddagil.Speed; 

    }

    private bool CheckIfShouldPause()
    {
        if (_spline != null)
        {
            float splineLength = _spline.CalculateLength();
            float distancePercentage = _naddagil.Speed * Time.deltaTime/splineLength;
            Vector3 estimatedPos = _spline.EvaluatePosition(distancePercentage);
            List<BezierKnot> knots = NaddagilUtillitys.ConvertToList<BezierKnot>((ICollection<BezierKnot>)(_spline.Spline.Knots)); 
            Vector3 PausePos = knots[2].Position;
            return Vector3.Distance(estimatedPos, PausePos) < 0.01f; 
        }
        return false; 
    }

    
}
