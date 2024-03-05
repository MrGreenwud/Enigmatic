using UnityEditor;
using EngineUtitlity;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Enigmatic.Experemental.SearchedWindowUtility;

namespace KFInputSystem.Utility
{
    [CustomEditor(typeof(InputMap))]
    public class InputMapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InputMap map = (InputMap)target;

            InspectorEditor.CreateButton("Generate Tags", map.GenerateTags);
            InspectorEditor.CreateButton("Apply Axis To Unity", map.ApplyAxisToUnity);
            InspectorEditor.CreateButton("Apply All", map.ApplyAll);

            InspectorEditor.CreateButton("Edit", OpenEditor);

            if (GUILayout.Button("Test"))
            {
                SearchedTreeListProvider provider = SearchedTreeListProvider.Create("KFInputSystem", "Input_Type");

                SearchWindow.Open(new SearchWindowContext
                    (GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
            }
        }

        public void OpenEditor()
        {
            InputEditorWindow.Open();
        }
    }
}
