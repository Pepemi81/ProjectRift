using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Linq;
#endif

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class EditorButtonAttribute : Attribute
{
    public string label;

    public EditorButtonAttribute(string label)
    {
        this.label = label;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class GlobalEditorButtonInjector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<EditorButtonAttribute>() != null);

        if (!methods.Any()) return;

        GUILayout.Space(10);
        GUILayout.Label("Debug Buttons", EditorStyles.boldLabel);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<EditorButtonAttribute>();

            if (GUILayout.Button(attr.label, GUILayout.Height(25)))
            {
                method.Invoke(target, null);
            }
        }
    }
}
#endif