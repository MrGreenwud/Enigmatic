using System;
using UnityEditor;
using UnityEngine;

namespace EngineUtitlity
{
    public static class InspectorEditor
    {
        public static void CreateButton(string text, Action action)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(text))
                action();

            EditorGUILayout.EndHorizontal();
        }
    }
}
