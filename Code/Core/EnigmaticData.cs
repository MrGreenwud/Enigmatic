using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Enigmatic.Core
{
    public static class EnigmaticData
    {
        public static readonly string main = $"Enigmatic";

        public static readonly string resources = $"Resources/Enigmatic";
        public static readonly string resourcesEditor = $"{resources}/Editor";
        public static readonly string source = $"{main}/Source";

        //Editor
        public static readonly string textures = $"{source}/Editor/Texture";

        //Input
        public static readonly string inputStorege = $"{resources}/KFInput";

        public static readonly string inputEditorSettings = $"{inputStorege}/EditorInputSettings.asset";
        public static readonly string inputSettings = $"{inputStorege}/InputSettings.asset";

        public static readonly string inputProviders = $"{inputStorege}/Providers";
        public static readonly string inputMaps = $"{inputStorege}/Maps";

        //SearchedTree
        public static string enigmaticTree => GetFullPath($"{source}/SearchedTree");
        public static readonly string treeStorege = $"{resources}/SearchedTree";

        //EditorStyle settings
        public static readonly string editorStyleSettings = $"{source}/Editor/EnigmaticStylesSettings.asset";

        //NodeGraph EditorInfo
        public static readonly string nodeGraphEditorInfo = $"{resourcesEditor}/NodeGraph";

#if UNITY_EDITOR

        public static UnityEngine.Object[] LoadAllAssetsAtPathWithExtantion(string path, string extantion, Type type)
        {
            List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

            string[] paths =
                Directory.GetFiles(GetFullPath(path), extantion)
                .Select((x) => GetUnityPath(GetUniformPath(x))).ToArray();

            foreach (string p in paths)
                assets.Add(AssetDatabase.LoadAssetAtPath(p, type));

            return assets.ToArray();
        }

#endif

        public static string GetFullPath(string path) => $"{Application.dataPath}/{path}";
        
        public static string GetUnityPath(string path) => $"Assets/{path}";
        
        public static string GetUniformPath(string path)
        {
            Queue<string> elments = path.Split('/').ToQueue();
            string resulPath = string.Empty;

            bool isFindRootFolder = false;

            while(elments.Count > 0)
            {
                if (isFindRootFolder)
                {
                    resulPath += $"{elments.Dequeue()}";

                    if (elments.Count > 0)
                        resulPath += "/";
                }
                else if (elments.Dequeue() == "Assets"
                    && elments.Contains("Assets") == false)
                {
                    isFindRootFolder = true;
                }
            }

            return resulPath;
        }

        public static string GetPath(params string[] paths)
        {
            string path = string.Empty;

            foreach (string p in paths)
                path += $"/{p}";

            return path;
        }

        public static T LoadResources<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>($"Enigmatic/{path}");
        }

        public static Texture2D LoadEditorTexture(string fileName, string additionalPath = "")
        {
            return LoadEditorResources(GetUnityPath($"{textures}/{additionalPath}/{fileName}")) as Texture2D;
        }

        public static UnityEngine.Object LoadEditorResources(string path)
        {
            return EditorGUIUtility.Load(path);
        }
    }
}
