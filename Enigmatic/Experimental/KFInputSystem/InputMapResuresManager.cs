using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Enigmatic.Experimental.ENIX;
using Enigmatic.Experimental.KFInputSystem.Editor;

namespace Enigmatic.Experimental.KFInputSystem
{
    internal static class InputMapResuresManager
    {
        public static void SaveEditorMaps(InputMaps inputMaps)
        {
            if (Directory.Exists(EnigmaticData.GetFullPath(EnigmaticData.inputStorege)) == false)
                Directory.CreateDirectory(EnigmaticData.GetFullPath(EnigmaticData.inputStorege));

            KFInputMapContaner contaner = ScriptableObject.CreateInstance<KFInputMapContaner>();
            contaner.name = "EditorInputSettings";

            List<string> serializedMaps = ENIXFile.Serialize(new object[] { inputMaps });
            contaner.AddObjects(serializedMaps);

            string path = EnigmaticData.GetUnityPath($"{EnigmaticData.inputEditorSettings}");
            AssetDatabase.CreateAsset(contaner, path);
        }

        public static InputMaps LoadEditorMaps()
        {
            KFInputMapContaner contaner = EnigmaticData.LoadResources<KFInputMapContaner>("KFInput/EditorInputSettings");
            var filteredObjects = ENIXFile.FilterObjectsByType(ENIXFile.Deserialize(contaner), new Type[] { typeof(InputMaps) });
            return (InputMaps)filteredObjects[typeof(InputMaps)][0];
        }

        public static void SeveMap(InputMaps inputMaps)
        {
            InputMapsProvider provider = InputMapGenerator.GenerateProvider(inputMaps);
            List<string> serializedMaps = new List<string>();

            serializedMaps = ENIXFile.Serialize(new object[] { provider });

            KFInputMapContaner contaner = ScriptableObject.CreateInstance<KFInputMapContaner>();
            contaner.AddObjects(serializedMaps);
            contaner.name = "InputSettings";

            string path = EnigmaticData.GetUnityPath($"{EnigmaticData.inputSettings}");
            AssetDatabase.CreateAsset(contaner, path);
        }

        public static InputMapsProvider LoadMap()
        {
            KFInputMapContaner contaner = EnigmaticData.LoadResources<KFInputMapContaner>("KFInput/InputSettings");
            var filteredObjects = ENIXFile.FilterObjectsByType(ENIXFile.Deserialize(contaner), new Type[] { typeof(InputMapsProvider) });
            return (InputMapsProvider)filteredObjects[typeof(InputMapsProvider)][0];
        }
    }
}
