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
        NaddiAgent.Naddi.PatrolPoint = GetFathesPoint();
        SplineContainer patrolWay = Paths[_indexOfNextPath].GetComponent<SplineContainer>();
        return patrolWay; 
    }
    private Vector3 GetFathesPoint()
    {
        float distance;
        float maxDistance = 0;
        for(int i = 0;  i < Paths.Length; i++)
        {
            distance = Vector3.Distance(_playerPosition.position, Paths[i].transform.position);
            if (distance >= maxDistance)
            {
                maxDistance = distance;
                _indexOfNextPath = i; 
            }
        }
        return Paths[_indexOfNextPath].transform.position; 
    }
}