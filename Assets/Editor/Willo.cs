using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Willo : EditorWindow
{
    [SerializeField] private GameObject willOTheWispPrefab;
    static float planeHeight = -5.8f;

    [MenuItem("Window/WillOTheWisp Gamedesigner Tool")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(Willo));
    }

    private void OnGUI() {
        GUILayout.Label("WillOTheWisp Gamedesigner Tool",EditorStyles.boldLabel);
        if(GUILayout.Button("Spawn new WillOTheWisp")) {
            if(willOTheWispPrefab != null) {
                GameObject newWilloInstance = PrefabUtility.InstantiatePrefab(willOTheWispPrefab) as GameObject;
                if(newWilloInstance != null) {
                    Debug.Log("New WillOTheWisp spawned!");
                    newWilloInstance.transform.position = Position();
                    newWilloInstance.transform.GetChild(1).position += Vector3.back;
                    Selection.activeGameObject = newWilloInstance.transform.GetChild(1).gameObject;
                }
                else {
                    Debug.LogError("Failed to spawn WillOTheWisp!");
                }
            }
            else {
                Debug.LogError("Please assign the WillOTheWisp prefab!");
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("Drag and drop the WillOTheWisp prefab here:");
        willOTheWispPrefab = EditorGUILayout.ObjectField(willOTheWispPrefab,typeof(GameObject),false) as GameObject;
    }

    private Vector3 Position() {
        Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;
        if(sceneViewCamera == null) {
            Debug.LogError("Scene view camera not found.");
            return Vector3.zero;
        }
        Vector3 rayOrigin = sceneViewCamera.transform.position;
        Vector3 rayDirection = sceneViewCamera.transform.forward;
        float denom = Vector3.Dot(Vector3.up,rayDirection);
        if(Mathf.Approximately(denom,0f)) {
            Debug.LogError("Ray is parallel to the plane.");
            return Vector3.zero;
        }
        float t = -(Vector3.Dot(Vector3.up,rayOrigin) + planeHeight) / denom;
        Vector3 collisionPoint = rayOrigin + t * rayDirection;
        Debug.Log("Collision point: " + collisionPoint);
        return collisionPoint;
    }
}
