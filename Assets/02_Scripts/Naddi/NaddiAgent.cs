using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines; 
using UnityEngine.AI;
using Unity.VisualScripting;

public class NaddiAgent : MonoBehaviour
{
    public static NaddiAgent Naddi;
    private NaddiSM _stateMachine;
    private NaddiEyeSight _naddiEye;
    [SerializeField]
    private PatrolPath _patrolPath;
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private Transform _playerPos;

    private NavMeshAgent _agent;
    private SplineAnimate _splineAnimate;
    private float _digDownHeight;
    private float _digUpHeight;
    private float _lerp=0; 
    private NaddiStates _state;
    private bool _diggingFinished = false; 

    public Vector3 PatrolPoint;
    [Header("Player visibility Check")]
    [SerializeField] private float _coneRadius;
    [SerializeField] private float _coneHalfAngleDegree;

    private Color _debugPlayerInsideCone = Color.blue;
    private Color _debugPlayerOutsideCone = Color.red;
    private bool _isDoingSomething=false; 

    private void Awake()
    {
        Naddi = this;
        _stateMachine = new NaddiSM();
        _splineAnimate = this.AddComponent<SplineAnimate>();
        _splineAnimate.enabled = false; 
        _splineAnimate.PlayOnAwake = false;
        _agent = this.GetComponent<NavMeshAgent>();
        _digDownHeight = transform.position.y - (transform.localScale.y * 2);
        _digUpHeight = transform.position.y;
        _naddiEye = new NaddiEyeSight(_coneRadius, _playerPos, this.transform, _coneHalfAngleDegree);
        _state = new NaddiStates();
        _state = NaddiStates.Digging;
    }
  
    private void Update()
    {
        bool seesPlayer = _naddiEye.isInsideCone();
        if (seesPlayer)
        {
            this.GetComponent<MeshRenderer>().material.color = _debugPlayerInsideCone;
        }
        else
        {
            this.GetComponent<MeshRenderer>().material.color = _debugPlayerOutsideCone;
        }
        HandleState(); 
    }

    private void WalkOnPatrol()
    {
        _isDoingSomething = true;
        transform.position = PatrolPoint; 
        _splineAnimate.enabled = true;
        _splineAnimate.Container = _patrolPath.ActivatePatrolPath();
        _splineAnimate.Play();
        _isDoingSomething = false; 
    }
    void HandleState()
    {
        if (!_isDoingSomething)
        {
            switch (_state)
            {
                case NaddiStates.Digging:
                    StartCoroutine(DigAndPatrol(_digDownHeight, _digUpHeight));
                    break;
                case NaddiStates.Patrol:
                    WalkOnPatrol();
                    break;
            }
        }
    }

    private IEnumerator DigAndPatrol(float digDownHeight, float digUpHeight)
    {
        yield return StartCoroutine(Dig(digDownHeight));
        Vector3 newPos = _patrolPath.GetFathesPoint();
        newPos.y = _digDownHeight;
        this.transform.position = newPos; 
        yield return StartCoroutine(Dig(digUpHeight));
        _state = NaddiStates.Patrol;
    }

    private IEnumerator Dig(float newHeight)
    {
        Vector3 digPos = new Vector3(transform.position.x, newHeight, transform.position.z);
        float duration = 2;
        float elapsedTime = 0f;

        float startPosition = transform.position.y;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float finalYPos = Mathf.Lerp(startPosition, digPos.y, t);
            Vector3 finalPos = transform.position;
            finalPos.y = finalYPos;
            transform.position = finalPos;
            Debug.Log(transform.position);
            elapsedTime += duration*Time.deltaTime;
            yield return null;
        }

        _diggingFinished = true;
    }


    private void ChasePlayer()
    {
        _agent.SetDestination(_playerPos.position); 
    }

    private void LookForPlayer()
    {
        throw new System.NotImplementedException(); 
    }
}