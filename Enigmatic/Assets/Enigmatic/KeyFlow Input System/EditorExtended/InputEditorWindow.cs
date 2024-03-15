using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using KFInputSystem.Utility;
using Enigmatic.KFInputSystem;
using Enigmatic.Experemental.SearchedWindowUtility;
using Enigmatic.Experemental.CodeLiteGen;
using System;

public static class EnigmaticData
{
    public static readonly string resources = $"Resources/Enigmatic";
    public static readonly string source = $"Enigmatic/Source";

    //Input
    public static readonly string inputStorege = $"{resources}/KFInput";
    public static readonly string inputProviders = $"{inputStorege}/Providers";
    public static readonly string inputMaps = $"{inputStorege}/Maps";

    //SearchedTree
    public static readonly string enigmaticTree = $"{source}/SearchedTree";
    public static readonly string treeStorege = $"{resources}/SearchedTree";

    public static string GetFullPath(string path) => $"{Application.dataPath}/{path}";
    public static string GetUnityPath(string path) => $"Asset/{path}";
}

#if UNITY_EDITOR

public class InputEditorWindow : EditorWindow
{
    private static readonly string s_ProvidersPath = $"{EnigmaticData.inputStorege}/Providers";
    private static bool s_IsAutoSave;

    private List<KFInputGrup> m_KFGrups = new List<KFInputGrup>();

    private EditorKFInput m_SelectionKFInput;
    private KFInputGrup m_SelectionGrup;

    private Vector2 m_ScrollPositionGrup;
    private Vector2 m_ScrollPositionInput;
    
    [MenuItem("Tools/Enigmatic/KFInput")]
    public static void Open()
    {
        InputEditorWindow window = GetWindow<InputEditorWindow>();
        window.titleContent = new GUIContent("KFInput Editor");
        
        window.minSize = new Vector2(797, 490);
        window.maxSize = new Vector2(797, 490);
    }

    private void OnEnable()
    {
        m_KFGrups = KFIMFile.Load();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
        {
            if (GUILayout.Button("Save", GUILayout.Width(75)))
            {
                foreach (KFInputGrup grup in m_KFGrups)
                    KFIMFile.Genetate(grup);

                KFIMFile.Save();
            }

            if (GUILayout.Button("Apply", GUILayout.Width(75)))
                Apply();

            //797 - 540 = 257
            GUILayout.Space(position.width - 257);

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(250));
            {
                GUILayout.Label("Auto Save");
                s_IsAutoSave = EditorGUILayout.Toggle(s_IsAutoSave);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            //Grups List
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(200),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
                {
                    GUILayout.Label("Grups", EditorStyles.boldLabel);

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        m_KFGrups.Add(new KFInputGrup("New Grup"));

                    EditorGUI.BeginDisabledGroup(m_SelectionGrup == null);
                    {
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            m_KFGrups.Remove(m_SelectionGrup);
                            m_SelectionGrup = null;
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    m_ScrollPositionGrup = GUILayout.BeginScrollView(m_ScrollPositionGrup);

                    DrawGrupList();

                    GUILayout.EndScrollView();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            //Input List
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(250),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
                {
                    GUILayout.Label("Inputs", EditorStyles.boldLabel);

                    if (m_SelectionGrup != null)
                    {
                        if (GUILayout.Button("+", GUILayout.Width(20)))
                            m_SelectionGrup.Add(new EditorKFInput("New Input"));

                        EditorGUI.BeginDisabledGroup(m_SelectionKFInput == null);
                        {
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                m_SelectionGrup.Remove(m_SelectionKFInput);
                                m_SelectionKFInput = null;
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    if (m_SelectionGrup != null)
                    {
                        m_ScrollPositionInput = GUILayout.BeginScrollView(m_ScrollPositionInput);

                        DrawInputList();

                        GUILayout.EndScrollView();
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (m_SelectionGrup != null)
                {
                    EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
                    {
                        if (GUILayout.Button("Clear"))
                            m_SelectionGrup.Clear();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            ///Settings 
            EditorGUILayout.BeginVertical("box", GUILayout.Width(797 - 200 - 250),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(797 / 6));
                    {
                        GUILayout.Label("Settings:", EditorStyles.boldLabel);
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (m_SelectionGrup != null)
                            {
                                GUILayout.Label("Grup Name:");
                                m_SelectionGrup.Name = EditorGUILayout.TextField(m_SelectionGrup.Name);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical("box",
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    if (m_SelectionKFInput != null)
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            m_SelectionKFInput.Tag = EditorGUILayout.TextField("Tag:", m_SelectionKFInput.Tag);

                            EditorGUILayout.Space(3);

                            EditorGUILayout.BeginHorizontal();
                            {
                                SearchedTreeListProvider provider = SearchedTreeListProvider.Create("KFInput_System", "Input_Type", "type");
                                provider.OnSelected += OnSelectValue;
                                SearchedTreeUtility.DrawSelectionTree("Type", m_SelectionKFInput.Type, provider);
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(3);

                            EditorGUILayout.BeginHorizontal();
                            {
                                SearchedTreeListProvider provider = SearchedTreeListProvider.Create("KFInput_System", "Device", "device");
                                provider.OnSelected += OnSelectValue;
                                SearchedTreeUtility.DrawSelectionTree("Device", m_SelectionKFInput.Device, provider);
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(3);
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical("box");
                        {
                            if (SearchedTreeUtility.DeCompileTree(m_SelectionKFInput.Type, 0) == "Axis")
                            {
                                m_SelectionKFInput.Gravity = EditorGUILayout.FloatField("Gravity", m_SelectionKFInput.Gravity);
                                m_SelectionKFInput.Dead = EditorGUILayout.FloatField("Dead", m_SelectionKFInput.Dead);
                                m_SelectionKFInput.Sensitivity = EditorGUILayout.FloatField("Sensitivity", m_SelectionKFInput.Sensitivity);

                                GUILayout.Space(10);
                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical("box");
                        {
                            if (m_SelectionKFInput.Device != "None")
                            {
                                if (SearchedTreeUtility.DeCompileTree(m_SelectionKFInput.Type, 0) == "Button")
                                {
                                    SearchedTreeListProvider provider = SearchedTreeListProvider.Create("KFInput_System", $"{m_SelectionKFInput.Device}Button", "button");
                                    provider.OnSelected += OnSelectValue;
                                    SearchedTreeUtility.DrawSelectionTree("Button", m_SelectionKFInput.Button, provider);
                                }
                                else if (SearchedTreeUtility.DeCompileTree(m_SelectionKFInput.Type, 0) == "Axis")
                                {
                                    if (m_SelectionKFInput.Device == "Keyboard")
                                        DrawAxisKeyboard("AxisX", m_SelectionKFInput.AxisX, "AxisX");
                                    else
                                        DrawAxisJosticOrMouse("AxisX", m_SelectionKFInput.AxisX, "AxisX");

                                    if(SearchedTreeUtility.DeCompileTree(m_SelectionKFInput.Type, 1) == "Vector2")
                                    {
                                        if (m_SelectionKFInput.Device == "Keyboard")
                                            DrawAxisKeyboard("AxisY", m_SelectionKFInput.AxisY, "AxisY");
                                        else
                                            DrawAxisJosticOrMouse("AxisY", m_SelectionKFInput.AxisX, "AxisY");
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawInputList()
    {
        if (m_SelectionGrup == null)
            return;

        EditorKFInput[] Inputs = m_SelectionGrup.KFInputs;

        for (int i = 0; i < Inputs.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EnigmaticGUI.BeginSelectedGrup(m_SelectionKFInput == Inputs[i]);

                if (GUILayout.Button(Inputs[i].Tag, EditorStyles.toolbarButton))
                {
                    m_SelectionKFInput = Inputs[i];

                    GUI.FocusControl(null);
                }

                EnigmaticGUI.EndSelectedGrup();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);
        }
    }

    private void DrawGrupList()
    {
        for (int i = 0; i < m_KFGrups.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EnigmaticGUI.BeginSelectedGrup(m_SelectionGrup == m_KFGrups[i]);

                if (GUILayout.Button(m_KFGrups[i].Name, EditorStyles.toolbarButton))
                {
                    m_SelectionGrup = m_KFGrups[i];
                    m_SelectionKFInput = null;

                    GUI.FocusControl(null);
                }

                EnigmaticGUI.EndSelectedGrup();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);
        }
    }

    private void DrawAxisKeyboard(string axisName, EditorKFAxis axis, string senderUID)
    {
        SearchedTreeListProvider providerPosetive = SearchedTreeListProvider.Create("KFInput_System", $"{m_SelectionKFInput.Device}Button", $"{senderUID}Posetive");
        SearchedTreeListProvider providerNegative = SearchedTreeListProvider.Create("KFInput_System", $"{m_SelectionKFInput.Device}Button", $"{senderUID}Negative");

        providerPosetive.OnSelected += OnSelectValue;
        providerNegative.OnSelected += OnSelectValue;

        DrawAxis(axisName, axis, m_SelectionKFInput.Device, providerPosetive, providerNegative);
    }

    private void DrawAxisJosticOrMouse(string axisName, EditorKFAxis axis, string senderUID)
    {
        SearchedTreeListProvider provider =
            SearchedTreeListProvider.Create("KFInput_System", $"{m_SelectionKFInput.Device}Axis", $"{senderUID}");

        provider.OnSelected += OnSelectValue;

        DrawAxis(axisName, axis, m_SelectionKFInput.Device, provider);
    }

    private void DrawAxis(string name, EditorKFAxis axis, string device, params SearchedTreeListProvider[] providers)
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label(name);

            EditorGUILayout.BeginVertical("box");
            {
                if (device == "Keyboard")
                {
                    SearchedTreeUtility.DrawSelectionTree("PosetiveButton", axis.PosetiveButton, providers[0]);
                    SearchedTreeUtility.DrawSelectionTree("NegativeButton", axis.NegativeButton, providers[1]);
                }
                else
                {
                    SearchedTreeUtility.DrawSelectionTree("Value", axis.Value, providers[0]);
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnSelectValue(string senderUID, List<string> tree)
    {
        string value = SearchedTreeUtility.CompileTree(tree);

        switch (senderUID)
        {
            case "type":
                m_SelectionKFInput.Type = value;
                break;
            case "device":
                m_SelectionKFInput.Device = value;
                break;
            case "button":
                m_SelectionKFInput.Button = value;
                break;
            case "AxisXPosetive":
                m_SelectionKFInput.AxisX.PosetiveButton = value;
                break;
            case "AxisXNegative":
                m_SelectionKFInput.AxisX.NegativeButton = value;
                break;
            case "AxisYPosetive":
                m_SelectionKFInput.AxisY.PosetiveButton = value;
                break;
            case "AxisYNegative":
                m_SelectionKFInput.AxisY.NegativeButton = value;
                break;
            case "AxisX":
                m_SelectionKFInput.AxisX.Value = value;
                break;
            case "AxisY":
                m_SelectionKFInput.AxisY.Value = value;
                break;
        }
    }

    private void Apply()
    {
        GenerateInputTagsAndGrupsName();

        Queue<InputAxis> axis = new Queue<InputAxis>();

        foreach(KFInputGrup grup in m_KFGrups)
        {
            KFInputMapGrupProvider provider = CreateKFInputGrupProvider(grup);

            provider.GrupName = (InputGrup)Enum.Parse(typeof(InputGrup), grup.Name);

            foreach (EditorKFInput input in grup.KFInputs)
            {
                provider.AddInput(input);

                if (SearchedTreeUtility.DeCompileTree(input.Type, 0) == "Button")
                    continue;

                InputAxis axisX = new InputAxis($"{input.Tag} X", input, input.AxisX);
                InputAxis axisY = new InputAxis($"{input.Tag} Y", input, input.AxisY);

                if (SearchedTreeUtility.DeCompileTree(input.Type, 1) == "Vector2")
                {
                    axis.Enqueue(axisX);
                    axis.Enqueue(axisY);
                }
                else
                {
                    axis.Enqueue(axisX);
                }
            }

            EditorUtility.SetDirty(provider);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string path = $"Assets/Resources/Enigmatic/KFInput/Providers/{grup.Name}Provider.asset";
            Debug.Log($"Grup KFInput {grup.Name} saved to provider by path: {path}");
        }

        UnityInputManager.Clear();

        while(axis.Count > 0)
            UnityInputManager.AddAxis(axis.Dequeue());

        Debug.Log("All inputs applyed!");
    }

    private void GenerateInputTagsAndGrupsName()
    {
        string tags = string.Empty;
        string grups = string.Empty;

        foreach(KFInputGrup grup in m_KFGrups)
        {
            grups += $"{grup.Name},\n";

            foreach(EditorKFInput input in grup.KFInputs)
                tags += $"{input.Tag},\n";
        }

        string code =
            $"namespace Enigmatic.KFInputSystem\n" +
            $"{{\n" +
            $"public enum InputTag\n" +
            $"{{\n" +
            $"{tags}\n" +
            $"}}\n" +
            $"\n" +
            $"public enum InputGrup\n" +
            $"{{\n" +
            $"{grups}\n" +
            $"}}\n" +
            $"}}\n";

        CodeGenerator.Generate
            (code, EnigmaticData.GetFullPath(EnigmaticData.inputStorege), "Inputs");

        Debug.Log($"Inputs tag and grup name saved by " +
            $"{EnigmaticData.GetFullPath(EnigmaticData.inputStorege)}");
    }

    //from https://discussions.unity.com/t/how-to-create-a-scriptableobject-file-with-specific-path-through-code/239303
    private KFInputMapGrupProvider CreateKFInputGrupProvider(KFInputGrup grup)
    {
        if(Directory.Exists(s_ProvidersPath) == false)
            Directory.CreateDirectory(s_ProvidersPath);

        KFInputMapGrupProvider provider = CreateInstance<KFInputMapGrupProvider>();

        string path = $"Assets/Resources/Enigmatic/KFInput/Providers/{grup.Name}Provider.asset";

        AssetDatabase.CreateAsset(provider, path);
        
        return provider;
    }

    ///         [
    ///             Name: "tagX",
    ///             Value: "axis",
    ///             PosetiveButton: "button",
    ///             NegativeButton: "button",
    ///         ],
}

public class EditorKFInput
{
    public string Tag { get; set; }
    public string Type { get; set; }
    public string Device { get; set; }

    public float Gravity { get; set; }
    public float Dead { get; set; }
    public float Sensitivity { get; set; }

    public EditorKFAxis AxisX { get; set; }
    public EditorKFAxis AxisY { get; set; }

    public string Button { get; set; }

    public EditorKFInput(string tag, string type, string device, EditorKFAxis axisX, EditorKFAxis axisY, string button)
    {
        Tag = tag;

        Type = type;
        Device = device;

        Gravity = 3;
        Dead = 0.001f;
        Sensitivity = 3;

        AxisX = axisX;
        AxisY = axisY;

        Button = button;
    }

    public EditorKFInput(string tag)
    {
        Tag = tag;

        Type = "None";
        Device = "None";

        Gravity = 3;
        Dead = 0.001f;
        Sensitivity = 3;

        AxisX = new EditorKFAxis();
        AxisY = new EditorKFAxis();

        Button = "None";
    }

}

public class EditorKFAxis
{
    public string Value;
    public string PosetiveButton;
    public string NegativeButton;

    public EditorKFAxis(string value, string posetiveButton, string negativeButton)
    {
        Value = value;
        PosetiveButton = posetiveButton;
        NegativeButton = negativeButton;
    }

    public EditorKFAxis()
    {
        Value = "None";
        PosetiveButton = "None";
        NegativeButton = "None";
    }
}

public class KFInputGrup
{
    public string Name;

    private List<EditorKFInput> m_KFInputs = new List<EditorKFInput>();

    public EditorKFInput[] KFInputs => m_KFInputs.ToArray();

    public KFInputGrup(string name)
    {
        Name = name;
    }

    public void Add(EditorKFInput newEditorKFInput)
    {
        m_KFInputs.Add(newEditorKFInput);
    }

    public void Remove(EditorKFInput editorKFInput)
    {
        m_KFInputs.Remove(editorKFInput);
    }

    public void Clear() => m_KFInputs.Clear();
}

#endif