using UnityEngine;
using UnityEngine.AI; 
public class NaddagilAttackBehaviour : MonoBehaviour
{
    
    [SerializeField, Header("Private Dependencies")]
    private Naddagil _naddi;

    [Header("Public References")]
    public Vector3 PlayerPosLastSeen = new Vector3(-999999, -9999999, -999999);
    public Transform PlayerPos;

    public NavMeshAgent Agent { get; private set; }
    [Header("Public Flags")]
    public bool KilledPlayer = false;
    public bool ChasePlayer = false;
    public bool StopAgent = false;
    public bool CanChasePlayer = true;
    public bool PlayerWasInSafeZone;
    public bool PlayerInSafeZone;

    [Tooltip("this Vector stores the start init value of player pos last seen, so that we can check if the value got actually updated or not")]
    private Vector3 invalidVector = new Vector3(-999999, -9999999, -999999);
    private void Awake()
    {
        Agent = this.GetComponent<NavMeshAgent>(); 
    }
    private void Update()
    {
        PlayerInSafeZone = _naddi.PlayerCol._inSafeZone;
        if (KilledPlayer)
        {
            CanChasePlayer = false;
        }
        if (_naddi.State != NaddiStates.Digging && !_naddi.RendererEnabled && !_naddi.MeshRenderer.enabled)
        {
            _naddi.RendererEnabled = true;
            _naddi.MeshRenderer.enabled = true;
        }
        if (PlayerInSafeZone && (_naddi.State == NaddiStates.Chase || _naddi.State == NaddiStates.Attack))
        {
            _naddi.StateMachiene.PlayerVanished();
        }
        if (_naddi.NaddiEye.isInsideCone() && _naddi.State != NaddiStates.Digging)
        {
            PlayerPosLastSeen = PlayerPos.position;
            _naddi.StateMachiene.FoundPlayer();
        }
        if (_naddi.State != NaddiStates.Patrol && _naddi.PatrolBehaviour.SplineAnimate.enabled)
        {
            _naddi.PatrolBehaviour.SplineAnimate.enabled = false;
        }
    }
    private float offset = 0.3f; 
    public void Chase()
    {
        if (CanChasePlayer)
        {
            float sqrMagnitude = (PlayerPos.position - this.transform.position).sqrMagnitude;
            if (_naddi.NaddiEye.isInsideCone() && sqrMagnitude <= Mathf.Pow(Agent.stoppingDistance + offset, 2) && !PlayerInSafeZone)
            {
                ChasePlayer = true;
                Agent.isStopped = true;
                _naddi.StateMachiene.AttackPlayer();
            }
            else if (((_naddi.NaddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(Agent.stoppingDistance + offset, 2)) || (sqrMagnitude < Mathf.Pow(Agent.stoppingDistance * 5, 2) && sqrMagnitude > Mathf.Pow(Agent.stoppingDistance, 2))) && !PlayerInSafeZone)
            {
                ChasePlayer = true;
                Agent.SetDestination(PlayerPos.position);
            }
            else if (!_naddi.NaddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow((Agent.stoppingDistance+ offset) * 5, 2) && !PlayerInSafeZone)
            {
                ChasePlayer = false;
               _naddi.StateMachiene.LostPlayer();
            }
        } 
    }

    public void WalkToLastPlayerPosition()
    {

        float sqrDistance = (PlayerPosLastSeen - this.transform.position).sqrMagnitude;
        if (sqrDistance <= Mathf.Pow(Agent.stoppingDistance + offset, 2) || PlayerPosLastSeen == invalidVector)
        {
            _naddi.StateMachiene.LookForPlayer();
            Agent.isStopped = true;
            ChasePlayer = false;
        }
        Agent.SetDestination(PlayerPosLastSeen);
    }

    public void DigToPlayer(Transform playerPos, Terrain terrain)
    {
        bool _validPos = false;
        float offsetX;
        float offsetZ;
        while (_validPos == false)
        {
            offsetX = NaddagilUtillitys.RandomOffset(5, 7);
            offsetZ = NaddagilUtillitys.RandomOffset(5, 7);
            Vector3 digOutPos = new Vector3(playerPos.position.x + offsetX, 0, playerPos.position.z + offsetZ);
            float terrainhight = terrain.SampleHeight(digOutPos);
            digOutPos.y = terrainhight;
            if (NaddagilUtillitys.IsValidNavMesh(digOutPos))
            {
                this.transform.position = digOutPos;
                _validPos = true;
            }
        }
        _naddi.StateMachiene.FoundPlayer();
    }
}