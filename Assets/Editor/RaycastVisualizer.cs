using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class RaycastVisualizerEditor
{
    static Camera sceneViewCamera;
    static float planeHeight = -5f; // Height of the invisible plane

    static RaycastVisualizerEditor() {
        // Subscribe to the Scene view update event
        SceneView.duringSceneGui += OnSceneGUI;

        // Get the current Scene view camera
        SceneView.onSceneGUIDelegate += (sceneView) => {
            sceneViewCamera = sceneView.camera;
        };
    }

    private static void OnSceneGUI(SceneView sceneView) {
        if(sceneViewCamera == null)
            return;

        // Calculate collision point between camera forward vector and plane at specific height
        Vector3 collisionPoint = CalculateCollisionPoint(Vector3.up,planeHeight,sceneViewCamera);

        // Visualize the collision point with a red circle
        Handles.color = Color.red;
        Handles.DrawWireDisc(collisionPoint,Vector3.up,0.5f);
    }

    private static Vector3 CalculateCollisionPoint(Vector3 planeNormal,float planeDistance,Camera camera) {
        // Get the ray origin (camera position) and direction (camera forward)
        Vector3 rayOrigin = camera.transform.position;
        Vector3 rayDirection = camera.transform.forward;

        float denom = Vector3.Dot(planeNormal,rayDirection);

        // Check if the ray is parallel to the plane
        if(Mathf.Approximately(denom,0f)) {
            // Ray is parallel to the plane, return Vector3.zero or handle accordingly
            return Vector3.zero;
        }

        float t = -(Vector3.Dot(planeNormal,rayOrigin) + planeDistance) / denom;
        return rayOrigin + t * rayDirection;
    }
}




//using UnityEditor;
//using UnityEngine;

//[InitializeOnLoad]
//public class RaycastVisualizerEditor : MonoBehaviour
//{
//    static Camera sceneViewCamera;

//    static RaycastVisualizerEditor() {
//        // Subscribe to the Scene view update event
//        SceneView.duringSceneGui += OnSceneGUI;

//        // Get the current Scene view camera
//        SceneView.onSceneGUIDelegate += (sceneView) => {
//            sceneViewCamera = SceneView.lastActiveSceneView.camera;
//        };
//    }

//    private static void OnSceneGUI(SceneView sceneView) {
//        if(sceneViewCamera == null)
//            return;

//        // Get the camera transform
//        Transform cameraTransform = sceneViewCamera.transform;

//        // Calculate ray origin and direction
//        Vector3 rayOrigin = cameraTransform.position;
//        Vector3 rayDirection = cameraTransform.forward;



//        //// Cast a ray from the Scene view camera
//        //RaycastHit hitInfo;
//        //bool hit = Physics.Raycast(rayOrigin,rayDirection,out hitInfo);

//        //// Visualize the ray hit point with a red circle
//        //if(hit) {
//        //    Handles.color = Color.red;
//        //    Handles.DrawWireDisc(hitInfo.point,hitInfo.normal,0.5f);
//        //}
//    }
//}
