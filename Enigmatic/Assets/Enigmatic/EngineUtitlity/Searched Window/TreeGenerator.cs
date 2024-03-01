using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using KFInputSystem.Utility;

namespace EngineUtitlity.SearchedWindow
{
    [CreateAssetMenu(fileName = "TreeGenerator", menuName = "Search Utility/TreeGenerator/Defauld", order = 0)]
    public class TreeGenerator : ScriptableObject
    {
        [SerializeField] protected SearchedTreeProvider searchedTreeProvider;

        public virtual void Generate() { }

        protected void AddTreeChild(string value, SearchedTree tree)
        {
            tree.AddCild(new SearchedTree(value, tree.Level + 1));
        }

        protected void AddTreeChilds(string[] values, SearchedTree tree)
        {
            foreach (var value in values)
                AddTreeChild(value, tree);
        }

        public static string[] EnumToStringArray<T>() where T : Enum
        {
            T[] enums = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            string[] values = new string[enums.Length];

            for (int i = 0; i < enums.Length; i++)
                values[i] = enums[i].ToString();

            return values;
        }
    }

    [CustomEditor(typeof(TreeGenerator))]
    public class TreeGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InspectorEditor.CreateButton("Generate", ((TreeGenerator)target).Generate);
        }
    }
}