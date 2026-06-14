using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameEvent gameEvent = (GameEvent)target;

        GUILayout.Space(15);

        GUI.enabled = Application.isPlaying;

        string buttonText = $"Invocar {gameEvent.name}";

        if (GUILayout.Button(buttonText, GUILayout.Height(35)))
        {
            gameEvent.Invoke();
        }

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Para testear el evento, Unity debe estar en Play Mode.", MessageType.Info);
        }

        GUI.enabled = true;
    }
}