using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

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

        if(GUILayout.Button("Add Waypoint to selected WillOTheWisp")) {
            AddWaypoint();
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

    private void AddWaypoint() {
        Transform rootTransform = Selection.activeGameObject.transform.root;
        Selection.activeGameObject = rootTransform.GetChild(1).gameObject;
        GameObject selectedObject = Selection.activeGameObject;
        if(selectedObject == null) {
            Debug.LogWarning("Please select a WillOTheWisp first.");
            return;
        }
        int highestIndex = 0;
        int number;
        for(int i = 0; i < rootTransform.childCount; i++) {
            Transform child = rootTransform.GetChild(i);
            Match match = Regex.Match(child.name,@"\d$");
            if(match.Success) {
                string numberString = match.Value;
                int.TryParse(numberString,out number);
                if(number > highestIndex) {
                    highestIndex = number;
                }
            }
        }
        GameObject duplicatedObject = Instantiate(selectedObject);
        highestIndex++;
        duplicatedObject.name = selectedObject.name.Replace("_1","_" + highestIndex);
        duplicatedObject.transform.SetParent(rootTransform);
        Selection.activeGameObject = duplicatedObject;
        Debug.Log("Object duplicated: " + duplicatedObject.name);
    }


}
