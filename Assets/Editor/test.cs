using UnityEditor;
using UnityEngine;

public class test
{
    static Camera sceneViewCamera;
    static float planeHeight = -5f; // Height of the invisible plane

    private static Vector3 OnSceneGUI(SceneView sceneView) {
        Vector3 rayOrigin = sceneViewCamera.transform.position;
        Vector3 rayDirection = sceneViewCamera.transform.forward;
        float denom = Vector3.Dot(Vector3.up,rayDirection);
        if(Mathf.Approximately(denom,0f)) {
            return Vector3.zero;
        }
        float t = -(Vector3.Dot(Vector3.up,rayOrigin) + planeHeight) / denom;
        Debug.Log(rayOrigin + t * rayDirection);
        return rayOrigin + t * rayDirection;
    }

}