using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Enigmatic.Core;

namespace Enigmatic.Experimental.SearchedWindowUtility
{
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
            Load();
        }

        public void OnDisable()
        {

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
                            STPFile.CompareAll(m_SearchedTreeGrups.ToArray());

                            for (int i = 0; i < m_SearchedTreeGrups.Count; i++)
                                STPFile.Generate(m_SearchedTreeGrups[i], (uint)i);

                            STPFile.Save();
                        }

                        if (GUILayout.Button("Load", GUILayout.Width(80)))
                            Load();

                        EditorGUI.BeginDisabledGroup(m_SelectedSearchedTree == null);
                        {
                            if (EnigmaticGUI.Button("Local View", (position.width / 2) - 80, 7, GUILayout.Width(80)))
                                m_LoaclViewSearchedTree = m_SelectedSearchedTree;
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(m_LoaclViewSearchedTree == null);
                        {
                            if (EnigmaticGUI.Button("Global View", (position.width / 2) + 5, 7, GUILayout.Width(80)))
                                m_LoaclViewSearchedTree = null;
                        }
                        EditorGUI.EndDisabledGroup();

                        if (EnigmaticGUI.Button("Generate", position.width - (88), 7, GUILayout.Width(80)))
                        {
                            SearchedTreeGeneratorWindow window = SearchedTreeGeneratorWindow.Open();
                            window.OnGenerated += GenerateTree;
                        }
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
                                DrawSearchedTreeGrups(m_SearchedTreeGrups, m_SelectedGrup);
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
                                            if (m_SelectedGrup.TryRemove(m_SelectedSearchedTree) == false)
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
                                                DrawSearchedTree(m_Trees, tree, m_SelectedSearchedTree);
                                        }
                                        else
                                        {
                                            DrawSearchedTree(m_Trees, m_LoaclViewSearchedTree,
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

        private void Load()
        {
            m_SearchedTreeGrups.Clear();
            SearchedTreeGrup[] searchedTreeGrups = STPFile.LoadTrees();

            foreach (SearchedTreeGrup grup in searchedTreeGrups)
            {
                RegisterGrup(grup);
                m_SearchedTreeGrups.Add(grup);
            }
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
            foreach (SearchedTree tree in grup.SearchedTrees)
                foreach (SearchedTree childTree in tree.GetAllTree())
                    RegisterTree(childTree);
        }

        private void DrawSearchedTreeGrups(List<SearchedTreeGrup> grups, SearchedTreeGrup selectionGrup)
        {
            Color color = GUI.backgroundColor;

            foreach (SearchedTreeGrup grup in grups)
            {
                EnigmaticGUI.BeginSelectedGrup(grup == selectionGrup);
                {
                    if (GUILayout.Button(grup.Name, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
                        OnSelectedGrup(grup);

                    GUILayout.Space(4);
                }
                EnigmaticGUI.EndSelectedGrup();
            }
        }

        private void DrawSearchedTree(Dictionary<SearchedTree, bool> trees,
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
                            OnChengedOpenSearchedTree(tree, true);
                    }
                    else
                    {
                        if (GUILayout.Button("▼", EditorStyles.toolbarButton, GUILayout.Width(20)))
                            OnChengedOpenSearchedTree(tree, false);
                    }
                }
                EditorGUI.EndDisabledGroup();

                EnigmaticGUI.BeginSelectedGrup(selectedTree == tree);
                {
                    if (GUILayout.Button(tree.Value, EditorStyles.toolbarButton))
                        OnSelectedTree(tree);
                }
                EnigmaticGUI.EndSelectedGrup();

                if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20)))
                    OnAddedTree(tree);

                EditorGUI.BeginDisabledGroup(tree != selectedTree);
                {
                    if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(20)))
                        OnRemovedTree(selectedTree);
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

            if (tree.Level == 0)
                GUILayout.Space(6);
        }

        private void GenerateTree(PathTypeTree pathType, string branchName, string[] childs)
        {
            SearchedTree tree = new SearchedTree(branchName);
            RegisterTree(tree);

            foreach(string child in childs)
            {
                SearchedTree childTree = new SearchedTree(child);
                tree.AddChild(childTree);
                RegisterTree(childTree);
            }

            if (pathType == PathTypeTree.SelectionGrup)
                m_SelectedGrup.Add(tree);
            else
                m_SelectedSearchedTree.AddChild(tree);

            SearchedTree[] trees = tree.GetAllTree();
            
            foreach(SearchedTree t in trees)
                t.UpdateLevel();
        }
    }
}