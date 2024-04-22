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
            float dist = Vector3.Distance(_playerPosition.position, patrolPath.Spline[0].Position);
            if (dist < minDistance)
            {
                Debug.Log("Distance: " + dist + " minDist: " + minDistance);
                minDistance = dist;
                _closestPath = patrolPath;
            }
        }

        return _closestPath;
    }
    public Vector3 GetFarthesPoint()
    {
        return CalculateDistanceForEachKnot();
    }

    private Vector3 CalculateDistanceForEachKnot()
    {
        SplineContainer spline = _closestPath.GetComponent<SplineContainer>();
        Vector3 farthestPoint = Vector3.zero;
        var knots = spline.Spline.Knots;
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
        int x = 0; 
        foreach (BezierKnot knot in knots)
        {
            Debug.Log("index: "+ x +"Position: " + knot.Position); 
            x++; 
        }
        spline.Spline.Knots = SwapKnotPoints(spline.Spline, indexOfNewStartKnot);
        x = 0; 
        foreach (BezierKnot knot in knots)
        {
            Debug.Log("index: " + x + "Position: " + knot.Position);
            x++;
        }
        return farthestPoint;
    }

    private List<BezierKnot> SwapKnotPoints(Spline spline, int indexStartSwapping)
    {
        List<BezierKnot> swappedKnots = new List<BezierKnot>();

        for (int i = indexStartSwapping; i < spline.Count; i++)
        {
            swappedKnots.Add(spline[i]);
        }

        for (int i = 0; i < indexStartSwapping; i++)
        {
            swappedKnots.Add(spline[i]);
        }

        return swappedKnots;
    }
}