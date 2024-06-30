using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class MoveOnSpline : MonoBehaviour
{
    [SerializeField]
    private SplineContainer _spline = null;
    private Naddagil _naddagil = null;

    private float _distancePercentage = 0f;
    private float _splineLength = 0f;
    private float threshold = 0.1f;
    private Vector3 _posOnSpline = new Vector3(0, 0, 0);
    private List<BezierKnot> knots = new List<BezierKnot>();

    private bool isPaused = false;
    private bool isStopped = false;


    private void Awake()
    {
        _naddagil = this.GetComponent<Naddagil>();
    }
    public void SetSpline(ref SplineContainer newSpline)
    {
        _spline = newSpline;
        _splineLength = _spline.CalculateLength();
        knots = NaddagilUtillitys.ConvertToList<BezierKnot>((ICollection<BezierKnot>)_spline.Spline.Knots); 
    }

    public void Play()
    {
        if (isStopped)
        {
            return;
        }
        
        if (_spline == null)
        {
            throw new System.NullReferenceException("_spline is NULL! \n Did you forget to call SetSpline(SplineContainer newSpline) before calling Play()?"); 
        }
        if (_distancePercentage > 1)
        {
            _distancePercentage = 0f;
            _naddagil.PatrolBehaviour.SetAllreadyPaused(false); 
        }
        _distancePercentage += _naddagil.Speed * Time.deltaTime / _splineLength;
        _posOnSpline = _spline.EvaluatePosition(_distancePercentage);
        transform.position = _posOnSpline;
        RotateAlongSpline(); 
        

    }

    public void Resume()
    {
        isStopped = false; 
    }
    public void Stop()
    {
        _distancePercentage = 0f;
        isStopped = true; 

    }

    public void Pause()
    {
        isStopped = true; 
    }
    void RotateAlongSpline()
    {
        Vector3 nxtPos = _spline.EvaluatePosition(_distancePercentage + 0.05f);
        Vector3 dir = nxtPos - _posOnSpline;
        transform.rotation = Quaternion.LookRotation((dir), transform.up); 
    }
    public bool CheckIfShouldPause(int IndexToPause)
    {
        float index = SplineUtility.ConvertIndexUnit<Spline>(_spline.Spline, _distancePercentage, PathIndexUnit.Knot);
        int indexOfCurKnot = (int)index; 
        return IndexToPause == indexOfCurKnot; 
    }
}   