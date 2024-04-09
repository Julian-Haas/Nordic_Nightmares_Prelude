using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    private List<int> _interactableStates = new List<int>();

    public int AddInitialInteractableState(int state)
    {
        _interactableStates.Add(state);
        return _interactableStates.Count - 1;
    }

    public List<int> ReturnInitialInteractableStates()
    {
        return _interactableStates;
    }

    public int InteractableGetState(int index)
    {
        return _interactableStates[index];
    }

    public void UpdateInteractableState(int index, int newState)
    {
        _interactableStates[index] = newState;
    }
}