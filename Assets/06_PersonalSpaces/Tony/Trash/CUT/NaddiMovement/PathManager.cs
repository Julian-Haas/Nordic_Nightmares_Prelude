using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private List<Pathways> _pathesNaddi = new List<Pathways>();
    private NaddiStateMachine _stateMachine;
    public int AddPath(Pathways newPath)
    {
        _pathesNaddi.Add(newPath);
        return _pathesNaddi.Count - 1;
    }

    public void SwitchNaddisPath(int index)
    {
        _stateMachine.CallForLocationSwitch(_pathesNaddi[index]);
    }
}