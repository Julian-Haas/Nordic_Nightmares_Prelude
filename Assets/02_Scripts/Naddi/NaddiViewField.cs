using UnityEngine;

public class NaddiViewField : MonoBehaviour
{
    [SerializeField]
    private float _coneRadius;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _coneOrigin;
    [SerializeField]
    private float _coneHalfAngleDegree;
    [SerializeField]
    private LayerMask ignoreLayer;

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
                RaycastHit hit;
                Vector3 raycastDir = _player.position - _coneOrigin.position;
                raycastDir.Normalize(); 
                if (Physics.Raycast(_coneOrigin.position,raycastDir, out hit, _coneRadius, ~ignoreLayer))
                {
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false; 
    }
}
