using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Enigmatic.KFInputSystem.Editor
{
    public static class KFIMFile
    {
        public static string s_Path = $"{EnigmaticData.GetFullPath(EnigmaticData.inputMaps)}";

        private static List<string> s_kfimFiles = new List<string>();

        public static void Save()
        {
            if (Directory.Exists(s_Path) == false)
                Directory.CreateDirectory(s_Path);

            foreach (string kfim in s_kfimFiles)
            {
                string path = $"{s_Path}/{GetGrupName(kfim)}.kfim";
                File.WriteAllText(path, kfim);
            }

            s_kfimFiles.Clear();
        }

        public static List<KFInputGrup> Load()
        {
            List<KFInputGrup> grups = new List<KFInputGrup>();

            if (Directory.Exists(s_Path) == false)
                return null;

            string[] kfimFiles = Directory.GetFiles(s_Path, "*.kfim");

            foreach (string kfimFile in kfimFiles)
            {
                string grupName = Path.GetFileNameWithoutExtension(kfimFile);

                KFInputGrup newGrup = new KFInputGrup(grupName);
                EditorKFInput[] inputs = LoadInputs(grupName);

                foreach (EditorKFInput input in inputs)
                    newGrup.Add(input);

                grups.Add(newGrup);
            }

            return grups;
        }

        public static EditorKFInput[] LoadInputs(string grupName)
        {
            StreamReader reader = new StreamReader($"{s_Path}/{grupName}.kfim");
            string kfimFile = reader.ReadToEnd();
            reader.Close();

            string[] inputsKFIM = GetInputs(kfimFile);

            List<EditorKFInput> inputs = new List<EditorKFInput>();

            foreach (string inputKFIM in inputsKFIM)
                inputs.Add(LoadInput(inputKFIM));

            return inputs.ToArray();
        }

        public static EditorKFInput LoadInput(string kfiInput)
        {
            string[] kfiInputSettings = GetInputSettings(kfiInput);

            EditorKFInputSettings keyboadAndMouse = LoadSettings(kfiInputSettings[0]);
            EditorKFInputSettings joystick = LoadSettings(kfiInputSettings[1]);

            EditorKFInput input = new EditorKFInput(keyboadAndMouse, joystick);

            input.Tag = GetTag(kfiInput);
            input.Type = GetType(kfiInput);

            return input;
        }

        public static EditorKFInputSettings LoadSettings(string kfiInputSettings)
        {
            EditorKFInputSettings settings = new EditorKFInputSettings();

            settings.Device = GetDevice(kfiInputSettings);

            settings.Gravity = GetGravity(kfiInputSettings);
            settings.Dead = GetDead(kfiInputSettings);
            settings.Sensitivity = GetSensitivity(kfiInputSettings);

            EditorKFAxisSettings axisX = new EditorKFAxisSettings();
            EditorKFAxisSettings axisY = new EditorKFAxisSettings();

            string[] values = GetAxisValue(kfiInputSettings);
            string[,] buttons = GetAxisButton(kfiInputSettings);

            axisX.Value = values[0];
            axisX.PosetiveButton = buttons[0, 0];
            axisX.NegativeButton = buttons[0, 1];

            axisY.Value = values[1];
            axisY.PosetiveButton = buttons[1, 0];
            axisY.NegativeButton = buttons[1, 1];

            settings.AxisXSettings = axisX;
            settings.AxisYSettings = axisY;

            settings.Button = GetButton(kfiInputSettings);

            return settings;
        }

        public static string[] GetInputs(string kfimFile)
        {
            List<string> inputsKFIM = kfimFile.SplitArea(2, 44).ToList();
            return inputsKFIM.ToArray();
        }

        public static string[] GetInputSettings(string kfimInput)
        {
            List<string> inputsKFIM = kfimInput.SplitArea(3, 20).ToList();
            return inputsKFIM.ToArray();
        }

        public static string[] GetAxis(string kfimInputSettings)
        {
            string axis = kfimInputSettings.Split("\n").ReadLines(6, 17);
            List<string> axisKFIM = axis.SplitArea(0, 6).ToList();
            return axisKFIM.ToArray();
        }

        public static string GetTag(string kfimInput)
        {
            string tag = GetProperty(kfimInput, 1);
            return tag;
        }

        public static string GetType(string kfimInput)
        {
            string type = GetProperty(kfimInput, 2);
            return type;
        }

        public static string GetDevice(string kfimInputSettings)
        {
            string device = GetProperty(kfimInputSettings, 2);
            return device;
        }

        public static float GetGravity(string kfimInput)
        {
            string gravity = GetProperty(kfimInput, 3);
            return float.Parse(gravity);
        }

        public static float GetDead(string kfimInput)
        {
            string dead = GetProperty(kfimInput, 4);
            return float.Parse(dead);
        }

        public static float GetSensitivity(string kfimInput)
        {
            string sensitivity = GetProperty(kfimInput, 5);
            return float.Parse(sensitivity);
        }

        public static string[] GetAxisValue(string kfimInput)
        {
            string[] value = GetAxisProperty(kfimInput, 0);
            return value;
        }

        public static string[,] GetAxisButton(string kfimInput)
        {
            string[] posetiveButton = GetAxisProperty(kfimInput, 1);
            string[] negativeButton = GetAxisProperty(kfimInput, 2);

            string[,] buttons =
            {
                { posetiveButton[0], negativeButton[0] },
                { posetiveButton[1], negativeButton[1] }
            };

            return buttons;
        }

        public static string GetButton(string kfimInputSettings)
        {
            string button = GetProperty(kfimInputSettings, 18);
            return button;
        }

        private static string[] GetAxisProperty(string kfimInput, int propertyOrder)
        {
            string[] kfimAxis = GetAxis(kfimInput);

            string[] propertys =
            {
            GetProperty(kfimAxis[0], propertyOrder + 2),
            GetProperty(kfimAxis[1], propertyOrder + 2),
        };

            return propertys;
        }

        private static string GetProperty(string @object, int lineNumber)
        {
            //Debug
#if false
            Debug.Log(lineNumber);
            Debug.Log(@object.Split("\n")[lineNumber].Replace(" ", string.Empty));
#endif

            string property = @object.Split("\n")[lineNumber].Replace(" ", string.Empty).Split(':')[1];
            return property;
        }

        public static string[] GetAllInputTags(string kfim)
        {
            List<string> tags = new List<string>();

            string[] inputs = GetInputs(kfim);

            foreach (string input in inputs)
                tags.Add(GetTag(input));

            return tags.ToArray();
        }

        public static string GetGrupName(string kfim)
        {
            string[] lines = kfim.Split('\n');
            return lines[0];
        }

        public static void Genetate(KFInputGrup grup)
        {
            EditorKFInput[] inputs = grup.KFInputs;

            string kfim = $"{grup.Name}\n";
            kfim += "[\n";

            foreach (EditorKFInput input in inputs)
                kfim += GenerateInput(input);

            kfim += "]\n";

            s_kfimFiles.Add(kfim);
        }

        private static string GenerateInput(EditorKFInput input)
        {
            string parentSpace = FileEditor.Space(1);
            string space = FileEditor.Space(2);

            string kfim = "";

            kfim += $"{parentSpace}[\n";

            kfim += $"{space}Tag: {input.Tag}\n";
            kfim += $"{space}Type: {input.Type}\n";

            kfim += $"{space}Keyboard And Mouse\n";

            kfim += $"{space}[\n";

            kfim += $"{GenerateInputSettings(input.GetInputSettings(Device.Keyboard_and_Mouse))}";

            kfim += $"{space}]\n";

            kfim += $"{space}Joystic\n";

            kfim += $"{space}[\n";

            kfim += $"{GenerateInputSettings(input.GetInputSettings(Device.Joystick))}";

            kfim += $"{space}]\n";

            kfim += $"{parentSpace}]\n";

            return kfim;
        }

        private static string GenerateInputSettings(EditorKFInputSettings settings)
        {
            string space = FileEditor.Space(3);

            string kfim = "";

            kfim += $"{space}Device: {settings.Device}\n";

            kfim += $"{space}Gravity: {settings.Gravity}\n";
            kfim += $"{space}Dead: {settings.Dead}\n";
            kfim += $"{space}Sensitivity: {settings.Sensitivity}\n";

            kfim += GenerateAxis($"AxisX", settings.AxisXSettings);
            kfim += GenerateAxis($"AxisY", settings.AxisYSettings);

            kfim += $"{space}Button: {settings.Button}\n";

            return kfim;
        }

        private static string GenerateAxis(string axisName, EditorKFAxisSettings axis)
        {
            string parentSpace = FileEditor.Space(3);
            string space = FileEditor.Space(4);

            string kfim = $"{parentSpace}{axisName}\n";

            kfim += $"{parentSpace}[\n";

            kfim += $"{space}Value: {axis.Value}\n";
            kfim += $"{space}PosetiveButton: {axis.PosetiveButton}\n";
            kfim += $"{space}NegativeButton: {axis.NegativeButton}\n";

            kfim += $"{parentSpace}]\n";

            return kfim;
        }
    }
}

public static class EnumerableExtantion
{
    public static Queue<TSource> ToQueue<TSource>(this IEnumerable<TSource> sources)
    {
        Queue<TSource> queue = new Queue<TSource>();

        foreach (TSource source in sources)
            queue.Enqueue(source);

        return queue;
    }

    public static Queue<TSource> ToQueue<TSource>(this IEnumerable<TSource> sources, int startIndex)
    {
        Queue<TSource> queue = new Queue<TSource>();

        for (int i = (int)startIndex; i < sources.Count(); i++)
            queue.Enqueue(sources.ElementAt(i));

        return queue;
    }
}

/// Grup_Name
/// {
///     [
///         Tag: tag
///         Type: type
///         Keyboard And Mouse
///         [
///             Device: device
///             Gravity: 3
///             Dead: 0.001f
///             Sensitivity: 3
///             AxisX 
///             [
///                 Value: axis
///                 PosetiveButton: button
///                 NegativeButton: button
///             ]
///             AxisY
///             [
///                 Value: axis
///                 PosetiveButton: button
///                 NegativeButton: button
///             ]
///             Button: button
///         ]
///         Joystick
///         [
///             Device: device
///             Gravity: 3
///             Dead: 0.001f
///             Sensitivity: 3
///             AxisX 
///             [
///                 Value: axis
///                 PosetiveButton: button
///                 NegativeButton: button
///             ]
///             AxisY
///             [
///                 Value: axis
///                 PosetiveButton: button
///                 NegativeButton: button
///             ]
///             Button: button
///         ]
///     ]
///     
///     InputTag2:
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
/// GetGrup(string kfimFile) return input GrupName
/// GetInput(string kfimFile) return all input tags.
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
