using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WillOTheWisp : Triggerable
{
    [SerializeField] private GameObject _willOTheWisp;
    [SerializeField] private GameObject _triggerCollider;
    [SerializeField] private Light _lightSource;
    [SerializeField] private List<GameObject> _waypoints;
    [SerializeField] private float _shrinktime;
    [SerializeField] private float _floatHeight;
    [SerializeField] private float _floatSpeed;

    private Vector3 _startPosition;
    private bool _floating = true;
    private float yOffset = 0f;


    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if(_waypoints.Count > 0) {
            Gizmos.DrawLine(_willOTheWisp.transform.position,_waypoints[0].transform.position);
            DrawLabel(_willOTheWisp.transform.position,"0");
            DrawLabel(_waypoints[0].transform.position,"1");
        }
        if(_waypoints.Count < 2) {
            return;
        }
        for(int i = 0; i < _waypoints.Count - 1; i++) {
            Vector3 startPos = _waypoints[i].transform.position;
            Vector3 endPos = _waypoints[i + 1].transform.position;
            Gizmos.DrawLine(startPos,endPos);
            DrawLabel(endPos,(i + 2).ToString());
        }
    }

    private void DrawLabel(Vector3 position,string label) {
        // Display number labels at the positions
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 20;
        Handles.Label(position,label,style);
    }

    public void AddWaypoint(GameObject waypoint) {
        _waypoints.Add(waypoint);
    }


    void Start() {
        _startPosition = _willOTheWisp.transform.position;
    }
    void Update() {
        if(_floating) {
            float yOffset = Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;
            _willOTheWisp.transform.position = _startPosition + new Vector3(0f,yOffset,0f);
        }
    }
    public override void Trigger() {
        if(_waypoints.Count == 0) {
            StartCoroutine(Vanish());
        }
        else {
            StartCoroutine(Teleport());
        }
    }
    private IEnumerator Teleport() {
        _triggerCollider.SetActive(false);
        Vector3 originalScale = _willOTheWisp.transform.localScale;
        float originalIntensity = _lightSource.intensity;
        float timer = 0f;
        while(timer < _shrinktime) {
            float scaleProgress = timer / _shrinktime;
            Vector3 newScale = Vector3.Lerp(originalScale,Vector3.zero,scaleProgress);
            _willOTheWisp.transform.localScale = newScale;
            float dimProgress = timer / _shrinktime;
            float newIntensity = Mathf.Lerp(originalIntensity,0f,dimProgress);
            _lightSource.intensity = newIntensity;
            timer += Time.deltaTime;
            yield return null;
        }
        _willOTheWisp.transform.localScale = Vector3.zero;
        _lightSource.intensity = 0f;
        _willOTheWisp.transform.position = _waypoints[0].transform.position + new Vector3(0f,yOffset,0f);
        _startPosition = _willOTheWisp.transform.position;
        timer = 0f;
        while(timer < _shrinktime) {
            float scaleProgress = timer / _shrinktime;
            Vector3 newScale = Vector3.Lerp(Vector3.zero,originalScale,scaleProgress);
            _willOTheWisp.transform.localScale = newScale;
            float brightenProgress = timer / _shrinktime;
            float newIntensity = Mathf.Lerp(0f,originalIntensity,brightenProgress);
            _lightSource.intensity = newIntensity;
            timer += Time.deltaTime;
            yield return null;
        }
        _willOTheWisp.transform.localScale = originalScale;
        _lightSource.intensity = originalIntensity;
        _waypoints.RemoveAt(0);
        _triggerCollider.SetActive(true);
        yield return null;
    }
    private IEnumerator Vanish() {
        Destroy(_triggerCollider);
        Vector3 originalScale = _willOTheWisp.transform.localScale;
        float originalIntensity = _lightSource.intensity;
        float timer = 0f;
        while(timer < _shrinktime) {
            float scaleProgress = timer / _shrinktime;
            Vector3 newScale = Vector3.Lerp(originalScale,Vector3.zero,scaleProgress);
            _willOTheWisp.transform.localScale = newScale;
            float dimProgress = timer / _shrinktime;
            float newIntensity = Mathf.Lerp(originalIntensity,0f,dimProgress);
            _lightSource.intensity = newIntensity;
            timer += Time.deltaTime;
            yield return null;
        }
        _willOTheWisp.transform.localScale = Vector3.zero;
        _lightSource.intensity = 0f;
        Destroy(transform.root.gameObject);
        yield return null;
    }
}