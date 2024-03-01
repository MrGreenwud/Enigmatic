using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EngineUtitlity.SearchedWindow
{
    [Serializable]
    public class SearchedTree
    {
        [SerializeField] private string m_Value;
        [SerializeField] private int m_Level;

        [SerializeField] private List<SearchedTree> m_SearchedTreeElementChilds;

        public SearchedTree Parent { get; private set; }

        public string Value => m_Value;
        public int Level => m_Level;

        public SearchedTree[] SearchedTrees => m_SearchedTreeElementChilds.ToArray();

        public SearchedTree(string value, int level)
        {
            m_Value = value;
            m_Level = level;
            m_SearchedTreeElementChilds = new List<SearchedTree>();
        }

        public static string GetAncestorTree(SearchedTree tree)
        {
            string ancestorTree = "";

            List<SearchedTree> trees = tree.GetAncestorTree();

            for (int i = trees.Count - 1; i >= 0; i--)
            {
                ancestorTree += $"{trees[i].Value}";

                if (i != 0)
                    ancestorTree += "/";
            }

            ancestorTree = ancestorTree.Replace("\n", string.Empty);

            return ancestorTree;
        }

        public void AddCild(SearchedTree newCild)
        {
            m_SearchedTreeElementChilds.Add(newCild);
            SetParent();
        }

        public void SetParent(SearchedTree parent)
        {
            if (parent == null)
                return;

            if (Parent != null)
                return;

            Parent = parent;
        }

        public List<SearchTreeEntry> GetSearchTreeEntry()
        {
            List<SearchTreeEntry> tempSearchTreeEntry = new List<SearchTreeEntry>();

            if (m_SearchedTreeElementChilds.Count == 0)
            {
                SearchTreeEntry searchTreeEntry = new SearchTreeEntry(new GUIContent(Value));
                searchTreeEntry.level = m_Level;
                searchTreeEntry.userData = this;
                tempSearchTreeEntry.Add(searchTreeEntry);

                return tempSearchTreeEntry;
            }

            tempSearchTreeEntry.Add(new SearchTreeGroupEntry(new GUIContent(Value), m_Level));

            foreach (SearchedTree searchedTreeElement in m_SearchedTreeElementChilds)
            {
                List<SearchTreeEntry> cildeTempTreeEntry = searchedTreeElement.GetSearchTreeEntry();

                foreach (SearchTreeEntry searchTreeEntry in cildeTempTreeEntry)
                    tempSearchTreeEntry.Add(searchTreeEntry);
            }

            return tempSearchTreeEntry;
        }

        public List<SearchedTree> GetAncestorTree()
        {
            List<SearchedTree> ancestorTree = new List<SearchedTree>();

            ancestorTree.Add(this);

            if (Parent != null)
            {
                foreach (SearchedTree ancestorTreeParent in Parent.GetAncestorTree())
                    ancestorTree.Add(ancestorTreeParent);
            }

            return ancestorTree;
        }

        public static string[] GetAncestor(string ancestor) => ancestor.Split("/");

        private void SetParent()
        {
            foreach (SearchedTree searchedTree in m_SearchedTreeElementChilds)
                searchedTree.SetParent(this);
        }

        public override string ToString()
        {
            return GetAncestorTree(this);
        }
    }
}
