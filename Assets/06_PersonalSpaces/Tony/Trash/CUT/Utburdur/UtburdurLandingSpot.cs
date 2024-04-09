using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UtburdurLandingSpot : MonoBehaviour
{
    private void Awake()
    {
        WorldStateData.Instance.AddLandingSpot(this);
    }
}