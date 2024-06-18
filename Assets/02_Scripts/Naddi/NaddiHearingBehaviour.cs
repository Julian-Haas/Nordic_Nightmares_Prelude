using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaddiHearingBehaviour : MonoBehaviour
{
    [SerializeField]
    private Naddi _naddi;

    private void Start()
    {
        _naddi.NaddiHearing.LookForPlayerAction += SusSoundHeard;
        _naddi.NaddiHearing.AttackPlayerAction += HeardPlayerNearby;
    }
    public void SusSoundHeard(Vector3 pos)
    {
        if (_naddi.State != NaddiStateEnum.Chase && _naddi.State != NaddiStateEnum.Attack && _naddi.State != NaddiStateEnum.Digging && !_naddi.HeardPlayer)
        {
            _naddi.HeardPlayer = true;
            _naddi.StateMachiene.HearedSomething();
            _naddi.PatrolBehaviour.DeactivatePatrol();
            StartCoroutine(TurnToSoundDirection(pos));
        }
    }

    public void HeardPlayerNearby()
    {
        _naddi.StateMachiene.FoundPlayer();
    }
    public IEnumerator HearingDelay()
    {
        _naddi.HeardPlayer = true;
        yield return new WaitForSeconds(10f);
        _naddi.HeardPlayer = false;
    }
    private IEnumerator TurnToSoundDirection(Vector3 soundPos)
    {
        Debug.Log("executing turn to player!");
        Quaternion desiredRotation;
        Vector3 direction = (soundPos - transform.position).normalized;
        desiredRotation = Quaternion.LookRotation(direction, transform.up);
        float lerpFactor = 1 * Time.deltaTime;
        float time = 0f;
        while (time <= 2)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, time * lerpFactor);
            time += Time.deltaTime;
            yield return null;
        }
        _naddi.StateMachiene.LookForPlayer();
    }
}
