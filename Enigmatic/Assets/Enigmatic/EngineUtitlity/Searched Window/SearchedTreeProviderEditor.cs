using UnityEditor;

namespace EngineUtitlity.SearchedWindow
{
    [CustomEditor(typeof(SearchedTreeProvider))]
    internal class SearchedTreeProviderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SearchedTreeProvider generator = (SearchedTreeProvider)target;
            InspectorEditor.CreateButton("Create Search Tree", generator.CreateSearchTreeEntry);
        }
    }
}
