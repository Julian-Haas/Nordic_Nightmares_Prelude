using UnityEditor;
using UnityEngine;
using System.Collections;

public class NoiseEmitter : MonoBehaviour
{
    [Header("Distance the emitted sounds can be heard according to interactions")]
    [SerializeField] float Quiet = 1.0f;
    [SerializeField] float Medium = 5.0f;
    [SerializeField] float Loud = 15.0f;

    private Color _loud = new Color(1.0f, 0.0f, 0.0f, 0.25f),
                  _medium = new Color(1.0f, 0.5f, 0.0f, 0.25f),
                  _quiet = new Color(1.0f, 1.0f, 0.0f, 0.25f);

    public float _radius = 0.0f;
    private NaddiAwareness _naddiAwareness;
    private UtburdurManager _utburdurManager;

    public void AdjustNoiseParamaters(float quiet, float med, float loud) 
    {
        Quiet = quiet;
        Medium = med;
        Loud = loud;
    }

    public void MakeSound(int noise = 1) { 
    switch(noise)
        {
            case 0:
                _radius = Quiet;
                break;
            case 1:
                _radius = Medium;
                break;
            case 2:
                _radius = Loud;
                break;
            default:
                break;
        }
        WorldStateData.Instance.AlertEnemiesInHearingRange(this);
    }

    public void AlertEnemiesInHearingRange(NoiseEmitter emitter)
    {
        if (_naddiAwareness != null && _utburdurManager != null)
        {
            _naddiAwareness.CheckIfNaddiCanHearNoise(emitter);
            _utburdurManager.CheckIfUtburdurCanHearNoise(emitter);
        }
        else
        {
            _naddiAwareness = GameObject.Find("Naddi").transform.GetChild(0).gameObject.GetComponent<NaddiAwareness>();
            _utburdurManager = GameObject.Find("UtburdurManager").GetComponent<UtburdurManager>();
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Draw Hearing Areas
        Handles.color = _loud;
        Handles.DrawWireDisc(transform.position, Vector3.up, Loud, 3.0f);
        Handles.color = _medium;
        Handles.DrawWireDisc(transform.position, Vector3.up, Medium, 3.0f);
        Handles.color = _quiet;
        Handles.DrawWireDisc(transform.position, Vector3.up, Quiet, 3.0f);
    }
#endif
}