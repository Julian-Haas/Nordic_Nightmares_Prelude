using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;

public class NaddiAgent : MonoBehaviour
{
    public static NaddiAgent Naddi;
    [SerializeField]
    private NaddiEyeSight _naddiEye;
    [SerializeField]
    private PatrolPath _patrolPath;
    [SerializeField]
    private Transform _playerPos;
    [SerializeField]
    private NaddiSM _naddiStateMachiene;

    private NavMeshAgent _agent;
    private SplineAnimate _splineAnimate;
    private NaddiStates _state = NaddiStates.Digging;
    private bool _executingState = false;
    private bool _foundPlayer;
    private Vector3 _playerPosLastSeen;

    private float _digDownHeight;
    private float _digUpHeight;

   
    private Vector3 PatrolPoint;

    public NaddiStates State
    {
        get{ return _state; }
        set { _state = value; }
    }

    public bool FoundPlayer
    {
        get { return _foundPlayer; }
    }

    private void Awake()
    {
        Naddi = this;
        InitSplineAnimate();
        SetUpDiggingHeight(); //prototyping -> will be removed when digging animations are ready 
        _agent = GetComponent<NavMeshAgent>();
    }

    void InitSplineAnimate()
    {
        _splineAnimate = gameObject.AddComponent<SplineAnimate>();
        _splineAnimate.enabled = false;
        _splineAnimate.PlayOnAwake = false;
        _splineAnimate.Duration = 15f; // magic number will be replaced with a function that calculate the duration with a function, so that the speed is on every patroll path equal 
    }

    private void SetUpDiggingHeight()
    {
        _digDownHeight = transform.position.y - (transform.localScale.y * 2);
        _digUpHeight = transform.position.y;
    }
    private void Update()
    {
        _foundPlayer = _naddiEye.isInsideCone();
        if (_foundPlayer)
        {
            _playerPosLastSeen = _playerPos.position;
            _naddiStateMachiene.FoundPlayer();
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
        }
    }

    private void HandleState()
    {
        switch (_state)
        {
            case NaddiStates.Digging:
                if (_executingState == false) //-> die courintine startet mehrmals ohne die if abfrage todo: bessere lösung finden
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
                if (_executingState == false)
                {
                    WalkToLastPlayerPosition();
                }
                break;
        }
    }

    private IEnumerator Digging()
    {
        _executingState = true;
        yield return StartCoroutine(Dig(_digDownHeight));
        _patrolPath.ActivatePatrolPath(); 
        Vector3 newPos = _patrolPath.GetFarthesPoint();
        newPos.y = _digDownHeight;
        transform.position = newPos;
        yield return StartCoroutine(Dig(_digUpHeight));
        _naddiStateMachiene.FinishedDigging();
        _executingState = false;
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
    }


    private void ChasePlayer()
    {
        if (_foundPlayer)
        {
            _agent.isStopped = false; 
            _splineAnimate.enabled = false;
            _executingState = true;
            _agent.SetDestination(_playerPos.position);
        }
        else
        {
            _executingState = false;
            _naddiStateMachiene.LostPlayer();
        }

    }

    private IEnumerator LookForPlayer()
    {
        _executingState = true;
        Quaternion localRot = transform.localRotation;
        Quaternion rotRight = localRot;
        Quaternion rotLeft = localRot;
        rotRight.y = localRot.y + 15;
        rotLeft.y = localRot.y - 15;
        yield return StartCoroutine(TurnNaddiAroundY(localRot, rotRight));
        yield return StartCoroutine(TurnNaddiAroundY(rotRight, localRot));
        yield return StartCoroutine(TurnNaddiAroundY(localRot, rotLeft));
        yield return StartCoroutine(TurnNaddiAroundY(rotLeft, localRot));
        _executingState = false;
        if (_naddiEye.isInsideCone())
        {
            _naddiStateMachiene.FoundPlayer();
        }
        else
        {
            _naddiStateMachiene.FinishedLookForPlayer();
        }

    }
    private void WalkToLastPlayerPosition()
    {
        _agent.SetDestination(_playerPosLastSeen);
        if (Vector3.Distance(_playerPosLastSeen, transform.position) <= 20)
        {
            _agent.isStopped = true;
            StartCoroutine(LookForPlayer());
        }
    }
    private IEnumerator TurnNaddiAroundY(Quaternion localRot, Quaternion rotDir)
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