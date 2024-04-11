using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PatrolPath : MonoBehaviour
{

    [SerializeField]
    private Transform _playerPosition;
    [SerializeField]
    private GameObject _naddi;
    private int _indexOfNextPath; 
    public GameObject[] Paths = new GameObject[2];


    public SplineContainer ActivatePatrolPath()
    {
        Vector3 fathesPoint = GetFathesPoint(); 
        SplineContainer patrolWay = Paths[_indexOfNextPath].GetComponent<SplineContainer>();
        return patrolWay; 
    }
    public Vector3 GetFathesPoint()
    {
        float distance;
        float maxDistance = 0;
        Vector3 fathesPoint = Vector3.zero; 
        for (int i = 0; i < Paths.Length; i++)
        {
            SplineContainer spline = Paths[i].GetComponent<SplineContainer>();
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
        }
        return fathesPoint; 
    }
}