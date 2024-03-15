using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enigmatic.Experemental.SearchedWindowUtility
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
                    throw new InvalidOperationException("");

                if (parent == null)
                    throw new ArgumentNullException(nameof(parent));
            }

            m_Parent = parent;
            Level = parent.Level + 1;
        }

        public SearchedTree AddChild(SearchedTree newChild)
        {
            {
                if (newChild == null)
                    throw new ArgumentNullException(nameof(newChild));

                if (m_SearchedTrees.Contains(newChild))
                    throw new ArgumentException("");

            }

            m_SearchedTrees.Add(newChild);
            newChild.AddParent(this);

            return newChild;
        }

        public void RemoveChild(SearchedTree child)
        {
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

        public uint Count()
        {
            uint count = 0;
            count += (uint)m_SearchedTrees.Count;

            if (m_Parent != null)
                count += m_Parent.Count();

            return count;
        }

        public SearchedTree[] GetAllTree()
        {
            List<SearchedTree> trees = new List<SearchedTree>(16) { this };
            
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
            tree = "";

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

    public static class EnigmaticGUI
    {
        public readonly static Color SelectionColor = new Color(0.6039f, 0.8117f, 1f);

        public static void BeginSelectedGrup(bool selected)
        {
            if (selected)
                GUI.backgroundColor = SelectionColor;
                //GUI.contentColor = SelectionColor;
        }

        public static void EndSelectedGrup()
        {
            GUI.backgroundColor = Color.white;
        }

        public static bool Button(string text, Vector2 positonPixel, params GUILayoutOption[] gUILayoutOptions)
        {
            bool result;

            GUILayout.BeginArea(new Rect(positonPixel.x, positonPixel.y, 100, 100));
            result = GUILayout.Button(text, gUILayoutOptions);
            GUILayout.EndArea();

            return result;
        }

        public static bool Button(string text, float x, float y, params GUILayoutOption[] gUILayoutOptions)
        {
            return Button(text, new Vector2(x, y), gUILayoutOptions);
        }
    }
}