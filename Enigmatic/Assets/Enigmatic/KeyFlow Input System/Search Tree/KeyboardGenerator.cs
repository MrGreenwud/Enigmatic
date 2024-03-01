using UnityEditor;
using UnityEngine;
using EngineUtitlity;
using EngineUtitlity.SearchedWindow;

namespace KFInputSystem.Utility
{
    [CreateAssetMenu(fileName = "InputTypeGenerator", menuName = "Search Utility/TreeGenerator/Custom/KFInputSysyem/Keyboard", order = 0)]
    public class KeyboardGenerator : TreeGenerator
    {
        public override void Generate()
        {
            base.Generate();

            AddTreeChilds(EnumToStringArray<KeyboardKeyCode>(), searchedTreeProvider.SearchedTree);
        }
    }

    [CustomEditor(typeof(KeyboardGenerator))]
    public class KeyboardGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InspectorEditor.CreateButton("Generate", ((TreeGenerator)target).Generate);
        }
    }
}
