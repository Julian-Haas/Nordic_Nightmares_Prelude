using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaddiEyeSight : MonoBehaviour
{
    private float _coneRadius;
    private Transform _player;
    private Transform _coneOrigin;
    private float _coneHalfAngleDegree;

    private Color _debugPlayerInsideCone = Color.blue;
    private Color _debugPlayerOutsideCone = Color.red;


    public NaddiEyeSight(float coneRadius, Transform player, Transform origin, float coneHalfAngleDegree)
    {
        _coneRadius = coneRadius;
        _player = player;
        _coneOrigin = origin;
        _coneHalfAngleDegree = coneHalfAngleDegree;
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
                return true; 
            }
        }
        return false; 
    }
}
