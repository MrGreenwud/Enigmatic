using UnityEngine;
using UnityEditor;

namespace Enigmatic.Experimental.CustomHierarchy
{
    internal static class CustomMenuItem
    {
        [MenuItem("GameObject/Create Folder", false, 0)]
        public static void CreateFolder()
        {
            GameObject folder = new GameObject("New Folder");
            HirarchyFolder hirarchyFolder = folder.AddComponent<HirarchyFolder>();
            hirarchyFolder.SetRandomColor();

            Undo.RegisterCreatedObjectUndo(folder, "Created Folder");
        }

        [MenuItem("GameObject/Create Splitter", false, 0)]
        public static void CreateSplitter()
        {
            GameObject splitter = new GameObject("Splitter");
            HirarchySpliter hirarchySpliter = splitter.AddComponent<HirarchySpliter>();
            hirarchySpliter.SetRandomColor();

            Undo.RegisterCreatedObjectUndo(splitter, "Created Splitter");
        }
    }
}
