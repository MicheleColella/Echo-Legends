using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CircleSync))]
public class CircleSyncEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CircleSync script = (CircleSync)target;
        if (GUILayout.Button("Populate Materials List"))
        {
            PopulateMaterials(script);
        }
    }

    private void PopulateMaterials(CircleSync script)
    {
        script.materials.Clear();
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            script.materials.AddRange(renderer.materials);
        }
    }
}
