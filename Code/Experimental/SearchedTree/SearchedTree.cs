using System;
using System.Collections.Generic;
namespace Enigmatic.Experimental.SearchedWindowUtility
{
    public class SearchedTree
    {
        public string Value;

        private SearchedTree m_Parent;
        private List<SearchedTree> m_SearchedTrees = new List<SearchedTree>();

        public uint Level { get; private set; }

        public SearchedTree GetParent() => m_Parent;
        public SearchedTree GetChild(uint index) => m_SearchedTrees[(int)index];
        public SearchedTree[] GetСhildren() => m_SearchedTrees.ToArray();

        public SearchedTree(string value)
        {
            Value = value;
        }

        public void AddParent(SearchedTree parent)
        {
            {
                if (m_Parent != null)
                    throw new InvalidOperationException("This search tree has a parent!");

                if (parent == null)
                    throw new ArgumentNullException("The passed parent is null!");
            }

            m_Parent = parent;
            Level = parent.Level + 1;
        }

        public SearchedTree AddChild(SearchedTree newChild)
        {
            {
                if (newChild == null)
                    throw new ArgumentNullException("This child searched tree is null!");

                if (m_SearchedTrees.Contains(newChild))
                    throw new ArgumentException("This searched tree has a this child!");

            }

            m_SearchedTrees.Add(newChild);
            newChild.AddParent(this);

            return newChild;
        }

        public void RemoveChild(SearchedTree child)
        {
            if (m_SearchedTrees.Contains(child) == false)
                throw new InvalidOperationException("This searched tree is not child the serched tree!");

            m_SearchedTrees.Remove(child);
        }

        public void UpdateLevel()
        {
            if (m_Parent == null)
            {
                Level = 0;
                return;
            }

            Level = m_Parent.Level + 1;
        }

        public int GetCount()
        {
            int count = 0;
            count += m_SearchedTrees.Count;

            if (m_Parent != null)
                count += m_Parent.GetCount();

            return count;
        }

        public SearchedTree[] GetAllTree()
        {
            List<SearchedTree> trees = new List<SearchedTree>(GetCount()) { this };

            foreach (SearchedTree tree in m_SearchedTrees)
            {
                if (tree.GetСhildren().Length > 0)
                    trees.AddRange(tree.GetAllTree());
                else
                    trees.Add(tree);
            }

            return trees.ToArray();
        }

        public string GetTree()
        {
            string tree = $"{Value}/";

            if (m_Parent != null)
                tree += m_Parent.GetTree();

            string[] tempTree = tree.Split('/');
            tree = string.Empty;

            for (int i = tempTree.Length - 1; i >= 0; i--)
            {
                tree += tempTree[i];

                if (i != 0)
                    tree += "/";
            }

            return tree;
        }

        public override string ToString() => GetTree();
    }
}