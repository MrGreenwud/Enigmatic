using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Enigmatic.Experimental.SearchedWindowUtility
{
    public enum PathTypeTree
    {
        SelectionGrup,
        SelectionBrench,
    }

    public class SearchedTreeGeneratorWindow : EditorWindow
    {
        private PathTypeTree m_PathType;
        private string m_EnumName = "";

        public event Action<PathTypeTree, string, string[]> OnGenerated;

        public static SearchedTreeGeneratorWindow Open()
        {
            SearchedTreeGeneratorWindow window = GetWindow<SearchedTreeGeneratorWindow>();
            window.titleContent = new GUIContent("Searched Tree Generator");

            Vector2 windowSize = new Vector2(300, 75);

            window.minSize = windowSize;
            window.maxSize = windowSize;

            return window;
        }

        public void OnDisable()
        {
            OnGenerated = null;
        }

        public void OnGUI()
        {
            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(150));
                GUILayout.Label("Path Type");
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                m_PathType = (PathTypeTree)EditorGUILayout.EnumPopup(m_PathType);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);

            m_EnumName = EditorGUILayout.TextField("Enum name", m_EnumName);

            GUILayout.Space(3);

            if (GUILayout.Button("Generate"))
                Generate();
        }

        private void Generate()
        {
            string[] enumElement = Enum.GetNames(ByName(m_EnumName));
            OnGenerated?.Invoke(m_PathType, m_EnumName, enumElement);
        }

        //from -> https://stackoverflow.com/questions/20008503/get-type-by-name
        private static Type ByName(string name)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .Reverse()
                    .Select(assembly => assembly.GetType(name))
                    .FirstOrDefault(t => t != null)
                // Safely delete the following part
                // if you do not want fall back to first partial result
                ??
                AppDomain.CurrentDomain.GetAssemblies()
                    .Reverse()
                    .SelectMany(assembly => assembly.GetTypes())
                    .FirstOrDefault(t => t.Name.Contains(name));
        }
    }
}
