using UnityEditor;
using UnityEngine;
using KinematicCharacterController.Examples; // Importa il namespace corretto

[CustomEditor(typeof(CheckInputManager))]
public class CheckInputManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CheckInputManager manager = (CheckInputManager)target;
        GUILayout.Space(10);
        GUILayout.Label("Current Input State", EditorStyles.boldLabel);
        GUILayout.Label(manager.GetCurrentInputState().ToString(), EditorStyles.wordWrappedLabel);
    }
}
