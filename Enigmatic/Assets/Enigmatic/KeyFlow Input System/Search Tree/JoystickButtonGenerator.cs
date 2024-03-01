using UnityEditor;
using UnityEngine;
using EngineUtitlity;
using EngineUtitlity.SearchedWindow;

namespace KFInputSystem.Utility
{
    [CreateAssetMenu(fileName = "JoystickButtonGenerator", menuName = "Search Utility/TreeGenerator/Custom/KFInputSysyem/JoystickButton", order = 2)]
    public class JoystickButtonGenerator : TreeGenerator
    {
        public override void Generate()
        {
            base.Generate();

            AddTreeChilds(EnumToStringArray<JoystickKeyCode>(), searchedTreeProvider.SearchedTree);
        }
    }

    [CustomEditor(typeof(JoystickButtonGenerator))]
    public class JoystickButtonGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InspectorEditor.CreateButton("Generate", ((TreeGenerator)target).Generate);
        }
    }
}
