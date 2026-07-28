using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonitorSequenceDirector))]
public class MonitorSequenceDirectorCE : Editor
{
    private SerializedProperty _testSlide;
    private SerializedProperty _testSequence;

    public override void OnInspectorGUI()
    {
        if (target == null) return;

        serializedObject.Update();

        _testSlide = serializedObject.FindProperty("_testSlide");
        _testSequence = serializedObject.FindProperty("_testSequence");

        EditorGUILayout.Space();
        GUILayout.Label("Testing", EditorStyles.boldLabel);

        bool hasData = _testSlide != null && _testSlide.objectReferenceValue != null;
        bool hasSequence = _testSequence != null && _testSequence.objectReferenceValue != null;

        if (!hasSequence && _testSlide != null)
        {
            EditorGUILayout.PropertyField(_testSlide);
        }

        if (!hasData && _testSequence != null)
        {
            EditorGUILayout.PropertyField(_testSequence);
        }

        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying && (hasData || hasSequence))
        {
            EditorGUILayout.Space();
            GUILayout.Label("Debug Buttons", EditorStyles.boldLabel);

            MonitorSequenceDirector director = (MonitorSequenceDirector)target;

            if (hasData && GUILayout.Button("Test Single Slide", GUILayout.Height(25)))
            {
                director.TestSingleSlide();
            }

            if (hasSequence && GUILayout.Button("Test Sequence", GUILayout.Height(25)))
            {
                director.TestSequence();
            }
        }
        else if (!Application.isPlaying && (hasData || hasSequence))
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Entra en PlayMode para habilitar los botones de testeo.", MessageType.Info);
        }
    }
}