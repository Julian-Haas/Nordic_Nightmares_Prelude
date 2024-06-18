using UnityEngine;
using UnityEngine.AI; 
public class NaddiAttack : MonoBehaviour
{
    private NaddiViewField _naddiEye;
    private Naddi _naddi;
    private NaddiStateMaschine _naddiStateMachiene;
    [Tooltip("this Vector stores the start init value of player pos last seen, so that we can check if the value got actually updated or not")]
    private Vector3 invalidVector = new Vector3(-999999, -9999999, -999999);
    private void Awake()
    {
        _naddi = GetComponent<Naddi>();
        _naddiEye = GetComponent<NaddiViewField>();
        _naddiStateMachiene = GetComponent<NaddiStateMaschine>();
    }

    private void Update()
    {
        if (_naddi.KilledPlayer)
        {
            _naddi.CanChasePlayer = false;
        }
        if (_naddi.State != NaddiStateEnum.Digging && !_naddi.RendererEnabled && !_naddi.StateMachiene.GetNaddiMeshRenderer.enabled)
        {
            _naddi.RendererEnabled = true;
            _naddi.StateMachiene.GetNaddiMeshRenderer.enabled = true;
        }
        if (_naddi.PlayerInSafeZone && (_naddi.State == NaddiStateEnum.Chase || _naddi.State == NaddiStateEnum.Attack))
        {
            _naddi.StateMachiene.PlayerVanished();
        }
        if (_naddi.NaddiEye.isInsideCone() && _naddi.State != NaddiStateEnum.Digging)
        {
            _naddi.PlayerPosLastSeen = _naddi.PlayerPos.position;
            _naddi.StateMachiene.FoundPlayer();
        }
        if (_naddi.State != NaddiStateEnum.Patrol && _naddi.PatrolBehaviour.SplineAnimate.enabled)
        {
            _naddi.PatrolBehaviour.SplineAnimate.enabled = false;
        }
    }
    private float offset = 0.3f; 
    public void ChasePlayer(Transform playerPos)
    {
        if (_naddi.CanChasePlayer)
        {
            float sqrMagnitude = (playerPos.position - this.transform.position).sqrMagnitude;
            if (_naddiEye.isInsideCone() && sqrMagnitude <= Mathf.Pow(_naddi.Agent.stoppingDistance + offset, 2) && !_naddi.PlayerInSafeZone)
            {
                _naddi.ChasePlayer = true;
                _naddi.Agent.isStopped = true;
                _naddiStateMachiene.AttackPlayer();
            }
            else if (((_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_naddi.Agent.stoppingDistance + offset, 2)) || (sqrMagnitude < Mathf.Pow(_naddi.Agent.stoppingDistance * 5, 2) && sqrMagnitude > Mathf.Pow(_naddi.Agent.stoppingDistance, 2))) && !_naddi.PlayerInSafeZone)
            {
                _naddi.ChasePlayer = true;
                _naddi.Agent.SetDestination(playerPos.position);
            }
            else if (!_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow((_naddi.Agent.stoppingDistance+ offset) * 5, 2) && !_naddi.PlayerInSafeZone)
            {
                _naddi.ChasePlayer = false;
                _naddiStateMachiene.LostPlayer();
            }
        } 
    }

    public void WalkToLastPlayerPosition(Vector3 playerPosLastSeen)
    {

        float sqrDistance = (playerPosLastSeen - this.transform.position).sqrMagnitude;
        if (sqrDistance <= Mathf.Pow(_naddi.Agent.stoppingDistance + offset, 2) || playerPosLastSeen == invalidVector)
        {
            _naddi.StateMachiene.LookForPlayer();
            _naddi.Agent.isStopped = true;
            _naddi.ChasePlayer = false;
        }
        _naddi.Agent.SetDestination(playerPosLastSeen);
    }

    public void DigToPlayer(Transform playerPos, Terrain terrain)
    {
        bool _validPos = false;
        float offsetX;
        float offsetZ;
        while (_validPos == false)
        {
            offsetX = NaddiUtillitys.RandomOffset(5, 7);
            offsetZ = NaddiUtillitys.RandomOffset(5, 7);
            Vector3 digOutPos = new Vector3(playerPos.position.x + offsetX, 0, playerPos.position.z + offsetZ);
            float terrainhight = terrain.SampleHeight(digOutPos);
            digOutPos.y = terrainhight;
            if (NaddiUtillitys.IsValidNavMesh(digOutPos))
            {
                this.transform.position = digOutPos;
                _validPos = true;
            }
        }
        _naddiStateMachiene.FoundPlayer();
    }
}