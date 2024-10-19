using System;

using UnityEditor;
using UnityEngine;

namespace Enigmatic.Core 
{
    public static class DrawingUtilities
    {
        public static void DrawTitle(string title, float width, bool drawAddRemoveButton = false,
            float buttonSpace = 0, Action addAction = null, Action removeAction = null)
        {
            GUIStyle titleLable = new GUIStyle(EditorStyles.miniBoldLabel);
            titleLable.fontSize = 12;

            //Title
            EnigmaticGUILayout.BeginHorizontal(EnigmaticStyles.columBackground, EnigmaticGUILayout.Width(width),
                EnigmaticGUILayout.Height(22), EnigmaticGUILayout.Padding(0));
            {
                EnigmaticGUILayout.Space(2);
                EnigmaticGUILayout.Lable(title, titleLable);

                EnigmaticGUILayout.Space(buttonSpace);

                if (drawAddRemoveButton)
                {
                    if (EnigmaticGUILayout.ButtonCentric("-", new Vector2(22, 20), new Vector2(0, -2), EnigmaticStyles.toolbarButton))
                        removeAction?.Invoke();

                    EnigmaticGUILayout.Space(-9);

                    if (EnigmaticGUILayout.ButtonCentric("+", new Vector2(22, 20), new Vector2(0, -2), EnigmaticStyles.toolbarButton))
                        addAction?.Invoke();
                }
            }
            EnigmaticGUILayout.EndHorizontal();
        }
    }
}
