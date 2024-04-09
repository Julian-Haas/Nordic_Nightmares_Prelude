using UnityEditor;
using UnityEngine;

public class DisplayPathInterruption : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        switch (gameObject.tag)
        {
            case "NaddiPause":
                Handles.color = Color.white;
                break;
            case "NaddiPointOfInterest":
                Handles.color = Color.green;
                break;
            case "NaddiConspicousArea":
                Handles.color = Color.red;
                break;
            default:
                break;
        }
        Handles.DrawWireDisc(transform.position, Vector3.up, transform.localScale.x * 0.5f, 10.0f);
    }
#endif
}