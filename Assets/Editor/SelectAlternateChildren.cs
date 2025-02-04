using UnityEngine;
using UnityEditor;

public class SelectAlternateChildren : EditorWindow
{
    private GameObject parentObject;
    private bool startFromFirstChild = true;

    [MenuItem("Tools/Select Alternate Children")]
    public static void ShowWindow()
    {
        GetWindow<SelectAlternateChildren>("Select Alternate Children");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Alternate Children", EditorStyles.boldLabel);

        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        GUILayout.Label("Selection Options", EditorStyles.boldLabel);
        startFromFirstChild = EditorGUILayout.Toggle("Start From First Child", startFromFirstChild);

        if (GUILayout.Button("Select Alternate Children"))
        {
            SelectAlternates();
        }
    }

    private void SelectAlternates()
    {
        if (parentObject == null)
        {
            Debug.LogWarning("No parent object selected!");
            return;
        }

        Transform parentTransform = parentObject.transform;
        int childCount = parentTransform.childCount;

        if (childCount == 0)
        {
            Debug.LogWarning("The selected GameObject has no children.");
            return;
        }

        // Calculate the starting index based on the user's choice
        int startIndex = startFromFirstChild ? 0 : 1;

        // Create a list to store alternate children
        var selectedTransforms = new System.Collections.Generic.List<Transform>();

        for (int i = startIndex; i < childCount; i += 2)
        {
            selectedTransforms.Add(parentTransform.GetChild(i));
        }

        // Set the selection in the Unity Editor
        Selection.objects = selectedTransforms.ConvertAll(t => t.gameObject).ToArray();
        Debug.Log($"Selected {selectedTransforms.Count} alternate children.");
    }
}
