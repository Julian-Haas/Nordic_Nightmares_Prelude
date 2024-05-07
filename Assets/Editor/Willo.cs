using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Willo : EditorWindow
{
    [SerializeField] private GameObject willOTheWispPrefab;

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

}
