using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
            List<SearchedTree> trees = new List<SearchedTree>(32) { this };
            
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

    public class SearchedTreeEditorWindow : EditorWindow
    {
        private List<SearchedTreeGrup> m_SearchedTreeGrups = new List<SearchedTreeGrup>();
        private Dictionary<SearchedTree, bool> m_Trees = new Dictionary<SearchedTree, bool>();

        private SearchedTreeGrup m_SelectedGrup;
        private SearchedTree m_SelectedSearchedTree;
        private SearchedTree m_LoaclViewSearchedTree;

        private Vector2 m_GrupScrollPosition;
        private Vector2 m_SearchTreeScrollPosition;

        private Queue<SearchedTree> m_DestroedSearchedTree = new Queue<SearchedTree>();

        [MenuItem("Tools/Enigmatic/Searched Tree Editor")]
        public static void Open()
        {
            SearchedTreeEditorWindow searchableEditorWindow = GetWindow<SearchedTreeEditorWindow>();
            searchableEditorWindow.titleContent = new GUIContent("Searched Tree Editor");
            searchableEditorWindow.minSize = new Vector2(800, 600);
        }

        public void OnEnable()
        {
            SearchedTreeGUI.SelectedSearchedTreeGrup += OnSelectedGrup;
            SearchedTreeGUI.SelectedSearchedTree += OnSelectedTree;
            SearchedTreeGUI.AddedTree += OnAddedTree;
            SearchedTreeGUI.RemovedTree += OnRemovedTree;
            SearchedTreeGUI.ChengedOpenSearchedTree += OnChengedOpenSearchedTree;
        }

        public void OnDisable()
        {
            SearchedTreeGUI.SelectedSearchedTreeGrup -= OnSelectedGrup;
            SearchedTreeGUI.AddedTree -= OnAddedTree;
            SearchedTreeGUI.RemovedTree -= OnRemovedTree;
            SearchedTreeGUI.ChengedOpenSearchedTree -= OnChengedOpenSearchedTree;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
                    {
                        if (GUILayout.Button("Save", GUILayout.Width(80)))
                        {
                            for (int i = 0; i < m_SearchedTreeGrups.Count; i++)
                                STPFile.Generate(m_SearchedTreeGrups[i], (uint)i);

                            STPFile.Create();
                        }

                        if (GUILayout.Button("Load", GUILayout.Width(80)))
                        {
                            m_SearchedTreeGrups.Clear();
                            SearchedTreeGrup[] searchedTreeGrups = STPFile.LoadTrees();

                            foreach (SearchedTreeGrup grup in searchedTreeGrups)
                            {
                                RegisterGrup(grup);
                                m_SearchedTreeGrups.Add(grup);  
                            }
                        }

                        GUILayout.Space(position.width / 3.5f - 90);

                        EditorGUI.BeginDisabledGroup(m_SelectedSearchedTree == null);
                        {
                            if (GUILayout.Button("Local View", GUILayout.Width(80)))
                                m_LoaclViewSearchedTree = m_SelectedSearchedTree;
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(m_LoaclViewSearchedTree == null);
                        {
                            if (GUILayout.Button("Global View", GUILayout.Width(80)))
                                m_LoaclViewSearchedTree = null;
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(position.width / 4),
                            GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                        {
                            EditorGUILayout.BeginHorizontal("box");
                            {
                                GUILayout.Label("Grups", EditorStyles.boldLabel);

                                if (GUILayout.Button("+", GUILayout.Width(20)))
                                    m_SearchedTreeGrups.Add(new SearchedTreeGrup("New Tree Grup"));

                                EditorGUI.BeginDisabledGroup(m_SelectedGrup == null);
                                {
                                    if (GUILayout.Button("-", GUILayout.Width(20)))
                                    {
                                        m_SearchedTreeGrups.Remove(m_SelectedGrup);
                                        m_SelectedGrup = null;
                                    }
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                            EditorGUILayout.EndHorizontal();

                            m_GrupScrollPosition = EditorGUILayout.BeginScrollView(m_GrupScrollPosition);
                            {
                                SearchedTreeGUI.DrawSearchedTreeGrups(m_SearchedTreeGrups, m_SelectedGrup);
                            }
                            EditorGUILayout.EndScrollView();

                            EditorGUILayout.BeginHorizontal("box");
                            {
                                if (GUILayout.Button("Clear"))
                                    m_SearchedTreeGrups.Clear();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                        {
                            if (m_SelectedGrup != null)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    m_SelectedGrup.Name = EditorGUILayout.TextField("Grup name:", m_SelectedGrup.Name);
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal("box");
                                {
                                    GUILayout.Label("Trees", EditorStyles.boldLabel);

                                    if (GUILayout.Button("Add", GUILayout.Width(60)))
                                    {
                                        SearchedTree newTree = new SearchedTree("New Tree");
                                        m_SelectedGrup.Add(newTree);
                                        m_Trees.Add(newTree, false);
                                    }

                                    EditorGUI.BeginDisabledGroup(m_SelectedSearchedTree == null);
                                    {
                                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                                        {
                                            if(m_SelectedGrup.TryRemove(m_SelectedSearchedTree) == false)
                                                OnRemovedTree(m_SelectedSearchedTree);

                                            m_SelectedSearchedTree = null;
                                        }
                                    }
                                    EditorGUI.EndDisabledGroup();
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                                {
                                    m_SearchTreeScrollPosition = EditorGUILayout.BeginScrollView(m_SearchTreeScrollPosition);
                                    {
                                        if (m_LoaclViewSearchedTree == null)
                                        {
                                            foreach (SearchedTree tree in m_SelectedGrup.SearchedTrees)
                                                SearchedTreeGUI.DrawSearchedTree(m_Trees, tree, m_SelectedSearchedTree);
                                        }
                                        else
                                        {
                                            SearchedTreeGUI.DrawSearchedTree(m_Trees, m_LoaclViewSearchedTree, 
                                                m_SelectedSearchedTree, m_LoaclViewSearchedTree.Level);
                                        }
                                    }
                                    EditorGUILayout.EndScrollView();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(position.width / 4),
                            GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                        {
                            EditorGUILayout.BeginHorizontal("box");
                            {
                                GUILayout.Label("Property");
                            }
                            EditorGUILayout.EndHorizontal();

                            if (m_SelectedSearchedTree != null)
                            {
                                m_SelectedSearchedTree.Value = EditorGUILayout.TextField(m_SelectedSearchedTree.Value);

                                GUILayout.Space(2);

                                if (GUILayout.Button("Add"))
                                    OnAddedTree(m_SelectedSearchedTree);

                                if (GUILayout.Button("Remove"))
                                    OnRemovedTree(m_SelectedSearchedTree);
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            RemoveTrees();
        }

        private void OnSelectedGrup(SearchedTreeGrup grup)
        {
            m_SelectedGrup = grup;
            GUI.FocusControl(null);
        }

        private void OnSelectedTree(SearchedTree tree)
        {
            m_SelectedSearchedTree = tree;
            GUI.FocusControl(null);
        }

        private void OnAddedTree(SearchedTree parentTree)
        {
            SearchedTree newTree = new SearchedTree("New Tree");
            parentTree.AddChild(newTree);

            RegisterTree(newTree);
        }

        private void OnRemovedTree(SearchedTree tree)
        {
            m_DestroedSearchedTree.Enqueue(tree);

            SearchedTree[] children = tree.GetСhildren();

            foreach (SearchedTree child in children)
                OnRemovedTree(child);
        }

        private void RemoveTrees()
        {
            while (m_DestroedSearchedTree.Count > 0)
            {
                SearchedTree tree = m_DestroedSearchedTree.Dequeue();
                SearchedTree parent = tree.GetParent();

                if (parent != null)
                    parent.RemoveChild(tree);

                m_Trees.Remove(tree);
                m_SelectedGrup.Remove(tree);
            }
        }

        private void OnChengedOpenSearchedTree(SearchedTree tree, bool isOpen)
        {
            if (m_Trees.ContainsKey(tree) == false)
            {
                Debug.LogError("This tree no register!");
                return;
            }

            m_Trees[tree] = isOpen;
        }

        private void RegisterTree(SearchedTree tree) => m_Trees.Add(tree, false);

        private void RegisterGrup(SearchedTreeGrup grup)
        {
            foreach(SearchedTree tree in grup.SearchedTrees)
                foreach(SearchedTree childTree in tree.GetAllTree())
                    RegisterTree(childTree);
        }
    }

    public class SearchedTreeGrup
    {
        public string Name;

        private List<SearchedTree> m_SearchedTrees { get; set; }

        public SearchedTree[] SearchedTrees => m_SearchedTrees.ToArray();

        public SearchedTreeGrup(string name)
        {
            Name = name;
            m_SearchedTrees = new List<SearchedTree>();
        }

        public void Add(SearchedTree newSearchedTree) => m_SearchedTrees.Add(newSearchedTree);

        public void Remove(SearchedTree newSearchedTree) => m_SearchedTrees.Remove(newSearchedTree);

        public bool TryRemove(SearchedTree newSearchedTree)
        {
            m_SearchedTrees.Remove(newSearchedTree);
            return m_SearchedTrees.Contains(newSearchedTree);
        }
    }

    public static class SearchedTreeGUI
    {
        public static event Action<SearchedTreeGrup> SelectedSearchedTreeGrup;
        public static event Action<SearchedTree> SelectedSearchedTree;

        public static event Action<SearchedTree> AddedTree;
        public static event Action<SearchedTree> RemovedTree;

        public static event Action<SearchedTree, bool> ChengedOpenSearchedTree;

        public static void DrawSearchedTreeGrups(List<SearchedTreeGrup> grups, SearchedTreeGrup selectionGrup)
        {
            Color color = GUI.backgroundColor;

            foreach (SearchedTreeGrup grup in grups)
            {
                EnigmatiocGUI.BeginSelectedGrup(grup == selectionGrup);
                {
                    if (GUILayout.Button(grup.Name, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
                        SelectedSearchedTreeGrup?.Invoke(grup);

                    GUILayout.Space(4);
                }
                EnigmatiocGUI.EndSelectedGrup();
            }
        }

        public static void DrawSearchedTree(SearchedTree searchedTree)
        {
            if (GUILayout.Button(searchedTree.Value)) { }

            SearchedTree[] childs = searchedTree.GetСhildren();

            if (childs.Length > 0)
                GUILayout.Space(3);

            foreach (SearchedTree tree in childs)
                DrawSearchedTree(tree);
        }

        public static void DrawSearchedTree(Dictionary<SearchedTree, bool> trees,
            SearchedTree tree, SearchedTree selectedTree = null, uint loaclViewTreeLevel = 0)
        {
            SearchedTree[] children = tree.GetСhildren().ToArray();

            EditorGUILayout.BeginHorizontal();
            {
                uint level = tree.Level - loaclViewTreeLevel;

                GUILayout.Space(20 * level);

                EditorGUI.BeginDisabledGroup(children.Length == 0);
                {
                    if (trees[tree] == false)
                    {
                        if (GUILayout.Button("►", EditorStyles.toolbarButton, GUILayout.Width(20)))
                            ChengedOpenSearchedTree?.Invoke(tree, true);
                    }
                    else
                    {
                        if (GUILayout.Button("▼", EditorStyles.toolbarButton, GUILayout.Width(20)))
                            ChengedOpenSearchedTree?.Invoke(tree, false);
                    }
                }
                EditorGUI.EndDisabledGroup();

                EnigmatiocGUI.BeginSelectedGrup(selectedTree == tree);
                {
                    if (GUILayout.Button(tree.Value, EditorStyles.toolbarButton))
                        SelectedSearchedTree?.Invoke(tree);
                }
                EnigmatiocGUI.EndSelectedGrup();

                if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20)))
                    AddedTree?.Invoke(tree);

                EditorGUI.BeginDisabledGroup (tree != selectedTree);
                {
                    if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(20)))
                        RemovedTree?.Invoke(selectedTree);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            if (trees[tree] == true)
            {
                foreach (SearchedTree cildTree in children)
                    DrawSearchedTree(trees, cildTree, selectedTree, loaclViewTreeLevel);
            }

            if(tree.Level == 0)
                GUILayout.Space(6);
        }
    }

    public static class EnigmatiocGUI
    {
        private static Color SelectionColor = new Color(0.6039f, 0.8117f, 1f);

        public static void BeginSelectedGrup(bool selected)
        {
            if (selected)
                GUI.backgroundColor = SelectionColor;
        }

        public static void EndSelectedGrup()
        {
            GUI.backgroundColor = Color.white;
        }
    }
}