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
    private float _digPosition; 
    public Vector3 PatrolPoint;

    private void Awake()
    {
        Naddi = this;
        _stateMachine = new NaddiSM();
        _splineAnimate = this.GetComponent<SplineAnimate>();
        _splineAnimate.PlayOnAwake = false; 
        _splineAnimate.enabled = false; 
        _agent = this.GetComponent<NavMeshAgent>();
        _digPosition = transform.position.y - (transform.localScale.y * 2);
    }

    private void Update()
    {
       Digging(); 
    }


    private void WalkOnPatrol()
    {
        transform.position = PatrolPoint; 
        _splineAnimate.enabled = true;
        _splineAnimate.Container = _patrolPath.ActivatePatrolPath();
        _splineAnimate.Play(); 
        
    }

    private void Digging()
    {
        
        float lerp = 0f; 
        Vector3 digPos = new Vector3(transform.position.x, _digPosition, transform.position.z);
        if (Vector3.Distance(transform.position, digPos) > 0.3f)
        {
            transform.position = Vector3.Lerp(transform.position, digPos, lerp += (1.3f*Time.deltaTime));
        }
        else
        {
            WalkOnPatrol();
        }
    }

}
