using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Enigmatic.KFInputSystem
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

        public static EditorKFInput LoadInput(string inputKFIM)
        {
            EditorKFInput input = new EditorKFInput(GetTag(inputKFIM));

            input.Type = GetType(inputKFIM);
            input.Device = GetDevice(inputKFIM);

            input.Gravity = GetGravity(inputKFIM);
            input.Dead = GetDead(inputKFIM);
            input.Sensitivity = GetSensitivity(inputKFIM);

            string[] values = GetAxisValue(inputKFIM);
            string[,] buttons = GetAxisButton(inputKFIM);

            EditorKFAxis axisX = new EditorKFAxis();

            axisX.Value = values[0];
            axisX.PosetiveButton = buttons[0, 0];
            axisX.NegativeButton = buttons[0, 1];

            EditorKFAxis axisY = new EditorKFAxis();

            axisY.Value = values[1];
            axisY.PosetiveButton = buttons[1, 0];
            axisY.NegativeButton = buttons[1, 1];

            input.AxisX = axisX;
            input.AxisY = axisY;

            input.Button = GetButton(inputKFIM);

            return input;
        }

        public static string[] GetInputs(string kfimFile)
        {
            List<string> inputsKFIM = kfimFile.SplitArea(2, 21).ToList();
            return inputsKFIM.ToArray();
        }

        public static string[] GetAxis(string kfimInput)
        {
            string axis = kfimInput.Split("\n").ReadLines(7, 18);
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

        public static string GetDevice(string kfimInput)
        {
            string device = GetProperty(kfimInput, 3);
            return device;
        }

        public static float GetGravity(string kfimInput)
        {
            string gravity = GetProperty(kfimInput, 4);
            return float.Parse(gravity);
        }

        public static float GetDead(string kfimInput)
        {
            string dead = GetProperty(kfimInput, 5);
            return float.Parse(dead);
        }

        public static float GetSensitivity(string kfimInput)
        {
            string sensitivity = GetProperty(kfimInput, 6);
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

        public static string GetButton(string kfimInput)
        {
            string button = GetProperty(kfimInput, 19);
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

        private static string GetProperty(string kfimInput, int lineNumber)
        {
            string property = kfimInput.Split("\n")[lineNumber].Replace(" ", string.Empty).Split(':')[1];
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
            kfim += $"{space}Device: {input.Device}\n";

            kfim += $"{space}Gravity: {input.Gravity}\n";
            kfim += $"{space}Dead: {input.Dead}\n";
            kfim += $"{space}Sensitivity: {input.Sensitivity}\n";

            kfim += GenerateAxis($"AxisX", input.AxisX);
            kfim += GenerateAxis($"AxisY", input.AxisY);

            kfim += $"{space}Button: {input.Button}\n";

            kfim += $"{parentSpace}]\n";

            return kfim;
        }

        private static string GenerateAxis(string axisName, EditorKFAxis axis)
        {
            string parentSpace = FileEditor.Space(2);
            string space = FileEditor.Space(3);

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

    public static string ReadLines(this string[] stringsArray, int start, int end)
    {
        string result = string.Empty;

        for (int i = start; i <= end; i++)
            result +=  $"{stringsArray[i]}\n";

        return result;
    }

    public static string[] SplitArea(this string text, int startLineNumber, int areaLineCount)
    {
        string[] lines = text.Split("\n");

        List<string> result = new List<string>();

        string tempResult = string.Empty;

        int j = 1;

        for (int i = startLineNumber; i < lines.Length; i++)
        {
            tempResult += $"{lines[i]}\n";

            j++;

            if (j == areaLineCount + 1)
            {
                result.Add(tempResult);
                tempResult = string.Empty;
                j = 0;
            }
        }

        return result.ToArray();
    }
}

/// Grup_Name
/// {
///     [
///         Tag: tag
///         Type: type
///         Device: device
///         Gravity: 3
///         Dead: 0.001f
///         Sensitivity: 3
///         AxisX 
///         [
///             Value: axis
///             PosetiveButton: button
///             NegativeButton: button
///         ]
///         AxisY
///         [
///             Value: axis
///             PosetiveButton: button
///             NegativeButton: button
///         ]
///         Button: button
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
