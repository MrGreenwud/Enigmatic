using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using EngineUtitlity.SearchedWindow;

public static class InputEditorUtils
{
    /// <summary>
    /// Возвращает значения древо поиска без первого предка.
    /// </summary>
    public static string GetSearchedTree(SearchedTree tree)
    {
        string value = "";

        string[] pathToSelection = tree.ToString().Split("/");

        for (int i = 1; i < pathToSelection.Length; i++)
        {
            value += pathToSelection[i];

            if (i != pathToSelection.Length - 1)
                value += "/";
        }

        return value;
    }

    /// <summary>
    /// Рисует кнопу выбора клавиши с окном поиска
    /// </summary>
    public static void DrowButton(string buttonName, Action<SearchedTree, string> action,
        string buttonValue, string device, string senderCode)
    {
        if (device == "Keyboard")
            DrowSelectableElement(buttonName, "Keyboard", senderCode, action, buttonValue);
        else if (device == "Mouse")
            DrowSelectableElement(buttonName, "MouseButton", senderCode, action, buttonValue);
        else if (device == "Jostick")
            DrowSelectableElement(buttonName, "JostickButton", senderCode, action, buttonValue);
    }

    /// <summary>
    /// Рисует axis, его параметры
    /// </summary>
    public static void DrowAxis(string name, Axis axis, string device,
        Action<SearchedTree, string> action, string senderCode, ref bool isOpen)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (isOpen == false)
                {
                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        isOpen = true;
                }
                else
                {
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        isOpen = false;
                }

                GUILayout.Label(name);
            }
            EditorGUILayout.EndHorizontal();

            if (isOpen == true)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    if (device == "Keyboard")
                    {
                        DrowButton("PosetiveButton", action, axis.PosetiveButton, device, $"{senderCode}Posetive");
                        DrowButton("NegativeButton", action, axis.NegativeButton, device, $"{senderCode}Negative");
                    }
                    else if (device == "Mouse" || device == "Jostick")
                    {
                        if (device == "Mouse")
                            DrowSelectableElement("Value", "MouseValue", senderCode, action, axis.Value);
                        else
                            DrowSelectableElement("Value", "JostickValue", senderCode, action, axis.Value);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void DrowSelectableElement(string name, string searchedTreeTag, string senderCode,
        Action<SearchedTree, string> action, string value)
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label($"{name}:");

            SearchedTreeListProvider provider = ScriptableObject.CreateInstance<SearchedTreeListProvider>();

            Debug.LogWarning(provider);

            provider.Create(searchedTreeTag, senderCode);
            provider.OnSelected += action;

            if (GUILayout.Button(value, EditorStyles.popup))
            {
                SearchWindow.Open(new SearchWindowContext
                    (GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}