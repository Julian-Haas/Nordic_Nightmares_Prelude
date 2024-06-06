using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NaddiValueStorage", menuName = "ScriptableObjects/EnemySettings", order = 1)]
public class NaddiValueStorage : ScriptableObject
{
    public float AttackTriggerVal;
    public float LookForPlayerVal;
    public float NaddiSpeed;
    public float ViewRange;
    public float HalfViewRadius; 
    public float HearingRange; 
}

//public class 