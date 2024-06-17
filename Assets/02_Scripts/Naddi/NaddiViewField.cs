//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;

public class NaddiViewField : MonoBehaviour
{
    [SerializeField]
    private float _coneRadius;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _coneOrigin;
    [SerializeField, Range(0, 90), Tooltip("This is the Angle of the View Cone devided by two!")]
    private float _coneHalfAngleDegree;
    [SerializeField]
    private LayerMask ignoreLayer;
    [SerializeField]
    private NaddiValueStorage valueStorage; 
    public float ConeRadius { get { return _coneRadius; } set { _coneRadius = value;  } }
    public float HalfAngleDegree { get { return _coneHalfAngleDegree; } set { _coneHalfAngleDegree = value; } }
    public Transform ConeOrigin { get { return _coneOrigin; } }

    private void Awake()
    {
        _coneRadius = valueStorage.ViewRange;
        _coneHalfAngleDegree = valueStorage.HalfViewRadius; 
    }

    public bool isInsideCone()
    {
        Vector3 distance = _player.position - _coneOrigin.position;
        float dist = Vector3.Distance(_player.position, _coneOrigin.position);
        if (dist <= _coneRadius)
        {
            Vector3 normalDist = Vector3.Normalize(distance);
            Vector3 normalViewDir = Vector3.Normalize(_coneOrigin.forward);
            float dotProduct = Vector3.Dot(normalDist, normalViewDir);
            float angleRad = Mathf.Acos(dotProduct);
            float angleDegree = angleRad * Mathf.Rad2Deg;
            if (angleDegree <= _coneHalfAngleDegree)
            {
                Vector3 offset = new Vector3((_player.localScale.x / 2), 0, 0);
                Vector3[] playerSides = new Vector3[2];
                playerSides[0] = _player.position - offset;
                playerSides[1] = _player.position + offset;
                RaycastHit hit;
                foreach (var playerSide in playerSides) 
                {
                    Vector3 raycastDir = playerSide - _coneOrigin.position;
                    raycastDir.Normalize();
                    if (Physics.Raycast(_coneOrigin.position, raycastDir, out hit, _coneRadius, ~ignoreLayer))
                    {
                        if (hit.collider.gameObject.CompareTag("Player"))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false; 
    }
}