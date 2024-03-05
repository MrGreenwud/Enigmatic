using UnityEditor;
using UnityEngine;

namespace Enigmatic.Experemental.FolderColorize
{
    [InitializeOnLoad]
    public static class FolderColoredEditor
    {
        public static FolderColorsSetup s_Setup;

        static FolderColoredEditor()
        {
            s_Setup = AssetDatabase.LoadAssetAtPath("Assets/Enigmatic/Source/FolderColorsSetup.asset",
                typeof(FolderColorsSetup)) as FolderColorsSetup;
        }

        [MenuItem("Assets/Change Color/Reset", priority = 0)]
        public static void Reset()
        {
            DefaultAsset folder = Selection.activeObject as DefaultAsset;
            s_Setup.RemoveSettings(folder);
        }

        [MenuItem("Assets/Change Color/Orange")]
        public static void ChangeColorOrange() => ChangeColor("Orange");

        [MenuItem("Assets/Change Color/Green")]
        public static void ChangeColorGreen() => ChangeColor("Green");

        [MenuItem("Assets/Change Color/Purple")]
        public static void ChangeColorPurple() => ChangeColor("Purple");

        [MenuItem("Assets/Change Color/Black")]
        public static void ChangeColorBlack() => ChangeColor("Black");

        [MenuItem("Assets/Change Color/Blue")]
        public static void ChangeColorBlue() => ChangeColor("Blue");

        [MenuItem("Assets/Change Color/Gray")]
        public static void ChangeColorGray() => ChangeColor("Gray");

        [MenuItem("Assets/Change Color/Red")]
        public static void ChangeColorRed() => ChangeColor("Red");

        [MenuItem("Assets/Change Color/Turquoise")]
        public static void ChangeColorTurquoise() => ChangeColor("Turquoise");

        [MenuItem("Assets/Change Color/Yellow")]
        public static void ChangeColorYellow() => ChangeColor("Yellow");

        private static void ChangeColor(string color)
        {
            Texture2D icon = AssetDatabase.LoadAssetAtPath($"Assets/Enigmatic/Source/Icon/Folder/{color}.png",
                typeof(Texture2D)) as Texture2D;

            DefaultAsset folder = Selection.activeObject as DefaultAsset;

            if (folder == null)
                return;

            s_Setup.AddSettings(folder, icon);
        }
    }

    [InitializeOnLoad]
    public static class CustomFolderDrower
    {
        static CustomFolderDrower()
        {
            EditorApplication.projectWindowItemOnGUI += DrawFolder;
        }

        private static void DrawFolder(string guid, Rect rect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (path == "")
                return;
            
            DefaultAsset folder = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset)) as DefaultAsset;

            if (folder == null)
                return;

            Rect imageRect;

            if (rect.height > 20)
                imageRect = new Rect(rect.x - 1, rect.y - 1, rect.width + 2, rect.width + 2);
            else if (rect.x > 20)
                imageRect = new Rect(rect.x - 1, rect.y - 1, rect.height + 2, rect.height + 2);
            else
                imageRect = new Rect(rect.x + 2, rect.y - 1, rect.height + 2, rect.height + 2);

            Texture2D icon = FolderColoredEditor.s_Setup.GetIcon(folder);

            if (icon == null)
                return;

            GUI.DrawTexture(imageRect, icon);
        }
    }
}