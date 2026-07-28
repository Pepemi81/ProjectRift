using UnityEditor;

[CustomEditor(typeof(MonitorSlideData))]
public class MonitorSlideDataCE : Editor
{
    private SerializedProperty _displayMode;
    private SerializedProperty _screenPrefab;
    private SerializedProperty _displayImage;
    private SerializedProperty _displayText;
    private SerializedProperty _autoAdvanceDelay;

    private void OnEnable()
    {
        _displayMode = serializedObject.FindProperty("_displayMode");
        _screenPrefab = serializedObject.FindProperty("_slidePrefab");
        _displayImage = serializedObject.FindProperty("_displayImage");
        _displayText = serializedObject.FindProperty("_displayText");
        _autoAdvanceDelay = serializedObject.FindProperty("_autoAdvanceDelay");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_displayMode);
        EditorGUILayout.Space();

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

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_autoAdvanceDelay);

        serializedObject.ApplyModifiedProperties();
    }
}