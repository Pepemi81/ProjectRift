using UnityEditor;

[CustomEditor(typeof(MonitorSlideData))]
[CanEditMultipleObjects]
public class MonitorSlideDataCE : Editor
{
    private SerializedProperty _script;
    private SerializedProperty _displayMode;
    private SerializedProperty _screenPrefab;
    private SerializedProperty _displayImage;
    private SerializedProperty _displayText;
    private SerializedProperty _autoAdvanceDelay;
    private SerializedProperty _audioSettings;

    private void OnEnable()
    {
        _script = serializedObject.FindProperty("m_Script");
        _displayMode = serializedObject.FindProperty("_displayMode");
        _screenPrefab = serializedObject.FindProperty("_slidePrefab");
        _displayImage = serializedObject.FindProperty("_displayImage");
        _displayText = serializedObject.FindProperty("_displayText");
        _autoAdvanceDelay = serializedObject.FindProperty("_autoAdvanceDelay");
        _audioSettings = serializedObject.FindProperty("_audioSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (_script != null)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_script);
            }
        }

        EditorGUILayout.PropertyField(_displayMode);
        EditorGUILayout.Space();

        if (_displayMode.hasMultipleDifferentValues)
        {
            EditorGUILayout.PropertyField(_displayImage);
            EditorGUILayout.PropertyField(_displayText);
            EditorGUILayout.PropertyField(_screenPrefab);
        }
        else
        {
            ScreenDisplayMode mode = (ScreenDisplayMode)_displayMode.enumValueIndex;

            if (mode == ScreenDisplayMode.DefaultUI)
            {
                EditorGUILayout.PropertyField(_displayImage);
                EditorGUILayout.PropertyField(_displayText);
            }
            else if (mode == ScreenDisplayMode.CustomPrefab)
            {
                EditorGUILayout.PropertyField(_screenPrefab);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_autoAdvanceDelay);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_audioSettings, true);

        serializedObject.ApplyModifiedProperties();
    }
}
