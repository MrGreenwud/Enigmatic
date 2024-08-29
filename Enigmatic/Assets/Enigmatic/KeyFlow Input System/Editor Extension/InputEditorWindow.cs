using System;
using System.IO;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Enigmatic.Core;
using Enigmatic.Experemental.CodeLiteGen;
using Enigmatic.Experemental.SearchedWindowUtility;

#if UNITY_EDITOR

namespace Enigmatic.KFInputSystem.Editor
{
    public enum Device
    {
        Keyboard_and_Mouse,
        Joystick,
        Mobile
    }

    public class InputEditorWindow : EditorWindow
    {
        private string[] DeviceNames = new string[]
        {
            "Keyboard and Mouse",
            "Joystick",
            "Mobile"
        };

        private static Device s_Device;
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

        private void OnDisable()
        {
            if (s_IsAutoSave == true)
                Save();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
            {
                s_Device = (Device)EditorGUILayout.Popup((int)s_Device, DeviceNames, GUILayout.Width(150));

                if (GUILayout.Button("Save", GUILayout.Width(75)))
                    Save();

                if (GUILayout.Button("Apply", GUILayout.Width(75)))
                    Apply();

                ////797 - 540 = 257
                GUILayout.Space(390);

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
                        {
                            m_KFGrups.Add(new KFInputGrup("New Grup"));
                        }

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
                                m_SelectionGrup.Add();

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
                                    EditorGUI.BeginDisabledGroup(s_Device == Device.Joystick);
                                    {
                                        SearchedTreeListProvider provider = SearchedTreeListProvider.Create("KFInput_System", "Device", "device");
                                        provider.OnSelected += OnSelectValue;
                                        SearchedTreeUtility.DrawSelectionTree("Device", m_SelectionKFInput.GetInputSettings(s_Device).Device, provider);
                                    }
                                    EditorGUI.EndDisabledGroup();
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space(3);
                            }
                            EditorGUILayout.EndVertical();

                            EditorKFInputSettings settings = m_SelectionKFInput.GetInputSettings(s_Device);

                            EditorGUILayout.BeginVertical("box");
                            {
                                if (SearchedTreeUtility.DeCompileTree(m_SelectionKFInput.Type, 0) == "Axis")
                                {
                                    settings.Gravity = EditorGUILayout.FloatField("Gravity", settings.Gravity);
                                    settings.Dead = EditorGUILayout.FloatField("Dead", settings.Dead);
                                    settings.Sensitivity = EditorGUILayout.FloatField("Sensitivity", settings.Sensitivity);

                                    GUILayout.Space(10);
                                }
                            }
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical("box");
                            {
                                if (settings.Device != "None")
                                {
                                    if (SearchedTreeUtility.DeCompileTree(m_SelectionKFInput.Type, 0) == "Button")
                                    {
                                        SearchedTreeListProvider provider = SearchedTreeListProvider.Create("KFInput_System", $"{settings.Device}Button", "button");
                                        provider.OnSelected += OnSelectValue;
                                        SearchedTreeUtility.DrawSelectionTree("Button", settings.Button, provider);
                                    }
                                    else if (SearchedTreeUtility.DeCompileTree(m_SelectionKFInput.Type, 0) == "Axis")
                                    {
                                        if (settings.Device == "Keyboard")
                                            DrawAxisKeyboard("AxisX", settings.AxisXSettings, "AxisX");
                                        else
                                            DrawAxisJosticOrMouse("AxisX", settings.AxisXSettings, "AxisX");

                                        if (SearchedTreeUtility.DeCompileTree(m_SelectionKFInput.Type, 1) == "Vector2")
                                        {
                                            if (settings.Device == "Keyboard")
                                                DrawAxisKeyboard("AxisY", settings.AxisYSettings, "AxisY");
                                            else
                                                DrawAxisJosticOrMouse("AxisY", settings.AxisYSettings, "AxisY");
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

        private void Save()
        {
            //if (ChackValidate() == false)
            //    return;

            ComplianceInputMap();

            foreach (KFInputGrup grup in m_KFGrups)
                KFIMFile.Genetate(grup);

            KFIMFile.Save();
        }

        private void Apply()
        {
            GenerateInputTagsAndGrupsName();

            Queue<InputAxis> axisQueue = new Queue<InputAxis>();

            foreach (KFInputGrup grup in m_KFGrups)
            {
                KFInputMapProvider provider = CreateKFInputGrupProvider(grup);
                provider.GrupName = (InputGrup)Enum.Parse(typeof(InputGrup), grup.Name);

                foreach (EditorKFInput input in grup.KFInputs)
                {
                    InputAxis[] axes = CreateInputAxis(input, Device.Keyboard_and_Mouse);

                    foreach (InputAxis axis in axes)
                        axisQueue.Enqueue(axis);

                    provider.AddInput(input.Tag, input.Type);
                }

                foreach (EditorKFInput input in grup.KFInputs)
                {
                    InputAxis[] axes = CreateInputAxis(input, Device.Joystick);

                    foreach (InputAxis axis in axes)
                        axisQueue.Enqueue(axis);
                }

                EditorUtility.SetDirty(provider);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            UnityInputManager.Clear();

            while (axisQueue.Count > 0)
                UnityInputManager.AddAxis(axisQueue.Dequeue());
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

        private void DrawAxisKeyboard(string axisName, EditorKFAxisSettings axis, string senderUID)
        {
            SearchedTreeListProvider providerPosetive = SearchedTreeListProvider.Create("KFInput_System", $"KeyboardButton", $"{senderUID}Posetive");
            SearchedTreeListProvider providerNegative = SearchedTreeListProvider.Create("KFInput_System", $"KeyboardButton", $"{senderUID}Negative");

            providerPosetive.OnSelected += OnSelectValue;
            providerNegative.OnSelected += OnSelectValue;

            DrawAxis(axisName, axis, "Keyboard", providerPosetive, providerNegative);
        }

        private void DrawAxisJosticOrMouse(string axisName, EditorKFAxisSettings axis, string senderUID)
        {
            EditorKFInputSettings settings = m_SelectionKFInput.GetInputSettings(s_Device);

            SearchedTreeListProvider provider =
                SearchedTreeListProvider.Create("KFInput_System", $"{settings.Device}Axis", $"{senderUID}");

            provider.OnSelected += OnSelectValue;

            DrawAxis(axisName, axis, m_SelectionKFInput.GetInputSettings(s_Device).Device, provider);
        }

        private void DrawAxis(string name, EditorKFAxisSettings axis, string device, params SearchedTreeListProvider[] providers)
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

            EditorKFInputSettings settings = m_SelectionKFInput.GetInputSettings(s_Device);

            switch (senderUID)
            {
                case "type":
                    m_SelectionKFInput.Type = value;
                    break;
                case "device":
                    settings.Device = value;
                    break;
                case "button":
                    settings.Button = value;
                    break;
                case "AxisXPosetive":
                    settings.AxisXSettings.PosetiveButton = value;
                    break;
                case "AxisXNegative":
                    settings.AxisXSettings.NegativeButton = value;
                    break;
                case "AxisYPosetive":
                    settings.AxisYSettings.PosetiveButton = value;
                    break;
                case "AxisYNegative":
                    settings.AxisYSettings.NegativeButton = value;
                    break;
                case "AxisX":
                    settings.AxisXSettings.Value = value;
                    break;
                case "AxisY":
                    settings.AxisYSettings.Value = value;
                    break;
            }
        }

        private InputAxis[] CreateInputAxis(EditorKFInput input, Device device)
        {
            List<InputAxis> axes = new List<InputAxis>();

            EditorKFInputSettings settings = input.GetInputSettings(device);

            if (SearchedTreeUtility.DeCompileTree(input.Type, 0) == "Button")
            {
                InputAxis axis = new InputAxis(input.Tag, input.Type, settings);
                axes.Add(axis);
            }
            else
            {
                if (SearchedTreeUtility.DeCompileTree(input.Type, 1) == "Vector2")
                {
                    InputAxis axisX = new InputAxis($"{input.Tag} X", input.Type, settings, settings.AxisXSettings);
                    InputAxis axisY = new InputAxis($"{input.Tag} Y", input.Type, settings, settings.AxisYSettings);

                    axes.Add(axisX);
                    axes.Add(axisY);
                }
                else
                {
                    InputAxis axis = new InputAxis($"{input.Tag}", input.Type, settings, settings.AxisXSettings);
                    axes.Add(axis);
                }
            }

            return axes.ToArray();
        }

        private void GenerateInputTagsAndGrupsName()
        {
            string tags = string.Empty;
            string grups = string.Empty;

            foreach (KFInputGrup grup in m_KFGrups)
            {
                grups += $"{grup.Name},\n";

                foreach (EditorKFInput input in grup.KFInputs)
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
        private KFInputMapProvider CreateKFInputGrupProvider(KFInputGrup grup)
        {
            if (Directory.Exists(EnigmaticData.GetFullPath(EnigmaticData.inputProviders)) == false)
                Directory.CreateDirectory(EnigmaticData.GetFullPath(EnigmaticData.inputProviders));

            KFInputMapProvider provider = CreateInstance<KFInputMapProvider>();

            string path = $"{EnigmaticData.GetUnityPath(EnigmaticData.inputProviders)}/{grup.Name}Provider.asset";

            AssetDatabase.CreateAsset(provider, path);
            
            return provider;
        }

        private void ComplianceInputMap()
        {
            Queue<string> pathInputMaps = Directory.GetFiles(EnigmaticData.GetFullPath(EnigmaticData.inputMaps), "*.kfim").ToQueue();
            Queue<string> pathInputMapsProviders = Directory.GetFiles(EnigmaticData.GetFullPath(EnigmaticData.inputProviders), "*.asset").ToQueue();

            foreach (KFInputGrup grup in m_KFGrups)
            {
                foreach (string path in pathInputMaps)
                {
                    if (Path.GetFileNameWithoutExtension(path) == grup.Name)
                        pathInputMaps.Dequeue();
                }

                foreach (string path in pathInputMapsProviders)
                {
                    if (Path.GetFileNameWithoutExtension(path) == $"{grup.Name}Provider")
                        pathInputMapsProviders.Dequeue();
                }
            }

            while (pathInputMaps.Count > 0)
                File.Delete(pathInputMaps.Dequeue());

            while (pathInputMapsProviders.Count > 0)
                File.Delete(pathInputMapsProviders.Dequeue());
        }
    }
}

#endif