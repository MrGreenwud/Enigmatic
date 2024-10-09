using UnityEditor;
using UnityEngine;

namespace Enigmatic.Core
{
    [InitializeOnLoad]
    public static class EnigmaticStyles
    {
        #region Main
        
        //Main
        private static GUIStyle sm_WhiteBox;
        private static GUIStyle sm_ArrowRightIcon;

        private static GUIStyle sm_AddButton;
        private static GUIStyle sm_SubstractButton;

        private static GUIStyle sm_ColumBackground;
        private static GUIStyle sm_ColumBackgroundSelected;
        private static GUIStyle sm_ToolbarButton;

        private static Color sm_DarkThemeBlue = new Color(0.227f, 0.447f, 0.690f, 1);
        private static Color sm_AltDarkThemeBlue = new Color(0.2724f, 0.5364f, 0.828f, 1);

        #endregion

        #region Massege

        //Massege
        private static GUIStyle sm_ErrorIcon;
        private static GUIStyle sm_WarningIcon;
        private static GUIStyle sm_MessegeIcon;

        #endregion

        #region Inspector

        //Inspector
        private static GUIStyle sm_SplitterLabel;
        private static GUIStyle sm_FolderLabel;
        private static GUIStyle sm_GameObjectNameLabelPrefabConnection;

        private static GUIStyle sm_GameObjectIcon;
        private static GUIStyle sm_GameObjectPrefabIcon;
        private static GUIStyle sm_LightIcon;

        private static GUIStyle sm_LayersIcon;
        private static GUIStyle sm_TagIcon;

        private static GUIStyle sm_FolderIcon;

        private static GUIStyle sm_TreeHierarchy;
        private static GUIStyle sm_TreeHierarchyContinue;
        private static GUIStyle sm_TreeHierarchyEnd;

        private static Color sm_DarkThemeBlueElementSelected = new Color(0.19295f, 0.37995f, 0.5865f, 0.85f);

        #endregion

        #region Node Graph

        //Node Graph
        private static GUIStyle sm_NodeTitle;
        private static GUIStyle sm_NodeLitleTitle;

        private static GUIStyle sm_NodeBox;
        private static GUIStyle sm_SelectedOutLine;
        private static GUIStyle sm_Port;

        private static GUIStyle sm_InputDataIcon;
        private static GUIStyle sm_OutputDataIcon;
        private static GUIStyle sm_Parametor;

        private static GUIStyle sm_SubgraphNodeIcon;
        private static GUIStyle sm_NodeEditorWindowIcon;
        private static GUIStyle sm_NodeEditorInspectorWindowIcon;

        #endregion

        //Checked ? -----> Trash
        private static GUIStyle sm_FoldoutButtonClose;
        private static GUIStyle sm_FoldoutButtonOpen;
        private static GUIStyle sm_FoldoutBackground;

        private static GUIStyle sm_ToolBarButton; //Delete

        private static GUIStyle sm_AddFoldoutButton; //Check


        #region Main Property

        public static GUIStyle whiteBox
        {
            get
            {
                if (sm_WhiteBox == null)
                {
                    Texture2D whiteTexture = new Texture2D(1, 1);
                    whiteTexture.SetPixel(0, 0, new Color(1, 1, 1, 0.05f));
                    whiteTexture.Apply();

                    sm_WhiteBox = new GUIStyle(GUI.skin.box);
                    sm_WhiteBox.normal.background = whiteTexture;
                }

                return sm_WhiteBox;
            }
        }

        public static GUIStyle arrowRightIcon
        {
            get
            {
                if (sm_ArrowRightIcon == null)
                {
                    sm_ArrowRightIcon = new GUIStyle(GUI.skin.box);
                    sm_ArrowRightIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/ArrowRight.png") as Texture2D;
                }

                return sm_ArrowRightIcon;
            }
        }

        public static GUIStyle addButton
        {
            get
            {
                if (sm_AddButton == null)
                {
                    sm_AddButton = new GUIStyle(GUI.skin.box);
                    sm_AddButton.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/AddButton.png") as Texture2D;
                }

                return sm_AddButton;
            }
        }

        public static GUIStyle substractButton
        {
            get
            {
                if (sm_SubstractButton == null)
                {
                    sm_SubstractButton = new GUIStyle(GUI.skin.box);
                    sm_SubstractButton.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/SubstractButton.png") as Texture2D;
                }

                return sm_SubstractButton;
            }
        }

        public static GUIStyle columBackground
        {
            get
            {
                if(sm_ColumBackground == null)
                {
                    sm_ColumBackground = new GUIStyle(GUI.skin.box);
                    sm_ColumBackground.normal.textColor = GUI.skin.label.normal.textColor;

                    //sm_ColumBackground.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/ColumBackground.png") as Texture2D;
                    sm_ColumBackground.normal.background = EditorGUIUtility.Load(EnigmaticData.GetUnityPath(EnigmaticData.GetPath
                        (EnigmaticData.textures, "ColumBackground.png"))) as Texture2D;

                    sm_ColumBackground.border = new RectOffset(4, 4, 4, 4); // Регулируйте отступы, чтобы избежать обрезания текста
                    sm_ColumBackground.padding = new RectOffset(2, 2, 2, 2); // Регулируйте отступы, чтобы избежать обрезания текста
                }

                return sm_ColumBackground;
            }
        }

        public static GUIStyle columBackgroundSelected
        {
            get
            {
                if (sm_ColumBackgroundSelected == null)
                {
                    sm_ColumBackgroundSelected = new GUIStyle(GUI.skin.box);
                    sm_ColumBackgroundSelected.normal.textColor = GUI.skin.label.normal.textColor;

                    sm_ColumBackgroundSelected.normal.background = EditorGUIUtility.Load(EnigmaticData.GetUnityPath(EnigmaticData
                        .GetPath(EnigmaticData.textures, "ColumBackgroundSelected.png"))) as Texture2D;

                    sm_ColumBackgroundSelected.border = new RectOffset(4, 4, 4, 4); // Регулируйте отступы, чтобы избежать обрезания текста
                    sm_ColumBackgroundSelected.padding = new RectOffset(2, 2, 2, 2); // Регулируйте отступы, чтобы избежать обрезания текста
                }

                return sm_ColumBackgroundSelected;
            }
        }

        public static GUIStyle toolbarButton
        { 
            get
            {
                if(sm_ToolBarButton == null)
                {
                    sm_ToolBarButton = new GUIStyle(EditorStyles.label);
                    sm_ToolBarButton.normal.textColor = GUI.skin.label.normal.textColor;
                    sm_ToolBarButton.alignment = TextAnchor.MiddleCenter;
                    sm_ToolBarButton.fontStyle = FontStyle.Bold;
                    sm_ToolBarButton.fontSize = 20;
                }

                return sm_ToolBarButton;
            }
        }

        public static Color DarkThemeBlue => sm_DarkThemeBlue;
        public static Color AltDarkThemeBlue => sm_AltDarkThemeBlue;

        #endregion

        #region Massege Property

        public static GUIStyle errorIcon
        {
            get
            {
                if (sm_ErrorIcon == null)
                {
                    sm_ErrorIcon = new GUIStyle(GUI.skin.box);
                    sm_ErrorIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/ErrorIcon.png") as Texture2D;
                }

                return sm_ErrorIcon;
            }
        }

        #endregion

        #region Inspector Property

        public static GUIStyle splitterLabel
        {
            get
            {
                if(sm_SplitterLabel == null)
                {
                    sm_SplitterLabel = new GUIStyle(EditorStyles.boldLabel);
                    sm_SplitterLabel.fontSize = 14;
                    sm_SplitterLabel.alignment = TextAnchor.MiddleCenter;
                }

                return sm_SplitterLabel;
            }
        }

        public static GUIStyle folderLabel
        {
            get
            {
                if(sm_FolderLabel == null)
                {
                    sm_FolderLabel = new GUIStyle(EditorStyles.boldLabel);
                    sm_FolderLabel.fontStyle = FontStyle.BoldAndItalic; ;
                }

                return sm_FolderLabel;
            }
        }

        public static GUIStyle gameObjectNameLabelPrefabConnection
        {
            get
            {
                if(sm_GameObjectNameLabelPrefabConnection == null)
                {
                    sm_GameObjectNameLabelPrefabConnection = new GUIStyle(EditorStyles.label);
                    sm_GameObjectNameLabelPrefabConnection.normal.textColor = BluePrefabConnected;
                }

                return sm_GameObjectNameLabelPrefabConnection;
            }
        }

        public static GUIStyle gameObjectIcon
        {
            get
            {
                if (sm_GameObjectIcon == null)
                {
                    sm_GameObjectIcon = new GUIStyle(GUI.skin.box);
                    sm_GameObjectIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/GameObject.png") as Texture2D;
                }

                return sm_GameObjectIcon;
            }
        }

        public static GUIStyle gameObjectPrefabIcon
        {
            get
            {
                if (sm_GameObjectPrefabIcon == null)
                {
                    Texture2D texture = gameObjectIcon.normal.background;
                    Color color = new Color(0.227f, 0.447f, 0.690f, 1) * 1.4f;
                    sm_GameObjectPrefabIcon = new GUIStyle(GUI.skin.box);
                    Texture2D newTexure = MultiplyColor(texture, color).normal.background;
                    sm_GameObjectPrefabIcon.normal.background = newTexure;
                }

                return sm_GameObjectPrefabIcon;
            }
        }

        public static GUIStyle lightIcon
        {
            get
            {
                if (sm_LightIcon == null)
                {
                    sm_LightIcon = new GUIStyle(GUI.skin.box);
                    sm_LightIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/Light.png") as Texture2D;
                }

                return sm_LightIcon;
            }
        }

        public static GUIStyle layersIcon
        {
            get
            {
                if (sm_LayersIcon == null)
                {
                    sm_LayersIcon = new GUIStyle(GUI.skin.box);
                    sm_LayersIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/Layers.png") as Texture2D;
                }

                return sm_LayersIcon;
            }
        }

        public static GUIStyle tagIcon
        {
            get
            {
                if (sm_TagIcon == null)
                {
                    sm_TagIcon = new GUIStyle(GUI.skin.box);
                    sm_TagIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/Tag.png") as Texture2D;
                }

                return sm_TagIcon;
            }
        }

        public static GUIStyle folderIcon
        {
            get
            {
                if (sm_FolderIcon == null)
                {
                    sm_FolderIcon = new GUIStyle();
                    sm_FolderIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/BaseFoldor.png") as Texture2D;
                }

                return sm_FolderIcon;
            }
        }

        public static GUIStyle treeHierarchy
        {
            get
            {
                if (sm_TreeHierarchy == null)
                {
                    sm_TreeHierarchy = new GUIStyle();
                    sm_TreeHierarchy.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/TreeHierarchy.png") as Texture2D;
                }

                return sm_TreeHierarchy;
            }
        }

        public static GUIStyle treeHierarchyContinue
        {
            get
            {
                if (sm_TreeHierarchyContinue == null)
                {
                    sm_TreeHierarchyContinue = new GUIStyle();
                    sm_TreeHierarchyContinue.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/TreeHierarchyContinue.png") as Texture2D;
                }

                return sm_TreeHierarchyContinue;
            }
        }

        public static GUIStyle treeHierarchyEnd
        {
            get
            {
                if (sm_TreeHierarchyEnd == null)
                {
                    sm_TreeHierarchyEnd = new GUIStyle();
                    sm_TreeHierarchyEnd.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/TreeHierarchyEnd.png") as Texture2D;
                }

                return sm_TreeHierarchyEnd;
            }
        }

        public static Color DarkThemeBlueElementSelected => sm_DarkThemeBlueElementSelected;
        public static Color BluePrefabConnected => new Color(0.227f, 0.447f, 0.690f, 1) * 1.4f;
        
        public static Color BackgroundColor
        {
            get
            {
                Color color = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f, 1) : new Color(0.76f, 0.76f, 0.76f);
                color.a = 1;
                return color;
            }
        }

        public static Color DarkedBackgroundColor
        {
            get
            {
                Color color = BackgroundColor * 0.97f;
                color.a = 1;
                return color;
            }
        }

        public static Color HeverColor
        {
            get
            {
                Color color = BackgroundColor * 1.2f;
                color.a = 0.5f;
                return color;
            }
        }

        #endregion

        #region Node Graph Property

        public static GUIStyle nodeTitleStyle
        {
            get
            {
                if (sm_NodeTitle == null)
                {
                    sm_NodeTitle = new GUIStyle(GUI.skin.label);

                    sm_NodeTitle.fontSize = 13;
                    sm_NodeTitle.fontStyle = FontStyle.Bold;
                    sm_NodeTitle.alignment = TextAnchor.MiddleCenter;
                }

                return sm_NodeTitle;
            }
        }

        public static GUIStyle nodeLitleTitleStyle
        {
            get
            {
                if (sm_NodeLitleTitle == null)
                {
                    sm_NodeLitleTitle = new GUIStyle(GUI.skin.label);

                    sm_NodeLitleTitle.fontSize = 8;
                    sm_NodeLitleTitle.alignment = TextAnchor.MiddleCenter;
                }

                return sm_NodeLitleTitle;
            }
        }

        public static GUIStyle nodeBox
        {
            get
            {
                if (sm_NodeBox == null)
                {
                    sm_NodeBox = new GUIStyle(GUI.skin.box);
                    sm_NodeBox.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeBox.png") as Texture2D;
                    sm_NodeBox.border = new RectOffset(22, 22, 45, 22); // Регулируйте отступы, чтобы избежать обрезания текста
                    sm_NodeBox.padding = new RectOffset(12, 12, 12, 12); // Регулируйте отступы, чтобы избежать обрезания текста
                }

                return sm_NodeBox;
            }
        }

        public static GUIStyle selectedOutLine
        {
            get
            {
                if (sm_SelectedOutLine == null)
                {
                    sm_SelectedOutLine = new GUIStyle(GUI.skin.box);
                    sm_SelectedOutLine.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeSelected.png") as Texture2D;
                    sm_SelectedOutLine.border = new RectOffset(16, 16, 16, 16); // Регулируйте отступы, чтобы избежать обрезания текста
                }

                return sm_SelectedOutLine;
            }
        }

        public static GUIStyle Port
        {
            get
            {
                if (sm_Port == null)
                {
                    sm_Port = new GUIStyle();
                    sm_Port.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/Port.png") as Texture2D;
                }

                return sm_Port;
            }
        }

        public static GUIStyle inputData
        {
            get
            {
                if (sm_InputDataIcon == null)
                {
                    sm_InputDataIcon = new GUIStyle();
                    sm_InputDataIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/InputData.png") as Texture2D;
                }

                return sm_InputDataIcon;
            }
        }

        public static GUIStyle outputData
        {
            get
            {
                if (sm_OutputDataIcon == null)
                {
                    sm_OutputDataIcon = new GUIStyle();
                    sm_OutputDataIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/OutputData.png") as Texture2D;
                }

                return sm_OutputDataIcon;
            }
        }

        public static GUIStyle parameter
        {
            get
            {
                if (sm_Parametor == null)
                {
                    sm_Parametor = new GUIStyle();
                    sm_Parametor.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/Parametor.png") as Texture2D;
                }

                return sm_Parametor;
            }
        }

        public static GUIStyle subgraphNodeIcon
        {
            get
            {
                if (sm_SubgraphNodeIcon == null)
                {
                    sm_SubgraphNodeIcon = new GUIStyle();
                    sm_SubgraphNodeIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/SubGraphNode.png") as Texture2D;
                }

                return sm_SubgraphNodeIcon;
            }
        }

        public static GUIStyle nodeEditorWindowIcon
        {
            get
            {
                if (sm_NodeEditorWindowIcon == null)
                {
                    sm_NodeEditorWindowIcon = new GUIStyle(GUI.skin.box);
                    sm_NodeEditorWindowIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeEditorGraphWindowIcon.png") as Texture2D;
                }

                return sm_NodeEditorWindowIcon;
            }
        }

        public static GUIStyle nodeEditorInspectorWindowIcon
        {
            get
            {
                if (sm_NodeEditorInspectorWindowIcon == null)
                {
                    sm_NodeEditorInspectorWindowIcon = new GUIStyle(GUI.skin.box);
                    sm_NodeEditorInspectorWindowIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeEditorGraphInspectorWindowIcon.png") as Texture2D;
                }

                return sm_NodeEditorInspectorWindowIcon;
            }
        }

        #endregion

        public static GUIStyle foldoutButtonClose
        {
            get
            {
                if (sm_FoldoutButtonClose == null)
                {
                    sm_FoldoutButtonClose = new GUIStyle(GUI.skin.box);
                    sm_FoldoutButtonClose.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/FoldoutButtonClose.png") as Texture2D;
                }

                return sm_FoldoutButtonClose;
            }
        }

        public static GUIStyle foldoutButtonOpen
        {
            get
            {
                if (sm_FoldoutButtonOpen == null)
                {
                    sm_FoldoutButtonOpen = new GUIStyle(GUI.skin.box);
                    sm_FoldoutButtonOpen.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/FoldoutButtonOpen.png") as Texture2D;
                }

                return sm_FoldoutButtonOpen;
            }
        }

        public static GUIStyle foldoutBackground
        {
            get
            {
                if (sm_FoldoutBackground == null)
                {
                    sm_FoldoutBackground = new GUIStyle(GUI.skin.box);
                    sm_FoldoutBackground.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/FoldoutBackground.png") as Texture2D;
                    sm_FoldoutBackground.border = new RectOffset(9, 9, 9, 9); // Регулируйте отступы, чтобы избежать обрезания текста
                    sm_FoldoutBackground.padding = new RectOffset(7, 7, 7, 7);
                }

                return sm_FoldoutBackground;
            }
        }

        public static GUIStyle toolBarButton
        {
            get
            {
                if (sm_ToolBarButton == null)
                {
                    sm_ToolBarButton = new GUIStyle(GUI.skin.box);
                    sm_ToolBarButton.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/ToolBarButton.png") as Texture2D;
                    sm_ToolBarButton.border = new RectOffset(17, 17, 2, 2); // Регулируйте отступы, чтобы избежать обрезания текста
                    sm_ToolBarButton.padding = new RectOffset(6, 6, 6, 6);
                }

                return sm_ToolBarButton;
            }
        } //Delete

        public static GUIStyle addFoldoutButton
        {
            get
            {
                if (sm_AddFoldoutButton == null)
                {
                    sm_AddFoldoutButton = new GUIStyle(GUI.skin.box);
                    sm_AddFoldoutButton.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/AddButtonFoldout.png") as Texture2D;
                }

                return sm_AddFoldoutButton;
            }
        }

        
        public static GUIStyle MultiplyColor(Texture2D texture, Color color)
        {
            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

            for (int x = 0; x <= newTexture.width; x++)
            {
                for (int y = 0; y <= newTexture.height; y++)
                {
                    Color originColor = texture.GetPixel(x, y);
                    newTexture.SetPixel(x, y, originColor * color);
                }
            }

            newTexture.Apply();

            GUIStyle result = new GUIStyle();
            result.normal.background = newTexture;

            return result;
        }
    }
}
