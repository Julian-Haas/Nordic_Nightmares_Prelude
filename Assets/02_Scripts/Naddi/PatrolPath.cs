using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PatrolPath : MonoBehaviour
{

    [SerializeField]
    private Transform _playerPosition;
    private int _indexOfNextPath; 
    public GameObject[] Paths = new GameObject[2];


    public SplineContainer ActivatePatrolPath()
    {
        GetFathesPoint(); 
        SplineContainer patrolWay = Paths[_indexOfNextPath].GetComponent<SplineContainer>();
        return patrolWay; 
    }
    public Vector3 GetFathesPoint()
    {
        float distance = 0;
        float maxDistance = 0;
        Vector3 fathesPoint = Vector3.zero; 
        for (int i = 0; i < Paths.Length; i++)
        {
           fathesPoint =  CalculateDistanceForEachKnot(distance, maxDistance, i); 
        }
        return fathesPoint; 
    }

    private Vector3 CalculateDistanceForEachKnot(float distance, float maxDistance, int i)
    {
        SplineContainer spline = Paths[i].GetComponent<SplineContainer>();
        Vector3 fathesPoint = Vector3.zero; 
        BezierKnot[] knots = spline.Spline.ToArray();
        foreach (BezierKnot knot in knots)
        {
            distance = Vector3.Distance(knot.Position, _playerPosition.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                fathesPoint = knots[0].Position;
                _indexOfNextPath = i;
            }
        }
        return fathesPoint; 
    }
}