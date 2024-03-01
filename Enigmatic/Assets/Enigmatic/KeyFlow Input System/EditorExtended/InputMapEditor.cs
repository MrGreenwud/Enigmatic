using UnityEditor;
using EngineUtitlity;

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
        }

        public void OpenEditor()
        {
            InputEditorWindow.Open();
        }
    }
}
