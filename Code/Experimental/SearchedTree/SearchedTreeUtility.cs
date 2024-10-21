using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

using Enigmatic.Core;

namespace Enigmatic.Experimental.SearchedWindowUtility
{
    public static class SearchedTreeUtility
    {
        public static string DeCompileTree(string tree, uint depthLevel)
        {
            string[] tempTree = tree.Split("/");

            if (tempTree.Length < depthLevel)
                throw new System.Exception();

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
        } //Old

        public static void DrawSelectionTree(string name, string value, float width, SearchedTreeListProvider provider)
        {
            EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.ExpandHeight(true),
                EnigmaticGUILayout.ElementSpacing(0), EnigmaticGUILayout.Padding(0));
            {
                EnigmaticGUILayout.Lable(name);

                EnigmaticGUILayout.Space(width / 2 - EnigmaticGUILayout.GetLastGUIRect().width);

                if (EnigmaticGUILayout.Button(value, new Vector2(width / 2, 18), EditorStyles.popup))
                {
                    Vector2 position = EnigmaticGUILayout.GetLastGUIRect().position + Vector2.one * 36 + Vector2.right * 85;
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(position)), provider);
                }
            }
            EnigmaticGUILayout.EndHorizontal();
        }
    }
}
