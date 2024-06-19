using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines; 
public class ActivtePatrolPath : MonoBehaviour
{
    [SerializeField]
    private PatrolPath _pathHolder;
    [SerializeField]
    private SplineContainer _spline;

    public List<int> IndexesToPauseAt = new List<int>();  

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.gameObject.name != "Path(4)")
            {
                Debug.Log("schau nach warum hier der trigger ausgelöst wird!"); 
            } 
            _pathHolder.ActivatePatrolPath(_spline);
        } 
    }

}
