using UnityEditor;
using UnityEngine;
using EngineUtitlity;
using EngineUtitlity.SearchedWindow;

namespace KFInputSystem.Utility
{
    [CreateAssetMenu(fileName = "MouseButtonGenerator", menuName = "Search Utility/TreeGenerator/Custom/KFInputSysyem/MouseButton", order = 1)]
    public class MouseButtonGenerator : TreeGenerator
    {
        public override void Generate()
        {
            base.Generate();

            AddTreeChilds(EnumToStringArray<MouseButtonCode>(), searchedTreeProvider.SearchedTree);
        }
    }

    [CustomEditor(typeof(MouseButtonGenerator))]
    public class MouseButtonGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InspectorEditor.CreateButton("Generate", ((TreeGenerator)target).Generate);
        }
    }
}
