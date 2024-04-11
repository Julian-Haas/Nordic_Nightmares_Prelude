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
    [SerializeField]
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
    private float _lerp = 0;
    private NaddiStates _state;
    private bool _diggingFinished = false;
    public Vector3 PatrolPoint;

    [Header("Prototyping Variables")]
    [SerializeField]
    private MeshRenderer _naddiMR;
    [SerializeField]
    private Material _debugPlayerInsideCone;
    private Material _debugPlayerOutsideCone;
    private bool _isDoingSomething = false;
    private bool _foundPlayer;
    [SerializeField]
    private NaddiSM _naddiStateMachiene;
    private Vector3 _playerPosLastSeen;
    private bool _onPatrol = false;

    public NaddiStates State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
        }
    }

    public bool FoundPlayer
    {
        get { return _foundPlayer; }
    }

    private void Awake()
    {
         Naddi = this;
        _splineAnimate = gameObject.AddComponent<SplineAnimate>();
        _splineAnimate.enabled = false;
        _splineAnimate.PlayOnAwake = false;
        _splineAnimate.Duration = 15f;
        _splineAnimate.MaxSpeed = _movementSpeed;
        _debugPlayerOutsideCone = _naddiMR.material;
        _agent = GetComponent<NavMeshAgent>();
        _digDownHeight = transform.position.y - (transform.localScale.y * 2);
        _digUpHeight = transform.position.y;
        _state = NaddiStates.Digging;
    }

    private void Update()
    {
        _foundPlayer = _naddiEye.isInsideCone();
        if (_foundPlayer)
        {
            _playerPosLastSeen = _playerPos.position;
            _naddiStateMachiene.FoundPlayer();
            _naddiMR.material = _debugPlayerInsideCone;
        }
        else
        {
            _naddiMR.material = _debugPlayerOutsideCone;
        }
        HandleState();
    }

    private void WalkOnPatrol()
    {
        _splineAnimate.enabled = true;
        _splineAnimate.Container = _patrolPath.ActivatePatrolPath();
        _splineAnimate.Play();
        if (_foundPlayer)
        {
            _splineAnimate.Pause();
            _splineAnimate.enabled = false;
            _onPatrol = false;
        }
    }

    void HandleState()
    {
        switch (_state)
        {
            case NaddiStates.Digging:
                if (_isDoingSomething == false)
                {
                    StartCoroutine(Digging());
                }
                break;
            case NaddiStates.Patrol:
                WalkOnPatrol();
                break;
            case NaddiStates.Chase:
                ChasePlayer();
                break;
            case NaddiStates.LookForPlayer:
                if (_isDoingSomething == false)
                {
                    WalkToLastPlayerPosition();
                }
                break;
        }
    }

    private IEnumerator Digging()
    {
        _isDoingSomething = true;
        yield return StartCoroutine(Dig(_digDownHeight));
        Vector3 newPos = _patrolPath.GetFathesPoint();
        newPos.y = _digDownHeight;
        transform.position = newPos;
        yield return StartCoroutine(Dig(_digUpHeight));
        _naddiStateMachiene.FinishedDigging();
        _isDoingSomething = false;
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
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _diggingFinished = true;
    }


    private void ChasePlayer()
    {
        if (_foundPlayer)
        {
            _agent.isStopped = false; 
            _splineAnimate.enabled = false;
            _isDoingSomething = true;
            _agent.SetDestination(_playerPos.position);
        }
        else
        {
            _isDoingSomething = false;
            _naddiStateMachiene.LostPlayer();
        }

    }

    private IEnumerator LookForPlayer()
    {
        _isDoingSomething = true;
        Quaternion localRot = transform.localRotation;
        Quaternion rotRight = localRot;
        Quaternion rotLeft = localRot;
        rotRight.y = localRot.y + 15;
        rotLeft.y = localRot.y - 15;
        yield return StartCoroutine(TurnNaddiAroundY(localRot, rotRight));
        yield return StartCoroutine(TurnNaddiAroundY(rotRight, localRot));
        yield return StartCoroutine(TurnNaddiAroundY(localRot, rotLeft));
        yield return StartCoroutine(TurnNaddiAroundY(rotLeft, localRot));
        _isDoingSomething = false;
        if (_naddiEye.isInsideCone())
        {
            _naddiStateMachiene.FoundPlayer();
        }
        else
        {
            _naddiStateMachiene.FinishedLookForPlayer();
        }

    }
    void WalkToLastPlayerPosition()
    {
        _agent.SetDestination(_playerPosLastSeen);
        if (Vector3.Distance(_playerPosLastSeen, transform.position) <= 20)
        {
            _agent.isStopped = true;
            StartCoroutine(LookForPlayer());
        }
    }
    IEnumerator TurnNaddiAroundY(Quaternion localRot, Quaternion rotDir)
    {
        float duration = 4f;
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            transform.rotation = Quaternion.Lerp(localRot, rotDir, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}