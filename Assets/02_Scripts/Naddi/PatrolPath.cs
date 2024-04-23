using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class PatrolPath : MonoBehaviour
{

    [SerializeField]
    private Transform _playerPosition;
    private int _indexOfNextPath;
    private SplineContainer _closestPath;
    public List<GameObject> Paths = new List<GameObject>();


    public SplineContainer ActivatePatrolPath()
    {
        float minDistance = float.MaxValue;
        SplineContainer patrolPath = null;
        foreach (GameObject spline in Paths)
        {
           patrolPath = spline.GetComponent<SplineContainer>();
            foreach (BezierKnot knot in patrolPath.Spline)
            {
                float distance = Vector3.Distance(_playerPosition.position, knot.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _closestPath = patrolPath;
                }
            }
           //float dist = Vector3.Distance(_playerPosition.position, patrolPath.Spline[0].Position);
           //if (dist < minDistance)
           //{
           //    Debug.Log("Distance: " + dist + " minDist: " + minDistance);
           //    minDistance = dist;
           //    _closestPath = patrolPath;
           //    Debug.Log(_closestPath.name); 
           //}
        }

        return _closestPath;
    }
    public Vector3 GetFarthesPoint()
    {
        return CalculateDistanceForEachKnot();
    }

    private Vector3 CalculateDistanceForEachKnot()
    {
        Vector3 farthestPoint = Vector3.zero;
        var knots = _closestPath.Spline.Knots;
        float maxDistance = 0;
        int indexOfNewStartKnot = 0;
        int i =0; 
        foreach(BezierKnot knot in knots)
        {
            float distance = Vector3.Distance(knot.Position, _playerPosition.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPoint = knot.Position;
                indexOfNewStartKnot = i;
            }
            i++; 
        }
        Debug.Log(_closestPath.name + " " + indexOfNewStartKnot);
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
            throw new System.IndexOutOfRangeException("index was out of Range: " + indexStartSwapping);
        }
    }

}