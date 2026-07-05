using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoreSequenceManager))]
public class LoreSequenceManagerEditor : Editor
{
    private SerializedProperty _screenController;
    private SerializedProperty _testData;
    private SerializedProperty _testSequence;

    private void OnEnable()
    {
        _screenController = serializedObject.FindProperty("_screenController");
        _testData = serializedObject.FindProperty("_testData");
        _testSequence = serializedObject.FindProperty("_testSequence");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_screenController);

        EditorGUILayout.Space();
        GUILayout.Label("Testing", EditorStyles.boldLabel);

        bool hasData = _testData.objectReferenceValue != null;
        bool hasSequence = _testSequence.objectReferenceValue != null;

        if (!hasSequence)
        {
            EditorGUILayout.PropertyField(_testData);
        }

        if (!hasData)
        {
            EditorGUILayout.PropertyField(_testSequence);
        }

        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying && (hasData || hasSequence))
        {
            EditorGUILayout.Space();
            GUILayout.Label("Debug Buttons", EditorStyles.boldLabel);

            LoreSequenceManager manager = (LoreSequenceManager)target;

            if (hasData && GUILayout.Button("Test Single Data", GUILayout.Height(25)))
            {
                manager.TestSingleData();
            }

            if (hasSequence && GUILayout.Button("Test Sequence", GUILayout.Height(25)))
            {
                manager.TestSequence();
            }
        }
        else if (!Application.isPlaying && (hasData || hasSequence))
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Entra en PlayMode para habilitar los botones de testeo.", MessageType.Info);
        }
    }
}