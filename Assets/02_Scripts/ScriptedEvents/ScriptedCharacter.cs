using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;



public class ScriptedCharacter : MonoBehaviour
{

    [SerializeField] private GameObject _disableCharacter;
    [SerializeField] private GameObject _enableCharacter;

    [SerializeReference, SubclassSelector] private List<ScriptedEvent> _eventList;
    private bool _scriptedEventsStarted;
    [SerializeField] private List<int> _waitForEventsToFinish;
    private int _eventFinishedCounter;

    private void Awake() {
        _scriptedEventsStarted = false;
        foreach(ScriptedEvent e in _eventList) {
            e.InitEvents(_eventList);
        }
        foreach(int index in _waitForEventsToFinish) {
            _eventList[index].EventFinished += EventsFinished;
        }
    }

    // Update is called once per frame
    void Update() {
        if(_scriptedEventsStarted == false) {
            return;
        }
        foreach(ScriptedEvent e in _eventList) {
            e.UpdateEvent(Time.deltaTime);
        }
    }

    public void StartScriptedEvents() {

        _disableCharacter.SetActive(false);
        transform.position = _disableCharacter.transform.position;
        transform.rotation = _disableCharacter.transform.rotation;
        gameObject.SetActive(true);
        _scriptedEventsStarted = true;
        _eventList[0].StartEvent();
    }
    private void EventsFinished() {
        _eventFinishedCounter = _eventFinishedCounter + 1;
        if(_eventFinishedCounter != _waitForEventsToFinish.Count) {
            return;
        }

        _enableCharacter.transform.position = transform.position;
        _enableCharacter.transform.rotation = transform.rotation;

        gameObject.SetActive(false);
        _enableCharacter.SetActive(true);
        _scriptedEventsStarted = false;
    }


    private void OnDestroy() {
        foreach(int index in _waitForEventsToFinish) {
            _eventList[index].EventFinished -= EventsFinished;
        }
        foreach(ScriptedEvent e in _eventList) {
            e.Dispose();
        }
    }
}
