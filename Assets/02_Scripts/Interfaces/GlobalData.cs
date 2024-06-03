using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GlobalData : MonoBehaviour
{
    private GlobalData() {
    }
    private static GlobalData _globalDataInstance;

    public static GlobalData GlobalDataInstance {
        get {
            if(_globalDataInstance == null) {
                GameObject globalDataObject = new GameObject(typeof(GlobalData).Name);
                _globalDataInstance = globalDataObject.AddComponent<GlobalData>();
                DontDestroyOnLoad(_globalDataInstance.gameObject);
            }
            return _globalDataInstance;
        }
    }

}



//reference
//function to register
//function to get

// soundmanager
// player
// naddi




/*

private static GlobalData _instance;
private IEnemyAction _enemy;
private IPlayerAction _player;
private float _awarenessValue = 0.0f, _sanityValue = 100.0f;
public float _suspicious, _aware, _alert, _hunting;
private List<int> _interactableStates = new List<int>();
private List<Pathways> _pathesNaddi = new List<Pathways>();
private List<UtburdurLandingSpot> _landingSpots = new List<UtburdurLandingSpot>();
private List<Utburdur_Sitting>_utburdurSitting = new List<Utburdur_Sitting>();
// camera variableu
// etc...

public static GlobalData Instance {
    get {
        if (_instance == null) {
            _instance = new GlobalData();
        }
        return _instance;
    }
}
public void ResetValues()
{
    _sanityValue = 100.0f;
    _awarenessValue = 0.0f;
    _enemy = null;
    _player = null;
    _interactableStates.Clear();
    _pathesNaddi.Clear();
    _landingSpots.Clear();
    _utburdurSitting.Clear();
}

public float GetAwareness() { return _awarenessValue; }
public void UpdateAwareness(float input) { _awarenessValue += input; }
public float GetSanity() { return _sanityValue; }
public void UpdateSanity(float input ) { 
    _sanityValue += input;
    _sanityValue = Mathf.Clamp(_sanityValue, 0.0f, 100.0f); ;
}

public void SetNaddiStateValues(float sus, float aware, float alert, float hunt) {
    _suspicious = sus;
    _aware = aware;
    _alert = alert;
    _hunting = hunt;
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

public void SwitchPath(int index)
{
    _enemy.SwitchLocation(_pathesNaddi[index]);
}

public List<int> ReturnInitialInteractableStates() { return _interactableStates; }

public void TransferSoundToNaddi(NoiseEmitter emitter) {
    _enemy.CheckHearing(emitter);
    foreach(var ut in _utburdurSitting)
    {
        ut.CheckHearing(emitter);
    }
}

public GameObject GetNaddi()
{
    return GameObject.Find("Naddi");
}

public IEnemyAction Enemy {
    get {
        return _enemy;
    }

    set {
        _enemy = value;
    }
}

public IPlayerAction Player {
    get {
        return _player;
    }

    set {
        _player = value;
    }
}

private GlobalData() {

}
 */