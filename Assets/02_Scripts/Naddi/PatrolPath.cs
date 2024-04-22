using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class PatrolPath : MonoBehaviour
{

    [SerializeField]
    private Transform _playerPosition;
    private int _indexOfNextPath;
    private GameObject _closestPath; 
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
                _closestPath = spline; 
            }
        }
        //SplineContainer patrolWay = _closestPath.GetComponent<SplineContainer>();
        return patrolPath; 
    }
    public Vector3 GetFarthesPoint()
    { 
        return CalculateDistanceForEachKnot(); 
    }

    private Vector3 CalculateDistanceForEachKnot()
    {
        SplineContainer spline = _closestPath.GetComponent<SplineContainer>();
        Vector3 fathesPoint = Vector3.zero; 
        BezierKnot[] knots = spline.Spline.ToArray();
        float distance = 0;
        float maxDistance = 0;
        foreach (BezierKnot knot in knots)
        {
            distance = Vector3.Distance(knot.Position, _playerPosition.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                fathesPoint = knot.Position;
            }
        }
        return fathesPoint; 
    }
}