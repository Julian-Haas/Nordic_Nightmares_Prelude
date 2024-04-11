
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]

public abstract class ScriptedEvent
{
    public Action EventFinished;
    [SerializeField] private List<int> _waitForEventsToFinish;
    private List<ScriptedEvent> _subscribedEvents;
    protected bool _eventStarted;


    public void InitEvents(List<ScriptedEvent> eventContainer) {
        _eventStarted = false;
        _subscribedEvents = new List<ScriptedEvent>();
        foreach(int index in _waitForEventsToFinish) {
            eventContainer[index].EventFinished += StartEvent;
            _subscribedEvents.Add(eventContainer[index]);
        }

    }

    protected void InvokeEventFinished() {
        EventFinished?.Invoke();
        _eventStarted = false;
        Debug.Log("event finished");
    }

    public virtual void StartEvent() {
        _eventStarted = true;
        Debug.Log("event started");
    }

    public virtual void UpdateEvent(float deltaTime) {

    }

    public void Dispose() {
        foreach(ScriptedEvent events in _subscribedEvents) {
            events.EventFinished -= StartEvent;
        }
        EventFinished = null;
    }
}
