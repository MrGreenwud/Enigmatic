using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CustomIconRepacer
{
    private static string defouldPath = $"{Application.dataPath}/Enigmatic/Source/Icon";

    private static string stpIconPath = $"{defouldPath}/stpFileIcon.png";

    private static Dictionary<string, string> s_Icons = new Dictionary<string, string>();

    static CustomIconRepacer()
    {
        EditorApplication.projectWindowItemOnGUI += DrawIcon;
    }

    private static void DrawIcon(string guid, Rect rect)
    {
        string file = AssetDatabase.GUIDToAssetPath(guid);

        if (file == "")
            return;

        string fileFormat = Path.GetExtension(file);
        
        Rect imageRect;

        if (rect.height > 20)
        {
            imageRect = new Rect(rect.x - 1, rect.y - 1, rect.width + 2, rect.width + 2);
        }
        else if (rect.x > 20)
        {
            imageRect = new Rect(rect.x - 1, rect.y - 1, rect.height + 2, rect.height + 2);
        }
        else
        {
            imageRect = new Rect(rect.x + 2, rect.y - 1, rect.height + 2, rect.height + 2);
        }

        if (fileFormat == ".stp")
        {
            Texture icon = GetTexture($"{defouldPath}/stpFileIcon.png");
            GUI.DrawTexture(imageRect, icon);
        }
        else if (fileFormat == ".kfim")
        {
            Texture icon = GetTexture($"{defouldPath}/kfimFileIcon.png");
            GUI.DrawTexture(imageRect, icon);
        }
    }

    private static Texture GetTexture(string path)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo($"{Application.dataPath}/Enigmatic/Source/Icon");
        FileInfo[] fileInfo = directoryInfo.GetFiles("*.png");
        Debug.Log($"Assets/Enigmatic/Source/Icon/{fileInfo[0].Name}");

        return (Texture)AssetDatabase.LoadAssetAtPath($"Assets/Enigmatic/Source/Icon/{fileInfo[0].Name}", typeof(Texture2D));
    }
}
