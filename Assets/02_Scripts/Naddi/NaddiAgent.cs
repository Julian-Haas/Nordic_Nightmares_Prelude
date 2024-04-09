using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines; 
using UnityEngine.AI;

public class NaddiAgent : MonoBehaviour
{
    public static NaddiAgent Naddi; 
    private NaddiSM _stateMachine;
    [SerializeField]
    private PatrolPath _patrolPath; 
    [SerializeField]
    private float _movementSpeed;
    private NavMeshAgent _agent;
    private SplineAnimate _splineAnimate; 
    public Vector3 PatrolPoint; 
    private void Awake()
    {
        Naddi = this;
        _stateMachine = new NaddiSM();
        _splineAnimate = this.GetComponent<SplineAnimate>();
        _splineAnimate.enabled = false; 
        _agent = this.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        WalkOnPatrol(); 
    }

    void WalkOnPatrol()
    {
        _splineAnimate.enabled = true;
        _splineAnimate.Container = _patrolPath.ActivatePatrolPath();

    }
}
