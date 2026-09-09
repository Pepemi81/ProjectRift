#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StoryManager))]
public class StoryManagerCE : Editor
{
    private SerializedProperty _script;
    private SerializedProperty _playMode;
    private SerializedProperty _playOnStart;
    private SerializedProperty _campaign;
    private SerializedProperty _debugChapter;
    private SerializedProperty _timelineDirector;
    private SerializedProperty _logProgress;

    private void OnEnable()
    {
        _script = serializedObject.FindProperty("m_Script");
        _playMode = serializedObject.FindProperty("_playMode");
        _playOnStart = serializedObject.FindProperty("_playOnStart");
        _campaign = serializedObject.FindProperty("_campaign");
        _debugChapter = serializedObject.FindProperty("_debugChapter");
        _timelineDirector = serializedObject.FindProperty("_timelineDirector");
        _logProgress = serializedObject.FindProperty("_logProgress");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawScriptField();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_playMode);
        EditorGUILayout.PropertyField(_playOnStart);

        EditorGUILayout.Space();
        DrawSelectedModeFields();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_timelineDirector);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_logProgress);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawScriptField()
    {
        if (_script == null)
        {
            return;
        }

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(_script);
        }
    }

    private void DrawSelectedModeFields()
    {
        StoryManagerMode selectedMode = (StoryManagerMode)_playMode.enumValueIndex;

        if (selectedMode == StoryManagerMode.Story)
        {
            GUILayout.Label("Story", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_campaign);
        }
        else
        {
            GUILayout.Label("Chapter Debug", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_debugChapter);
        }
    }
}
#endif
