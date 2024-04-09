using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class WorldStateData {

    private static WorldStateData _instance = null;
    private WorldStateData() { }

    private List<int> _interactableStates = new List<int>();
    private List<Pathways> _pathesNaddi = new List<Pathways>();
    private List<UtburdurLandingSpot> _landingSpots = new List<UtburdurLandingSpot>();
    private List<Utburdur_Sitting>_utburdurSitting = new List<Utburdur_Sitting>();
    private NaddiStateMachine _stateMachine;
    private NaddiAwareness _naddiAwareness;

    public event System.Action<float> OnSanityChanged;

    public static WorldStateData Instance 
    {
        get 
        { 
            if (_instance == null)
            {
                //Debug.Log("WorldStateData already instantiated");
                _instance = new WorldStateData();
            }
            return _instance;    
        }
    }

    ~WorldStateData() { _instance = null; }

    //static WorldStateData() { }
    /*
    public static WorldStateData Instance {
        get
        {
            return _instance;
        }
    }
     */

    public void Reset()
    {
        _interactableStates.Clear();
        _pathesNaddi.Clear();
        _landingSpots.Clear();
        _utburdurSitting.Clear();
        _stateMachine = null;
        _naddiAwareness = null;
    }

    public void SetNaddiStateMachine(NaddiStateMachine stateMachine) { 
        //_stateMachine = stateMachine; 
    }
    public void SetNaddiAwareness(NaddiAwareness awareness) { 
        //_naddiAwareness = awareness;
    }

    public int AddInitialInteractableState(int state)
    {
        _interactableStates.Add(state);
        return _interactableStates.Count - 1;
    }

    public int AddPath(Pathways newPath)
    {
        _pathesNaddi.Add(newPath);
        return _pathesNaddi.Count - 1;
    }

    public void AddSittingUtburdur(Utburdur_Sitting utSit) { 
        _utburdurSitting.Add(utSit);
    }

    public void RemoveSittingUtburdur(Utburdur_Sitting utSit) {
        _utburdurSitting.Remove(utSit);
    }

    public void AddLandingSpot(UtburdurLandingSpot spot)
    {
        _landingSpots.Add(spot);
    }

    public UtburdurLandingSpot SwitchLandingSpot() {
        int ind = Random.Range(0, _landingSpots.Count);
        UtburdurLandingSpot spot = _landingSpots[ind];
        _landingSpots.RemoveAt(ind);
        return spot;
    }

    public int InteractableGetState( int index)
    {
        return _interactableStates[index];
    }

    public void UpdateInteractableState(int index, int newState)
    {
        _interactableStates[index] = newState;
    }

    public void SwitchNaddisPath(int index)
    {
        //_stateMachine.CallForLocationSwitch(_pathesNaddi[index]);
    }

    public List<int> ReturnInitialInteractableStates() { return _interactableStates; }

    public void AlertEnemiesInHearingRange(NoiseEmitter emitter) {
        //_naddiAwareness.CheckIfNaddiCanHearNoise(emitter);
        //foreach(var ut in _utburdurSitting)
        //{
        //    ut.CheckIfUtburdurCanHearNoise(emitter);
        //}
    }

    public void AlertNaddiOfPlayerMadness(Vector3 position) 
    {
        //_naddiAwareness.PlayerIsMad(position);
    }

    public void UpdatePlayerSanity(float newSanity)
    {
        OnSanityChanged?.Invoke(newSanity);
    }
}