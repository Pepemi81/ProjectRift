#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonitorSequenceDirector))]
public class MonitorSequenceDirectorCE : Editor
{
    private bool _showDebug;

    private SerializedProperty _testMode;
    private SerializedProperty _testSlide;
    private SerializedProperty _testSequence;
    private SerializedProperty _currentSequence;
    private SerializedProperty _currentSlide;
    private SerializedProperty _currentSlideNumber;
    private SerializedProperty _totalSlides;
    private SerializedProperty _remainingSlides;
    private SerializedProperty _interactPressed;
    private SerializedProperty _isPlaying;
    private SerializedProperty _script;

    private MonitorSequenceTestMode TestMode => (MonitorSequenceTestMode)_testMode.enumValueIndex;
    private bool HasSelectedTestData => TestMode == MonitorSequenceTestMode.SingleSlide
        ? _testSlide.objectReferenceValue != null
        : _testSequence.objectReferenceValue != null;

    private void OnEnable()
    {
        _script = serializedObject.FindProperty("m_Script");
        _testMode = serializedObject.FindProperty("_testMode");
        _testSlide = serializedObject.FindProperty("_testSlide");
        _testSequence = serializedObject.FindProperty("_testSequence");
        _currentSequence = serializedObject.FindProperty("_currentSequence");
        _currentSlide = serializedObject.FindProperty("_currentSlide");
        _currentSlideNumber = serializedObject.FindProperty("_currentSlideNumber");
        _totalSlides = serializedObject.FindProperty("_totalSlides");
        _remainingSlides = serializedObject.FindProperty("_remainingSlides");
        _interactPressed = serializedObject.FindProperty("_interactPressed");
        _isPlaying = serializedObject.FindProperty("_isPlaying");
    }

    public override void OnInspectorGUI()
    {
        if (target == null) return;

        serializedObject.Update();

        DrawScriptField();
        DrawTestingFields();
        DrawDebugFields();

        serializedObject.ApplyModifiedProperties();

        DrawDebugButtons();
    }

    private void DrawScriptField()
    {
        if (_script != null)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_script);
            }
        }
    }

    private void DrawTestingFields()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Testing", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_testMode);

        if (TestMode == MonitorSequenceTestMode.SingleSlide)
        {
            EditorGUILayout.PropertyField(_testSlide);
            return;
        }

        EditorGUILayout.PropertyField(_testSequence);
    }

    private void DrawDebugFields()
    {
        EditorGUILayout.Space();
        _showDebug = EditorGUILayout.Toggle("Show Debug", _showDebug);

        if (!_showDebug) return;

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(_currentSequence);
            EditorGUILayout.PropertyField(_currentSlide);
            EditorGUILayout.PropertyField(_currentSlideNumber);
            EditorGUILayout.PropertyField(_totalSlides);
            EditorGUILayout.PropertyField(_remainingSlides);
            EditorGUILayout.PropertyField(_interactPressed);
            EditorGUILayout.PropertyField(_isPlaying);
        }
    }

    private void DrawDebugButtons()
    {
        if (!HasSelectedTestData) return;

        EditorGUILayout.Space();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Entra en PlayMode para habilitar los botones de testeo.", MessageType.Info);
            return;
        }

        GUILayout.Label("Debug Buttons", EditorStyles.boldLabel);

        MonitorSequenceDirector director = (MonitorSequenceDirector)target;

        if (TestMode == MonitorSequenceTestMode.SingleSlide)
        {
            if (GUILayout.Button("Test Single Slide", GUILayout.Height(25)))
            {
                director.TestSingleSlide();
            }

            return;
        }

        if (GUILayout.Button("Test Sequence", GUILayout.Height(25)))
        {
            director.TestSequence();
        }
    }
}
#endif
