using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ScriptFinder : EditorWindow {
    private MonoScript _selectedScript;
    private Vector2 _scrollPosition;
    private List<GameObject> _foundObjects = new List<GameObject>();

    [MenuItem("Tools/Script Finder")]
    public static void ShowWindow() {
        GetWindow<ScriptFinder>("Script Finder");
    }

    private void OnGUI() {
        EditorGUILayout.LabelField("Script Finder", EditorStyles.boldLabel);

        // Field to select a script
        _selectedScript = (MonoScript)EditorGUILayout.ObjectField("Script", _selectedScript, typeof(MonoScript), false);

        if (GUILayout.Button("Find GameObjects")) {
            FindGameObjectsWithScript();
        }

        EditorGUILayout.Space();

        if (_foundObjects.Count > 0) {
            EditorGUILayout.LabelField($"Found {_foundObjects.Count} GameObject(s):", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (var obj in _foundObjects) {
                if (GUILayout.Button(obj.name)) {
                    Selection.activeGameObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void FindGameObjectsWithScript() {
        _foundObjects.Clear();

        if (_selectedScript == null) {
            Debug.LogWarning("No script selected.");
            return;
        }

        System.Type scriptType = _selectedScript.GetClass();

        if (scriptType == null) {
            Debug.LogWarning("Selected script does not represent a valid class.");
            return;
        }

        foreach (GameObject obj in FindObjectsOfType<GameObject>()) {
            Component component = obj.GetComponent(scriptType);
            if (component != null) {
                _foundObjects.Add(obj);
            }
        }

        if (_foundObjects.Count == 0) {
            Debug.Log("No GameObjects found with the selected script.");
        }
    }
}
