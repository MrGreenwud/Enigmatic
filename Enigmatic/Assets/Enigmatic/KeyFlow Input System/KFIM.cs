using System.Collections.Generic;
using System.IO;
using System.Linq;
using KFInputSystem;

public enum AxisDirection
{
    X,
    Y
}

public static class KFIM
{
    private static string s_kfimFile;
    private static Dictionary<string, string> s_Inputs = new Dictionary<string, string>();

    public static void Save(string name, string path)
    {

    }

    public static void Load(string path) 
    {
        s_Inputs.Clear();

        StreamReader reader = new StreamReader(path);
        string kfimFile = reader.ReadToEnd();

        for(int i = 0; i < kfimFile.Length; i++)
        {

        }
    }

    public static string[] GetTags()
    {
        return s_Inputs.Keys.ToArray();
    }

    public static string GetType(string tag)
    {
        string[] words = s_Inputs[tag].Split(' ');
        string type = "";

        for(int i = 0; i < words.Length; i++)
        {
            if (words[i] == "Type:")
                type = words[i + 1];
        }

        return type;
    }

    private static string GenerateInput(KFInputButton kFInputButton, string device)
    {
        string type = "Button/";

        if ((kFInputButton as KFInputButtonPress) != null)
            type += "Press";
        else if((kFInputButton as KFInputButtonDown) != null)
            type += "Down";
        else if((kFInputButton as KFInputButtonUp) != null)
            type += "Up";

        return GenerateInput(kFInputButton.Tag, type, device,
               GenerateAxis(kFInputButton.Tag, AxisDirection.X),
               GenerateAxis(kFInputButton.Tag, AxisDirection.Y),
               kFInputButton.KeyCode.ToString());
    }

    private static string GenerateInput(KFInputVec2 kFInputVec2, string device)
    {
        return "";
    }

    private static string GenerateInput(string tag, string type, string device, 
        string axisX, string axisY, string button)
    {
        string input = "";
        input += "[\n";

        input += $"Tag: {tag}\n";
        input += $"Type: {type}\n";
        input += $"Device: {device}\n";
        input += $"{axisX}\n";
        input += $"{axisY}\n";
        input += $"Button:: {button}";

        input += "]";

        return input;
    }

    private static string GenerateAxis(string inputTag, AxisDirection direction)
    {
        return GenerateAxis(inputTag, direction, "None", "None", "None");
    }

    private static string GenerateAxis(string inputTag, AxisDirection direction, string @float, 
        string posetiveButton, string negativeButton)
    {
        string axis = "";

        axis += $"Axis{direction}: \n";
        axis += "[\n";
        axis += $"Name: {inputTag}{direction}\n";
        axis += $"Float: {@float}\n";
        axis += $"PosetiveButton: {posetiveButton}\n";
        axis += $"NegativeButton: {negativeButton}\n";
        axis += "],\n";

        return axis;
    }

}

//public struct KFInput
//{
//    public string Tag { get; private set; }
//    public string Type { get; private set; }
//    public DeviceType Device { get; private set; }
//    public AxisI AxisX { get; private set; }
//    public AxisI AxisY { get; private set; }

//    public KFInput(string tag, string type, DeviceType device, AxisI axisX, AxisI axisY)
//    {
//        Tag = tag;
//        Type = type;
//        Device = device;
//        AxisX = axisX;
//        AxisY = axisY;
//    }
//}

//public struct AxisI
//{
//    public string Name { get; private set; }
//    public string Float { get; private set; }
//    public string PosetiveButton { get; private set; }
//    public string NegativeButton { get; private set; }

//    public AxisI(string name, string @float, string posetiveButton, string negativeButton)
//    {
//        Name = name;
//        Float = @float;
//        PosetiveButton = posetiveButton;
//        NegativeButton = negativeButton;
//    }
//}

/// KFInputs:
/// {
///     [
///         Tag: "tag1",
///         Type: "type",
///         Device: "device"
///         AxisX: 
///         [
///             Name: "tagX",
///             Float: "axis",
///             PosetiveButton: "button",
///             NegativeButton: "button",
///         ],
///         AxisY: 
///         [
///             Name: "tagY",
///             Float: "axis",
///             PosetiveButton: "button",
///             NegativeButton: "button",
///         ],
///         Button: "button"
///     ],
///     [
///         Tag: "tag2",
///         Type: "type",
///         Device: "device"
///         AxisX: 
///         [
///             name: "tagX",
///             Float: "axis",
///             PosetiveButton: "button",
///             NegativeButton: "button",
///         ],
///         AxisY: 
///         [
///             name: "tagY",
///             Float: "axis",
///             PosetiveButton: "button",
///             NegativeButton: "button",
///         ],
///         Button: "button"
///     ],
///     ...
/// }
/// 
//-------------------------------------------------------------------------------
/// GetTags(string kfimFile) return all input tags.
/// GetType(string tag) return type form input with tag.
/// GetDevice(string tag) return devvice form input with tag
/// GetAxis(string tag) return axis form input with tag
/// GetAxis(string tag) return two axis form input with tag
/// GetButton(sting tag) return button form input with tag
/// 
/// GetAxisNames(sting tag) return all axis name form input with tag
/// GetAxisFloat(sting tag, sting axisName) return float form input with tag in axis with axisName 
/// GetPosetiveButton(sting tag, sting axisName) return posetive button form input with tag in axis with axisName 
/// GetNegativeButton(sting tag, sting axisName) return negative button form input with tag in axis with axisName 
