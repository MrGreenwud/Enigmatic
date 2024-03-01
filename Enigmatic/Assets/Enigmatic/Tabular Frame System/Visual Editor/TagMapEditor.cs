using System;
using UnityEditor;
using UnityEngine;

namespace TabularFrameSystem
{
    [CustomEditor(typeof(TagMap))]
    public class TagMapEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI();

            TagMap map = (TagMap)target;

            CreateButton("Generate Tags", map.GenerateTag);
        }

        private void CreateButton(string text, Action action)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(text))
                action();

            EditorGUILayout.EndHorizontal();
        }
    }
}
