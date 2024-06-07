using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWillOTheWispList",menuName = "Custom/WillOTheWispList")]
public class Waypoints : ScriptableObject
{
    public List<GameObject> waypoints = new List<GameObject>();

    public void AddWaypoint(GameObject waypoint) {
        waypoints.Add(waypoint);
    }

    public void ClearWaypoints() {
        waypoints.Clear();
    }
}
