using UnityEditor;
using UnityEngine;

public class Tool_Willo : MonoBehaviour
{
    static float planeHeight = -5f;

    [MenuItem("Custom Tools/Do Something")]
    private static void DoSomething() {
        // Get the scene view camera
        Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;
        if(sceneViewCamera == null) {
            Debug.LogError("Scene view camera not found.");
            return;
        }

        // Get the ray origin (camera position) and direction (camera forward)
        Vector3 rayOrigin = sceneViewCamera.transform.position;
        Vector3 rayDirection = sceneViewCamera.transform.forward;

        // Calculate the collision point with the plane at a specific height
        float denom = Vector3.Dot(Vector3.up,rayDirection);
        if(Mathf.Approximately(denom,0f)) {
            Debug.LogError("Ray is parallel to the plane.");
            return;
        }
        float t = -(Vector3.Dot(Vector3.up,rayOrigin) + planeHeight) / denom;
        Vector3 collisionPoint = rayOrigin + t * rayDirection;
    }
}
