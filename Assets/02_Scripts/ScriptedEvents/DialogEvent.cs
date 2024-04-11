
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]

public class DialogEvent : ScriptedEvent
{

    [SerializeField] private TMP_Text _textbox;
    [SerializeField] private string _displayText;
    [SerializeField] private float duration;
    private float _currentTime;


    public override void StartEvent() {
        base.StartEvent();
        _currentTime = 0;
        _textbox.text = _displayText;
        _textbox.gameObject.SetActive(true);
    }

    public override void UpdateEvent(float deltaTime) {
        if(_eventStarted == false) {
            return;
        }
        _currentTime = _currentTime + deltaTime;
        if(_currentTime > duration) {

            _textbox.gameObject.SetActive(false);
            InvokeEventFinished();
        }
    }

}
