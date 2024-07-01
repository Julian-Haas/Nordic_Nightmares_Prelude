//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class PatrolPath : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer naddiMeshRender;
    [SerializeField]
    private Transform _playerPosition;
    private int _indexOfNextPath;
    [SerializeField]
    private SplineContainer _closestPath;
    public List<SplineContainer> Paths = new List<SplineContainer>();
    [SerializeField]
    private Naddagil _naddi;
    [SerializeField]
    private List<string> DistanceOutput;
    public SplineContainer debugSplainRef; 

    public void ActivatePatrolPath(SplineContainer newPath)
    {
        if (newPath != _closestPath)
        {
            _naddi.NaddiHearing.ResetSoundSum(); 
            _closestPath.gameObject.SetActive(false);
            _closestPath = newPath;
            _closestPath.gameObject.SetActive(true);
            _naddi.StateMachiene.StartDigging();

        }
    }

    public SplineContainer GetActivePatrolPath()
    {
        if(_closestPath.gameObject != debugSplainRef.gameObject)
        {
            DebugFileLogger.Log("Spline check", "Spline isnt the ref spline"); 
        }
        return _closestPath; 
    }
    public Vector3 GetFarthesPoint()
    {
        return CalculateDistanceForEachKnot();
    }

    public Vector3 CalculateDistanceForEachKnot()
    {
        Vector3 farthestPoint = Vector3.zero;
        var knots = _closestPath.Spline.Knots;
        float maxDistance = 0;
        int indexOfNewStartKnot = 0;
        int i =0; 
        foreach(BezierKnot knot in knots)
        {
            float distance = Vector3.Distance(knot.Position, _playerPosition.position);
#if UNITY_EDITOR
            string distanceTXT = distance.ToString();
            DistanceOutput.Add(distanceTXT);
#endif
            if (distance >= maxDistance)
            {
                maxDistance = distance;
                farthestPoint = knot.Position;
                indexOfNewStartKnot = i;
            }
            i++; 
        }
#if UNITY_EDITOR
        DistanceOutput.Add("Done checking for nearest knot! Final result is: " + maxDistance.ToString());
#endif
        if (indexOfNewStartKnot != 0)
        {
            _closestPath.Spline = SwapKnotPoints(_closestPath.Spline, indexOfNewStartKnot);
        }
        return farthestPoint;
    }

    private Spline SwapKnotPoints(Spline spline, int indexStartSwapping)
    {
        List<BezierKnot> reorderedSpline = new List<BezierKnot>();
        if (indexStartSwapping >= 0 && indexStartSwapping < spline.Count)
        {
            for (int i = indexStartSwapping; i < spline.Count; i++)
            {
                reorderedSpline.Add(spline[i]); 
            }
            for (int i = 0; i < indexStartSwapping; i++)
            {
                reorderedSpline.Add(spline[i]);
            }
            for (int i =0; i < reorderedSpline.Count; i++)
            {
                spline.SetKnot(i, reorderedSpline[i]); 
            }
            return spline; 
        }
        else
        {
            throw new System.IndexOutOfRangeException("index was out of Range: " + indexStartSwapping + " Knot count: " + spline.Count);
        }
    }

    public bool ShouldPausePatrol(BezierKnot knot)
    {
        List<int> indexes = _closestPath.GetComponentInParent<ActivtePatrolPath>().IndexesToPauseAt;
        List<BezierKnot> knots = NaddagilUtillitys.ConvertToList<BezierKnot>((ICollection<BezierKnot>)_closestPath.Spline.Knots); 
        foreach(int index in indexes)
        {
            if (knots[index].Equals(knot))
            {
                return true; 
            }
        }
        return false; 
    }
}