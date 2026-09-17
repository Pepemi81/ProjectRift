#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StoryManager))]
public class StoryManagerCE : Editor
{
    private bool _showDebug;

    private SerializedProperty _script;
    private SerializedProperty _playMode;
    private SerializedProperty _playOnStart;
    private SerializedProperty _campaign;
    private SerializedProperty _debugChapter;
    private SerializedProperty _timelineDirector;
    private SerializedProperty _logProgress;
    private SerializedProperty _currentChapter;
    private SerializedProperty _currentChapterIndex;
    private SerializedProperty _currentStepIndex;
    private SerializedProperty _waitingEventReceived;
    private SerializedProperty _activeWaitEvent;
    private SerializedProperty _isPlaying;

    private bool IsStoryMode => (StoryManagerMode)_playMode.enumValueIndex == StoryManagerMode.Story;

    private void OnEnable()
    {
        _script = serializedObject.FindProperty("m_Script");
        _playMode = serializedObject.FindProperty("_playMode");
        _playOnStart = serializedObject.FindProperty("_playOnStart");
        _campaign = serializedObject.FindProperty("_campaign");
        _debugChapter = serializedObject.FindProperty("_debugChapter");
        _timelineDirector = serializedObject.FindProperty("_timelineDirector");
        _logProgress = serializedObject.FindProperty("_logProgress");
        _currentChapter = serializedObject.FindProperty("_currentChapter");
        _currentChapterIndex = serializedObject.FindProperty("_currentChapterIndex");
        _currentStepIndex = serializedObject.FindProperty("_currentStepIndex");
        _waitingEventReceived = serializedObject.FindProperty("_waitingEventReceived");
        _activeWaitEvent = serializedObject.FindProperty("_activeWaitEvent");
        _isPlaying = serializedObject.FindProperty("_isPlaying");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawScriptField();
        DrawModeFields();
        DrawSelectedModeFields();
        DrawTimelineFields();
        DrawDebugSettings();
        DrawRuntimeDebugFields();

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

    private void DrawModeFields()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_playMode);
        EditorGUILayout.PropertyField(_playOnStart);
    }

    private void DrawSelectedModeFields()
    {
        EditorGUILayout.Space();

        if (IsStoryMode)
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

    private void DrawTimelineFields()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_timelineDirector);
    }

    private void DrawDebugSettings()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_logProgress);
    }

    private void DrawRuntimeDebugFields()
    {
        _showDebug = EditorGUILayout.Toggle("Show Debug", _showDebug);

        if (!_showDebug) return;

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(_currentChapter);
            EditorGUILayout.PropertyField(_currentChapterIndex);
            EditorGUILayout.PropertyField(_currentStepIndex);
            EditorGUILayout.PropertyField(_waitingEventReceived);
            EditorGUILayout.PropertyField(_activeWaitEvent);
            EditorGUILayout.PropertyField(_isPlaying);
        }
    }
}
#endif
