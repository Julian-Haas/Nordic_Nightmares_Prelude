
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class ScriptedEvent
{
    public Action EventFinished;
    [SerializeField] private float _delay;
    private float _time;

    public float Delay {
        get {
            return _delay;
        }
    }

    protected void InvokeEventFinished() {
        EventFinished?.Invoke();
    }

    public virtual void StartEvent() {
        _time = 0;
    }

    public virtual bool UpdateEvent(float deltaTime) {
        _time = _time + deltaTime;

        if(_time >= _delay) {
            return true;
        }
        return false;
    }

    public void Dispose() {
        EventFinished = null;
    }
}
