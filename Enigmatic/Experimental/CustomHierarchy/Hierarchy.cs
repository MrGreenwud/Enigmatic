using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Enigmatic.Core;

namespace Enigmatic.Experimental.CustomHierarchy
{
    [InitializeOnLoad]
    internal static class Hierarchy
    {
        public static bool isEnabled = true;
        private static bool sm_IsInit;

        private static int[] sm_ObjectIDs = new int[0];

        private static EditorWindow sm_HierarchyWindow;

        private static object sceneHierarchyInstance;
        private static object treeViewControllerInstance;

        private static MethodInfo GetExpandedIDsMethod;
        private static MethodInfo IsSelectedIDsMethod;
        private static PropertyInfo ShowingVerticalScrollBarProperty;

        private static Dictionary<GameObject, KeyValuePair<Color, GUIStyle>> sm_IsFolders = new Dictionary<GameObject, KeyValuePair<Color, GUIStyle>>();
        private static Dictionary<GameObject, HirarchySpliter> sm_IsSpliters = new Dictionary<GameObject, HirarchySpliter>();

        private static float sm_CurrentViewWidth;

        private static bool ShowingVerticalScrollBar => (bool)ShowingVerticalScrollBarProperty.GetValue(treeViewControllerInstance);
        private static bool IsSelected(int id) => (bool)IsSelectedIDsMethod.Invoke(treeViewControllerInstance, new object[] { id });

        static Hierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            EditorApplication.hierarchyChanged += UpdateHierarchy;
            UpdateHierarchy();

            //isEnabled = false;

            //SceneHierarchy
            //SceneHierarchyWindow
            //SceneHierarchy
            //TreeViewController
            //UnityEditor.IMGUI.Controls.TreeViewDataSource
            //UnityEditor.IMGUI.Controls.TreeViewController
            //UnityEditor.SceneHierarchy
            //UnityEditor.IMGUI.Controls
            //GetExpandedIDs
            //IsExpanded
            //SetExpanded
        }

        private static void Init()
        {
            if (sm_IsInit)
                return;

            UpdateHierarchy();

            var sceneHierarchyType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchy");
            var sceneHierarchyWindowType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var treeViewControllerType = typeof(Editor).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewController");

            var sceneHierarchyWindowInstance = GetHierarchyWindow();
            sm_HierarchyWindow = (EditorWindow)sceneHierarchyWindowInstance;

            var sceneHierarchyField = sceneHierarchyWindowType.GetField("m_SceneHierarchy", BindingFlags.Instance | BindingFlags.NonPublic);
            GetExpandedIDsMethod = sceneHierarchyType.GetMethod("GetExpandedIDs", BindingFlags.Instance | BindingFlags.Public);

            sceneHierarchyInstance = sceneHierarchyField.GetValue(sceneHierarchyWindowInstance);

            var treeViewControllerField = sceneHierarchyType.GetField("m_TreeView", BindingFlags.Instance | BindingFlags.NonPublic);
            treeViewControllerInstance = treeViewControllerField.GetValue(sceneHierarchyInstance);

            ShowingVerticalScrollBarProperty = treeViewControllerType.GetProperty("showingVerticalScrollBar", BindingFlags.Instance | BindingFlags.Public);
            IsSelectedIDsMethod = treeViewControllerType.GetMethod("IsSelected", BindingFlags.Instance | BindingFlags.Public);

            sm_IsInit = true;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            if (isEnabled == false)
                return;

            Init();

            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject == null)
                return;

            if (ShowingVerticalScrollBar)
                sm_CurrentViewWidth = EditorGUIUtility.currentViewWidth - 12;
            else
                sm_CurrentViewWidth = EditorGUIUtility.currentViewWidth;

            sm_ObjectIDs = (int[])GetExpandedIDsMethod.Invoke(sceneHierarchyInstance, null);

            PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);

            bool isPrefabConnected = prefabInstanceStatus == PrefabInstanceStatus.Connected && IsPrefabRoot(gameObject);
            bool isSelected = IsSelected(instanceID); //CheckIsSelected(gameObject);
            bool isSpliter = sm_IsSpliters.ContainsKey(gameObject);
            bool isFolder = sm_IsFolders.ContainsKey(gameObject);

            DrawBackground(gameObject, selectionRect, isSelected);

            FoldOutButton(gameObject, selectionRect, IsHierarchyExpanded(instanceID));
            DrawIcon(gameObject, selectionRect, isSelected, isPrefabConnected);
            DrawObjectName(gameObject, selectionRect, isSelected, isFolder, isSpliter, isPrefabConnected);

            if (isSpliter == false)
            {
                DrawObjectActiveToggle(gameObject, selectionRect);
                DrawLayerSelectionButton(gameObject, selectionRect);
                DrawTagSelectionButton(gameObject, selectionRect);
            }

            if (isPrefabConnected)
                DrawPrefabOpenButton(selectionRect);

            DrawObjectTreeHierarchy(gameObject, selectionRect);
        }

        public static void UpdateFolderIcon(HirarchyFolder folder)
        {
            if (sm_IsFolders.ContainsKey(folder.gameObject))
                sm_IsFolders.Remove(folder.gameObject);

            Texture2D folderTexure = EnigmaticStyles.folderIcon.normal.background;
            GUIStyle folderIcon = EnigmaticStyles.MultiplyColor(folderTexure, folder.Color);

            KeyValuePair<Color, GUIStyle> pair =
                        new KeyValuePair<Color, GUIStyle>(
                            folder.Color, folderIcon);

            sm_IsFolders.Add(folder.gameObject, pair);
        }

        private static void UpdateHierarchy()
        {
            if (isEnabled == false)
                return;

            sm_IsSpliters.Clear();

            Dictionary<GameObject, KeyValuePair<Color, GUIStyle>> folderIcons = new Dictionary<GameObject, KeyValuePair<Color, GUIStyle>>();

            HirarchyFolder[] folders = GameObject.FindObjectsByType<HirarchyFolder>(FindObjectsSortMode.None);
            HirarchySpliter[] splitters = GameObject.FindObjectsByType<HirarchySpliter>(FindObjectsSortMode.None);

            foreach(HirarchyFolder f in folders)
            {
                GameObject gameObject = f.gameObject;

                if (sm_IsFolders.ContainsKey(gameObject))
                {
                    if (sm_IsFolders[gameObject].Key == f.Color)
                    {
                        folderIcons.Add(gameObject, sm_IsFolders[gameObject]);
                        continue;
                    }
                }

                Texture2D folderTexure = EnigmaticStyles.folderIcon.normal.background;
                GUIStyle folder = EnigmaticStyles.MultiplyColor(folderTexure, f.Color);

                KeyValuePair<Color, GUIStyle> pair =
                            new KeyValuePair<Color, GUIStyle>(
                                f.Color, folder);

                folderIcons.Add(gameObject, pair);
            }

            foreach(HirarchySpliter s in splitters)
            {
                sm_IsSpliters.Add(s.gameObject, s);
            }

            sm_IsFolders = folderIcons;
        }

        private static void DrawBackground(GameObject gameObject, Rect selectionRect, bool isSelected)
        {
            if (sm_IsSpliters.ContainsKey(gameObject) == false)
            {
                selectionRect.x = 32;
                selectionRect.width = EditorGUIUtility.currentViewWidth;

                int index = Mathf.FloorToInt(selectionRect.y / selectionRect.height);
                Color backgroundColor = EnigmaticStyles.BackgroundColor;

                if (index % 2 == 0)
                    backgroundColor = EnigmaticStyles.DarkedBackgroundColor;

                EditorGUI.DrawRect(selectionRect, backgroundColor);

                if (isSelected)
                    EditorGUI.DrawRect(selectionRect, EnigmaticStyles.DarkThemeBlueElementSelected);
            }
            else
            {
                selectionRect = new Rect(32, selectionRect.y, EditorGUIUtility.currentViewWidth, selectionRect.height);
                EditorGUI.DrawRect(selectionRect, sm_IsSpliters[gameObject].Color);
            }

            if (selectionRect.Contains(Event.current.mousePosition) && isSelected == false)
                EditorGUI.DrawRect(selectionRect, EnigmaticStyles.HeverColor);
        }

        private static void FoldOutButton(GameObject gameObject, Rect selectionRect, bool isExpanded)
        {
            if (gameObject.transform.childCount == 0)
                return;

            Rect iconRect = new Rect(selectionRect.x - 17, selectionRect.y - 1, 18, 18);

            if (isExpanded)
                EditorGUI.LabelField(iconRect, "", EnigmaticStyles.foldoutButtonOpen);
            else
                EditorGUI.LabelField(iconRect, "", EnigmaticStyles.foldoutButtonClose);
        }

        private static void DrawIcon(GameObject gameObject, Rect selectionRect, bool isSelected, bool isPrefabConnected)
        {
            if (sm_IsFolders.ContainsKey(gameObject))
            {
                Rect rect = new Rect(selectionRect.x - 1, selectionRect.y - 1, 19, 19);
                GUI.Box(rect, "", sm_IsFolders[gameObject].Value);
            }
            else if (sm_IsSpliters.ContainsKey(gameObject))
            {
                return;
            }
            else if (isPrefabConnected && isSelected == false)
            {
                Rect rect = new Rect(selectionRect.x - 2, selectionRect.y - 1, 19, 19);
                GUI.Box(rect, "", EnigmaticStyles.gameObjectPrefabIcon);
            }
            else
            {
                Rect rect = new Rect(selectionRect.x - 2, selectionRect.y - 1, 19, 19);
                EditorGUI.LabelField(rect, "", EnigmaticStyles.gameObjectIcon);
            }
        }

        private static void DrawObjectName(GameObject gameObject, Rect selectionRect, 
            bool isSelected, bool isFolder, bool isSplitter, bool isPrefabConnected)
        {
            Rect rect = new Rect(selectionRect.x + 16, selectionRect.y, selectionRect.width - 78, selectionRect.height);
            GUIStyle objectNameStyle = EditorStyles.label;

            if (isSplitter)
            {
                rect.y -= 1;
                objectNameStyle = EnigmaticStyles.splitterLabel;
            }
            else if (isFolder)
            {
                objectNameStyle = EnigmaticStyles.folderLabel;
            }
            else if (isPrefabConnected && isSelected == false)
            {
                objectNameStyle = EnigmaticStyles.gameObjectNameLabelPrefabConnection;
            }

            GUI.Label(rect, gameObject.name, objectNameStyle);
        }

        private static void DrawObjectActiveToggle(GameObject gameObject, Rect selectionRect)
        {
            Rect rect = new Rect(sm_CurrentViewWidth - 35, selectionRect.y - 1, 18, 18);
            bool active = EditorGUI.Toggle(rect, gameObject.activeSelf, EditorStyles.toggle);

            if (gameObject.activeSelf != active)
                gameObject.SetActive(active);
        }

        private static void DrawLayerSelectionButton(GameObject gameObject, Rect selectionRect)
        {
            Rect rect = new Rect(sm_CurrentViewWidth - 57, selectionRect.y - 1, 18, 18);
            GUIContent layerContent = new GUIContent("", LayerMask.LayerToName(gameObject.layer));

            if (GUI.Button(rect, layerContent, EnigmaticStyles.layersIcon))
                OnChageObjectLayer(gameObject);
        }

        private static void DrawTagSelectionButton(GameObject gameObject, Rect selectionRect)
        {
            Rect rect = new Rect(sm_CurrentViewWidth - 77, selectionRect.y - 2, 20, 20);
            GUIContent tagContent = new GUIContent("", gameObject.tag);

            if (GUI.Button(rect, tagContent, EnigmaticStyles.tagIcon))
                OnChageObjectTag(gameObject);
        }

        private static void DrawPrefabOpenButton(Rect selectionRect)
        {
            Rect rect = new Rect(sm_CurrentViewWidth - 20, selectionRect.y - 1, 19, 19);
            GUI.Box(rect, "", EnigmaticStyles.arrowRightIcon);
        }

        private static void DrawObjectTreeHierarchy(GameObject gameObject, Rect selectionRect)
        {
            if (gameObject.transform.childCount == 0 && gameObject.transform.parent != null)
            {
                Rect rect = new Rect(selectionRect.x - 14, selectionRect.y, 14, 16);

                if (CheckIsLastOnParent(gameObject))
                    GUI.Box(rect, "", EnigmaticStyles.treeHierarchyEnd);
                else
                    GUI.Box(rect, "", EnigmaticStyles.treeHierarchy);
            }

            if (selectionRect.x > 60)
            {
                int depth = (int)(selectionRect.x - 60) / 14;

                for (int i = 0; i < depth; i++)
                {
                    if (i == 0 || CheckIsLastOnParentByDepth(gameObject, i))
                        continue;

                    Rect rect = new Rect(selectionRect.x - 14 * (i + 1), selectionRect.y, 14, 16);
                    GUI.Box(rect, "", EnigmaticStyles.treeHierarchyContinue);
                }
            }
        }

        private static bool CheckIsSelected(GameObject gameObject)
        {
            foreach(GameObject g in Selection.gameObjects)
            {
                if (g == gameObject)
                    return true;
            }

            return false;
        }

        private static bool CheckIsLastOnParentByDepth(GameObject gameObject, int depth)
        {
            if (gameObject.transform.parent == null)
                return false;//

            GameObject parent = gameObject;

            for (int i = 1; i <= depth; i++)
                parent = parent.transform.parent.gameObject;

            return CheckIsLastOnParent(parent);
        }

        private static bool CheckIsLastOnParent(GameObject gameObject)
        {
            if(gameObject.transform.parent == null)
                return false;

            int last = gameObject.transform.parent.childCount - 1;

            return gameObject.transform.parent.GetChild(last).transform == gameObject.transform;
        }

        private static void OnChageObjectLayer(GameObject gameObject)
        {
            GenericMenu menu = new GenericMenu();

            string[] layers = InternalEditorUtility.layers;
            
            foreach(string layer in layers)
            {
                bool isSelected = LayerMask.NameToLayer(layer) == gameObject.layer;
                menu.AddItem(new GUIContent(layer), isSelected, SelectedLayer, new SelectedData(gameObject, layer));
            }

            menu.ShowAsContext();
        }

        private static void OnChageObjectTag(GameObject gameObject)
        {
            GenericMenu menu = new GenericMenu();

            string[] tags = InternalEditorUtility.tags;

            foreach (string tag in tags)
            {
                bool isSelected = tag == gameObject.tag;
                menu.AddItem(new GUIContent(tag), isSelected, SelectedTag, new SelectedData(gameObject, tag));
            }

            menu.ShowAsContext();
        }

        private static void SelectedLayer(object selectedData)
        {
            SelectedData data = (SelectedData)selectedData;
            data.GameObject.layer = LayerMask.NameToLayer(data.ElementName);
        }

        private static void SelectedTag(object selectedData)
        {
            SelectedData data = (SelectedData)selectedData;
            data.GameObject.tag = data.ElementName;
        }

        private static bool IsHierarchyExpanded(int instanceID)
        {
            return sm_ObjectIDs.ToList().Contains(instanceID);
        }

        private static bool IsPrefabRoot(GameObject gameObject)
        {
            return PrefabUtility.IsPartOfPrefabInstance(gameObject) 
                && PrefabUtility.GetPrefabInstanceHandle(gameObject.transform.parent) == null;
        }

        private static object GetHierarchyWindow()
        {
            var windowType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var windows = Resources.FindObjectsOfTypeAll(windowType);
            return windows.Length > 0 ? windows[0] : null;
        }
    }
}
