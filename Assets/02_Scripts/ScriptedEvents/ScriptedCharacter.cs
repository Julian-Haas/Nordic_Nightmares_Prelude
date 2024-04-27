using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;



public class ScriptedCharacter : MonoBehaviour
{

    [SerializeField] private GameObject _disableCharacter;
    [SerializeField] private GameObject _enableCharacter;
    [SerializeField] private Vector3 _enableOffset;

    [SerializeField] private bool _takeCamera;

    [SerializeField] private MoveToPositionEvent _moveToStartPosition;
    [SerializeField] private AnimationEvent _startAnimation;

    [SerializeReference, SubclassSelector] private List<ScriptedEvent> _moveEvents;
    [SerializeReference, SubclassSelector] private List<ScriptedEvent> _dialogEvents;
    [SerializeReference, SubclassSelector] private List<ScriptedEvent> _animationEvents;

    private int _moveEventCounter;
    private int _dialogEventCounter;
    private int _animationEventCounter;

    private PlayerFollow _followPlayerCamera;

    private bool _startPositionReached;


    // Update is called once per frame
    void Update() {

        if(_startPositionReached == false) {
            _moveToStartPosition.UpdateEvent(Time.deltaTime);

        }
        else {
            if(_moveEvents.Count > _moveEventCounter) {
                _moveEvents[_moveEventCounter].UpdateEvent(Time.deltaTime);
            }
            if(_dialogEvents.Count > _dialogEventCounter) {
                _dialogEvents[_dialogEventCounter].UpdateEvent(Time.deltaTime);
            }
            if(_animationEvents.Count > _animationEventCounter) {
                _animationEvents[_animationEventCounter].UpdateEvent(Time.deltaTime);
            }
        }

    }


    public void StartScriptedEvents() {
        _followPlayerCamera = Camera.main.GetComponent<PlayerFollow>();
        _startPositionReached = false;
        _disableCharacter.SetActive(false);
        transform.position = _disableCharacter.transform.position;
        transform.rotation = _disableCharacter.transform.rotation;
        if(_takeCamera == true && _followPlayerCamera != null) {
            _followPlayerCamera.target = gameObject.transform;
        }


        gameObject.SetActive(true);
        _startAnimation.UpdateEvent(0);

        _moveToStartPosition.EventFinished += StartMovementFinished;
        _moveToStartPosition.StartEvent();



    }


    private void MoveEventFinished() {
        _moveEvents[_moveEventCounter].EventFinished -= MoveEventFinished;
        _moveEventCounter = _moveEventCounter + 1;
        if(_moveEventCounter < _moveEvents.Count) {
            _moveEvents[_moveEventCounter].EventFinished += MoveEventFinished;
            _moveEvents[_moveEventCounter].StartEvent();
        }
        else {
            CheckAllEventsFinished();
        }
    }
    private void DialogEventFinished() {
        _dialogEvents[_dialogEventCounter].EventFinished -= DialogEventFinished;
        _dialogEventCounter = _dialogEventCounter + 1;
        if(_dialogEventCounter < _dialogEvents.Count) {
            _dialogEvents[_dialogEventCounter].EventFinished += DialogEventFinished;
            _dialogEvents[_dialogEventCounter].StartEvent();
        }
        else {
            CheckAllEventsFinished();
        }
    }
    private void AnimationEventFinished() {
        _animationEvents[_animationEventCounter].EventFinished -= AnimationEventFinished;
        _animationEventCounter = _animationEventCounter + 1;
        if(_animationEventCounter < _animationEvents.Count) {
            _animationEvents[_animationEventCounter].EventFinished += AnimationEventFinished;
            _animationEvents[_animationEventCounter].StartEvent();
        }
        else {
            CheckAllEventsFinished();
        }
    }
    private void StartMovementFinished() {
        _startPositionReached = true;
        _moveToStartPosition.EventFinished -= StartMovementFinished;

        if(_moveEvents.Count > 0) {
            _moveEvents[_moveEventCounter].EventFinished += MoveEventFinished;
            _moveEvents[_moveEventCounter].StartEvent();
        }
        if(_dialogEvents.Count > 0) {
            _dialogEvents[_dialogEventCounter].EventFinished += DialogEventFinished;
            _dialogEvents[_dialogEventCounter].StartEvent();
        }
        if(_animationEvents.Count > 0) {
            _animationEvents[_animationEventCounter].EventFinished += AnimationEventFinished;
            _animationEvents[_animationEventCounter].StartEvent();
        }
    }

    private void CheckAllEventsFinished() {
        if(_moveEventCounter >= _moveEvents.Count && _dialogEventCounter >= _dialogEvents.Count && _animationEventCounter >= _animationEvents.Count) {
            _enableCharacter.transform.position = transform.position + _enableOffset;
            _enableCharacter.transform.rotation = transform.rotation;

            gameObject.SetActive(false);

            if(_takeCamera == true && _followPlayerCamera != null) {
                _followPlayerCamera.target = _enableCharacter.transform;
            }

            _enableCharacter.SetActive(true);
        }
    }

    private void OnDestroy() {
        _moveToStartPosition.Dispose();

        foreach(ScriptedEvent e in _moveEvents) {
            e.Dispose();
        }
        foreach(ScriptedEvent e in _dialogEvents) {
            e.Dispose();
        }
        foreach(ScriptedEvent e in _animationEvents) {
            e.Dispose();
        }
    }
}
