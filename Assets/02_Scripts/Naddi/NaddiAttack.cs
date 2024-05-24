using UnityEngine;
using UnityEngine.AI; 
public class NaddiAttack : MonoBehaviour
{
    private NaddiViewField _naddiEye;
    private Naddi _naddi;
    private NaddiStateMaschine _naddiStateMachiene;
    private void Awake()
    {
        _naddi = GetComponent<Naddi>();
        _naddiEye = GetComponent<NaddiViewField>();
        _naddiStateMachiene = GetComponent<NaddiStateMaschine>();
    }
    public void ChasePlayer(Transform playerPos)
    {
        if (_naddi.CanChasePlayer)
        {
            float sqrMagnitude = (playerPos.position - this.transform.position).sqrMagnitude;
            if (_naddiEye.isInsideCone() && sqrMagnitude <= Mathf.Pow(_naddi.Agent.stoppingDistance, 2) && !_naddi.PlayerInSafeZone)
            {
                _naddi.ChasePlayer = true;
                _naddi.Agent.isStopped = true;
                _naddiStateMachiene.AttackPlayer();
            }
            else if (((_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_naddi.Agent.stoppingDistance, 2)) || (sqrMagnitude < Mathf.Pow(_naddi.Agent.stoppingDistance * 5, 2) && sqrMagnitude > Mathf.Pow(_naddi.Agent.stoppingDistance, 2))) && !_naddi.PlayerInSafeZone)
            {
                _naddi.ChasePlayer = true;
                _naddi.Agent.SetDestination(playerPos.position);
            }
            else if (!_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_naddi.Agent.stoppingDistance * 5, 2) && !_naddi.PlayerInSafeZone)
            {
                _naddi.ChasePlayer = false;
                _naddiStateMachiene.LostPlayer();
            }
        } 
    }

    public void WalkToLastPlayerPosition(Vector3 playerPosLastSeen)
    {
        _naddi.Agent.SetDestination(playerPosLastSeen);
        float sqrDistance = (playerPosLastSeen - this.transform.position).sqrMagnitude;
        if (sqrDistance <= Mathf.Pow(5, 2))
        {
            _naddi.StateMachiene.LookForPlayer();
            _naddi.Agent.isStopped = true;
            _naddi.ChasePlayer = false;
        }
    }

    public void DigToPlayer(Transform playerPos, Terrain terrain)
    {
        bool _validPos = false;
        float offsetX;
        float offsetZ;
        while (_validPos == false)
        {
            offsetX = RandomOffset();
            offsetZ = RandomOffset();
            Vector3 digOutPos = new Vector3(playerPos.position.x + offsetX, 0, playerPos.position.z + offsetZ);
            float terrainhight = terrain.SampleHeight(digOutPos);
            digOutPos.y = terrainhight;
            if (IsValidNavMesh(digOutPos))
            {
                this.transform.position = digOutPos;
                _validPos = true;
            }
        }
        _naddiStateMachiene.FoundPlayer();
    }

    private float RandomOffset()
    {
        float val = Random.Range(5, 7);
        float signOffset = Random.Range(0, 1);
        if (signOffset == 0)
            val *= -1;
        return val;
    }
    bool IsValidNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }
}
