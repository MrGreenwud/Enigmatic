using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Enigmatic.Experemental.SearchedWindowUtility
{
    public static class SearchedTreeUtility
    {
        public static string DeCompileTree(string tree, uint depthLevel)
        {
            string[] tempTree = tree.Split("/");
            return tempTree[depthLevel];
        }

        public static string CompileTree(List<string> values)
        {
            string tree = "";

            for (int i = 1; i < values.Count; i++)
            {
                tree += values[i];

                if (i + 1 != values.Count)
                    tree += "/";
            }

            return tree;
        }

        public static void DrawSelectionTree(string name, string value, SearchedTreeListProvider provider)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label($"{name}:");

                if (GUILayout.Button(value, EditorStyles.popup))
                {
                    SearchWindow.Open(new SearchWindowContext
                        (GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
