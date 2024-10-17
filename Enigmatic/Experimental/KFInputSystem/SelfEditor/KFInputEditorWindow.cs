using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Enigmatic.Core;
using Enigmatic.Experimental.Window.Editor;
using Enigmatic.Experimental.SearchedWindowUtility;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    internal class KFInputEditorWindow : EnigmaticWindow
    {
        private KFInputEditor m_kFInputEditor;

        private static Device sm_Device = Device.KeyboardAndMouse;

        private string[] m_DeviceNames = new string[]
        {
            "Keyboard and Mouse",
            "Joystick",
            "Mobile"
        };

        private Vector2 m_CoumSize;
        private Vector2 m_InputMapScrollPosition;
        private Vector2 m_InputScrollPosition;

        private bool m_IsXAxisExpanded = true;
        private bool m_IsYAxisExpanded = true;

        [MenuItem("Tools/Enigmatic/KFInput")]
        public static void Open()
        {
            KFInputEditorWindow window = GetWindow<KFInputEditorWindow>();
            window.titleContent = new GUIContent("KFInput Editor");

            //window.maxSize = new Vector2(800, 800);
            window.minSize = new Vector2(800, 600);

            window.Show();
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            m_CoumSize = new Vector2(position.width / 3 - 6, position.height - 32);

            m_kFInputEditor = new KFInputEditor();

            m_kFInputEditor.OnSelectedElement += SelectElement;
            m_kFInputEditor.OnAddedElement += SelectElement;
            m_kFInputEditor.OnRemovedElement += SelectElement;

            m_kFInputEditor.LoadMap();
        }

        protected override void OnClose()
        {
            m_kFInputEditor.OnSelectedElement -= SelectElement;
            m_kFInputEditor.OnAddedElement -= SelectElement;
            m_kFInputEditor.OnRemovedElement -= SelectElement;
        }

        protected override void Draw()
        {
            EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.Padding(0), 
                EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.Height(position.height),
                EnigmaticGUILayout.ElementSpacing(0));
            {
                //ToolBar
                EnigmaticGUILayout.BeginHorizontal(EditorStyles.toolbar, EnigmaticGUILayout.Width(position.width),
                    EnigmaticGUILayout.Height(21), EnigmaticGUILayout.Padding(0));
                {
                    if (EnigmaticGUILayout.Button(GetDeviceName(sm_Device), new Vector2(140, 20), EditorStyles.toolbarDropDown))
                        SwichDevice();
                    
                    if(EnigmaticGUILayout.Button("Save", new Vector2(50, 20), EditorStyles.toolbarButton))
                        m_kFInputEditor.SaveMap();

                    if (EnigmaticGUILayout.Button("Apply", new Vector2(50, 20), EditorStyles.toolbarButton))
                        m_kFInputEditor.ApplyMap();
                }
                EnigmaticGUILayout.EndHorizontal();

                EnigmaticGUILayout.Space(2);

                //Contaners
                EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.Padding(4),
                    EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.Height(position.height - 20));
                {
                    GUIStyle titleLable = new GUIStyle(EditorStyles.miniBoldLabel);
                    titleLable.fontSize = 12;

                    //InputMaps
                    EnigmaticGUILayout.BeginVertical(EnigmaticStyles.columBackground, EnigmaticGUILayout.Width(m_CoumSize.x),
                        EnigmaticGUILayout.Height(m_CoumSize.y), EnigmaticGUILayout.Padding(0), EnigmaticGUILayout.ElementSpacing(0));
                    {
                        DrawingUtilities.DrawTitle("Maps", m_CoumSize.x, true, m_CoumSize.x - 97, m_kFInputEditor.AddInputMap, m_kFInputEditor.RemoveSelectionInputMap);

                        Rect rect = EnigmaticGUILayout.GetActiveGrup().Rect;

                        rect.y += 21;
                        rect.width = m_CoumSize.x;
                        rect.height = m_CoumSize.y - 21;

                        EnigmaticGUILayout.BeginVerticalScrollView(rect, rect, m_InputMapScrollPosition, EnigmaticGUILayout.Padding(0),
                            EnigmaticGUILayout.ElementSpacing(-1));
                        {
                            m_kFInputEditor.DrawInputMaps();
                        }
                        m_InputMapScrollPosition = EnigmaticGUILayout.EndScrollView(Repaint);
                    }
                    EnigmaticGUILayout.EndVertical();

                    //Inputs
                    EnigmaticGUILayout.BeginVertical(EnigmaticStyles.columBackground, EnigmaticGUILayout.Width(m_CoumSize.x),
                        EnigmaticGUILayout.Height(m_CoumSize.y), EnigmaticGUILayout.Padding(0),
                        EnigmaticGUILayout.ExpandHeight(true));
                    {
                        DrawingUtilities.DrawTitle("Inputs", m_CoumSize.x, true, m_CoumSize.x - 100, 
                            m_kFInputEditor.AddInput, m_kFInputEditor.RemoveSelectionInput);

                        Rect rect = EnigmaticGUILayout.GetActiveGrup().Rect;

                        rect.y += 21;
                        rect.width = m_CoumSize.x;
                        rect.height = m_CoumSize.y - 21;

                        EnigmaticGUILayout.BeginVerticalScrollView(rect, rect, m_InputScrollPosition, 
                            EnigmaticGUILayout.Padding(0),EnigmaticGUILayout.ElementSpacing(-1));
                        {
                            m_kFInputEditor.DrawInputs();
                        }
                        m_InputScrollPosition = EnigmaticGUILayout.EndScrollView(Repaint);
                    }
                    EnigmaticGUILayout.EndVertical();

                    //Settings
                    EnigmaticGUILayout.BeginVertical(EnigmaticStyles.columBackground, EnigmaticGUILayout.Width(m_CoumSize.x + 3),
                        EnigmaticGUILayout.Height(m_CoumSize.y), EnigmaticGUILayout.Padding(0));
                    {
                        DrawingUtilities.DrawTitle("Settings", m_CoumSize.x + 3, false);
                        DrawSetting();
                    }
                    EnigmaticGUILayout.EndVertical();
                }
                EnigmaticGUILayout.EndHorizontal();
            }
            EnigmaticGUILayout.EndVertical();
        }

        private void SelectElement()
        {
            GUI.FocusControl(null);
            Repaint();
        }

        private void SwichDevice()
        {
            GenericMenu menu = new GenericMenu();

            foreach(string device in m_DeviceNames)
            {
                bool isSelection = sm_Device == GetDevice(device);
                menu.AddItem(new GUIContent(device), isSelection, SelectedDevice, device);
            }

            menu.ShowAsContext();
        }

        private void SelectedDevice(object data)
        {
            string device = (string)data;
            sm_Device = GetDevice(device);

            Repaint();
        }

        private void DrawSetting()
        {
            if (m_kFInputEditor.SelectionMap == null)
                return;

            EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.Width(m_CoumSize.x),
                        EnigmaticGUILayout.Height(m_CoumSize.y - 21), EnigmaticGUILayout.Padding(5),
                        EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ElementSpacing(8));
            {
                float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;
                m_kFInputEditor.SelectionMap.Name = EnigmaticGUILayout.TextField("Map Name:", m_kFInputEditor.SelectionMap.Name, width);

                EnigmaticGUILayout.Image(new Vector2(width, 1), EnigmaticStyles.columBackground);

                DrawInputSettings();
            }
            EnigmaticGUILayout.EndVertical();
        }

        private void DrawInputSettings()
        {
            if (m_kFInputEditor.SelectedInput == null)
                return;

            KFInput input = m_kFInputEditor.SelectedInput;

            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;

            input.Tag = EnigmaticGUILayout.TextField("Tag:", input.Tag, width);

            SearchedTreeListProvider inputTypeProvider = SearchedTreeListProvider.Create("KFInput_System", "Input_Type", "type");
            inputTypeProvider.OnSelected += OnSelected;
            
            SearchedTreeUtility.DrawSelectionTree("Type:", input.Type, width, inputTypeProvider);

            KFInputSettings inputSettings = input.GetInputSettings(sm_Device);

            EnigmaticGUILayout.BeginDisabledGroup(sm_Device != Device.KeyboardAndMouse);
            {
                SearchedTreeListProvider inputDeviceProvider = SearchedTreeListProvider.Create("KFInput_System", "Device", "device");
                inputDeviceProvider.OnSelected += OnSelected;

                SearchedTreeUtility.DrawSelectionTree("Device:", inputSettings.Device, width, inputDeviceProvider);
            }
            EnigmaticGUILayout.EndDisabledGroup();

            EnigmaticGUILayout.Image(new Vector2(width, 1), EnigmaticStyles.columBackground);

            if (input.Type == "None" || inputSettings.Device == "None")
                return;

            if (sm_Device != Device.Mobile && SearchedTreeUtility.DeCompileTree(input.Type, 0) == "Axis")
            {
                inputSettings.Gravity = EnigmaticGUILayout.FloatField("Gravity:", inputSettings.Gravity, width);
                inputSettings.Dead = EnigmaticGUILayout.FloatField("Dead:", inputSettings.Dead, width);
                inputSettings.Sensitivity = EnigmaticGUILayout.FloatField("Sensitivity:", inputSettings.Sensitivity, width);

                EnigmaticGUILayout.Image(new Vector2(width, 1), EnigmaticStyles.columBackground);
            }

            if(SearchedTreeUtility.DeCompileTree(input.Type, 0) == "Button")
                DrawButtonSettings(input.Type, inputSettings);
            else if(SearchedTreeUtility.DeCompileTree(input.Type, 0) == "Axis")
                DrawAxisSettings(input.Type, inputSettings);

            EnigmaticGUILayout.Image(new Vector2(width, 1), EnigmaticStyles.columBackground);
        }

        private void DrawButtonSettings(string inputType, KFInputSettings inputSettings)
        {
            if (inputSettings.Device == "Keyboard")
                DrawKeyboardButtonSettings(inputSettings);
            else if(inputSettings.Device == "Mouse")
                DrawMouseButtonSettings(inputSettings);
            else if(inputSettings.Device == "Joystick")
                DrawJoysticButtonSettings(inputSettings);
            else if(inputSettings.Device == "Mobile")
                DrawMobileButtonSettings(inputSettings);
        }

        private void DrawAxisSettings(string inputType, KFInputSettings inputSettings)
        {
            if (inputSettings.Device == "Keyboard")
                DrawKeyboardAxisSettings(inputType, inputSettings);
            else if (inputSettings.Device == "Mouse")
                DrawMouseAxisSettings(inputType, inputSettings);
            else if (inputSettings.Device == "Joystick")
                DrawJoysticAxisSettings(inputType, inputSettings);
            else if (inputSettings.Device == "Mobile")
                DrawMobileButtonSettings(inputSettings);
        }

        private void DrawKeyboardButtonSettings(KFInputSettings inputSettings)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;

            SearchedTreeListProvider inputButtonProvider = SearchedTreeListProvider
                .Create("KFInput_System", "KeyboardButton", "keyboardButton");
            
            inputButtonProvider.OnSelected += OnSelected;

            SearchedTreeUtility.DrawSelectionTree("Button:", inputSettings.Button, width, inputButtonProvider);
        }

        private void DrawMouseButtonSettings(KFInputSettings inputSettings)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;

            SearchedTreeListProvider inputButtonProvider = SearchedTreeListProvider
                .Create("KFInput_System", "MouseButton", "mouseButton");

            inputButtonProvider.OnSelected += OnSelected;

            SearchedTreeUtility.DrawSelectionTree("Button:", inputSettings.Button, width, inputButtonProvider);
        }

        private void DrawJoysticButtonSettings(KFInputSettings inputSettings)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;

            SearchedTreeListProvider inputButtonProvider = SearchedTreeListProvider
                .Create("KFInput_System", "JoystickButton", "joystickButton");

            inputButtonProvider.OnSelected += OnSelected;

            SearchedTreeUtility.DrawSelectionTree("Button:", inputSettings.Button, width, inputButtonProvider);
        }

        private void DrawMobileButtonSettings(KFInputSettings inputSettings)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;

            inputSettings.ControlGUID = EnigmaticGUILayout.TextField("ControlGUID", inputSettings.ControlGUID, width);

            if (EnigmaticGUILayout.Button("Generate GUID", new Vector2(width, 20)))
            {
                GUI.FocusControl(null);
                inputSettings.ControlGUID = Guid.NewGuid().ToString();
            }
        }

        private void DrawKeyboardAxisSettings(string inputType, KFInputSettings inputSettings)
        {
            if(SearchedTreeUtility.DeCompileTree(inputType, 1) == "Vector2")
            {
                DrawKeyboardAxis1DSettings("Axis X", inputSettings.HorizontalAxis, ref m_IsXAxisExpanded);
                EnigmaticGUILayout.Space(-17);
                DrawKeyboardAxis1DSettings("Axis Y", inputSettings.VerticalAxis, ref m_IsYAxisExpanded);
            }
            else
            {
                DrawKeyboardAxis1DSettings("Axis X", inputSettings.HorizontalAxis, ref m_IsXAxisExpanded);
            }
        }

        private void DrawKeyboardAxis1DSettings(string axisName, KFAxisSettings axis, ref bool isExpanded)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;

            SearchedTreeListProvider inputButtonPosetiveProvider = SearchedTreeListProvider
                .Create("KFInput_System", "KeyboardButton", $"keyboardPosetive{axisName.Replace(" ", string.Empty)}Button");

            SearchedTreeListProvider inputButtonNegativeProvider = SearchedTreeListProvider
                .Create("KFInput_System", "KeyboardButton", $"keyboardNegative{axisName.Replace(" ", string.Empty)}Button");

            inputButtonPosetiveProvider.OnSelected += OnSelected;
            inputButtonNegativeProvider.OnSelected += OnSelected;

            if (EnigmaticGUILayout.BeginFoldout(ref isExpanded, axisName, Repaint, width))
            {
                SearchedTreeUtility.DrawSelectionTree("PosetiveButton:", axis.PosetiveButton, width, inputButtonPosetiveProvider);
                SearchedTreeUtility.DrawSelectionTree("NegativeButton:", axis.NegativeButton, width, inputButtonNegativeProvider);
            }
            EnigmaticGUILayout.EndFoldout(isExpanded);
        }

        private void DrawMouseAxisSettings(string inputType, KFInputSettings inputSettings)
        {
            if (SearchedTreeUtility.DeCompileTree(inputType, 1) == "Vector2")
            {
                DrawMouseAxis1DSettings("Axis X", inputSettings.HorizontalAxis);
                DrawMouseAxis1DSettings("Axis Y", inputSettings.VerticalAxis);
            }
            else
            {
                DrawMouseAxis1DSettings("Axis X", inputSettings.HorizontalAxis);
            }
        }

        private void DrawMouseAxis1DSettings(string axisName, KFAxisSettings axis)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;

            SearchedTreeListProvider inputAxisProvider = SearchedTreeListProvider
                .Create("KFInput_System", "MouseAxis", $"mouse{axisName.Replace(" ", string.Empty)}");

            inputAxisProvider.OnSelected += OnSelected;

            SearchedTreeUtility.DrawSelectionTree(axisName, axis.Axis, width, inputAxisProvider);
        }

        private void DrawJoysticAxisSettings(string inputType, KFInputSettings inputSettings)
        {
            if (SearchedTreeUtility.DeCompileTree(inputType, 1) == "Vector2")
            {
                DrawJoysticAxis1DSettings("Axis X", inputSettings.HorizontalAxis, ref m_IsXAxisExpanded);
                EnigmaticGUILayout.Space(-17);
                DrawJoysticAxis1DSettings("Axis Y", inputSettings.VerticalAxis, ref m_IsYAxisExpanded);
            }
            else
            {
                DrawJoysticAxis1DSettings("Axis X", inputSettings.HorizontalAxis, ref m_IsXAxisExpanded);
            }//
        }

        private void DrawJoysticAxis1DSettings(string axisName, KFAxisSettings axis, ref bool isExpanded)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;

            SearchedTreeListProvider axisTypeProvider = SearchedTreeListProvider
                .Create("KFInput_System", "AxisType", $"joystickAxisType{axisName.Replace(" ", string.Empty)}");

            SearchedTreeListProvider inputAxisProvider = SearchedTreeListProvider
                .Create("KFInput_System", "JoystickAxis", $"joystick{axisName.Replace(" ", string.Empty)}");

            SearchedTreeListProvider inputButtonPosetiveProvider = SearchedTreeListProvider
                .Create("KFInput_System", "JoystickButton", $"joystickPosetive{axisName.Replace(" ", string.Empty)}");

            SearchedTreeListProvider inputButtonNegativeProvider = SearchedTreeListProvider
                .Create("KFInput_System", "JoystickButton", $"joystickNegative{axisName.Replace(" ", string.Empty)}");

            axisTypeProvider.OnSelected += OnSelected;
            inputAxisProvider.OnSelected += OnSelected;
            inputButtonPosetiveProvider.OnSelected += OnSelected;
            inputButtonNegativeProvider.OnSelected += OnSelected;

            if (EnigmaticGUILayout.BeginFoldout(ref isExpanded, axisName, Repaint, width))
            {
                SearchedTreeUtility.DrawSelectionTree("Axis Type", axis.Type, width, axisTypeProvider);

                EnigmaticGUILayout.Space(5);

                EnigmaticGUILayout.BeginDisabledGroup(axis.Type == "Buttons");
                {
                    SearchedTreeUtility.DrawSelectionTree("Axis", axis.Axis, width, inputAxisProvider);
                }
                EnigmaticGUILayout.EndDisabledGroup();

                EnigmaticGUILayout.Space(2);

                EnigmaticGUILayout.BeginDisabledGroup(axis.Type == "Value");
                {
                    SearchedTreeUtility.DrawSelectionTree("PosetiveButton:", axis.PosetiveButton, width, inputButtonPosetiveProvider);
                    SearchedTreeUtility.DrawSelectionTree("NegativeButton:", axis.NegativeButton, width, inputButtonNegativeProvider);
                }
                EnigmaticGUILayout.EndDisabledGroup();
            }
            EnigmaticGUILayout.EndFoldout(isExpanded);
        }

        private void OnSelected(string guid, List<string> tree)
        {
            KFInput input = m_kFInputEditor.SelectedInput;
            KFInputSettings inputSettings = input.GetInputSettings(sm_Device);

            string value = SearchedTreeUtility.CompileTree(tree);

            switch (guid)
            {
                case "type":
                    input.Type = value;
                    break;
                case "device":
                    inputSettings.Device = value;
                    break;
                case "keyboardButton":
                case "mouseButton":
                    input.GetInputSettings(Device.KeyboardAndMouse).Button = value;
                    break;
                case "keyboardPosetiveAxisXButton":
                    input.GetInputSettings(Device.KeyboardAndMouse).HorizontalAxis.PosetiveButton =
                        SearchedTreeUtility.DeCompileTree(value, 1);
                    break;
                case "keyboardPosetiveAxisYButton":
                    input.GetInputSettings(Device.KeyboardAndMouse).VerticalAxis.PosetiveButton =
                        SearchedTreeUtility.DeCompileTree(value, 1);
                    break;
                case "keyboardNegativeAxisXButton":
                    input.GetInputSettings(Device.KeyboardAndMouse).HorizontalAxis.NegativeButton =
                        SearchedTreeUtility.DeCompileTree(value, 1);
                    break;
                case "keyboardNegativeAxisYButton":
                    input.GetInputSettings(Device.KeyboardAndMouse).VerticalAxis.NegativeButton =
                        SearchedTreeUtility.DeCompileTree(value, 1);
                    break;
                case "joystickButton":
                    input.GetInputSettings(Device.Joystick).Button = value;
                    break;
                case "joystickAxisTypeAxisX":
                    input.GetInputSettings(Device.Joystick).HorizontalAxis.Type = value;
                    break;
                case "joystickAxisTypeAxisY":
                    input.GetInputSettings(Device.Joystick).VerticalAxis.Type = value;
                    break;
                case "joystickPosetiveAxisX":
                    input.GetInputSettings(Device.Joystick).HorizontalAxis.PosetiveButton = value;
                    break;
                case "joystickPosetiveAxisY":
                    input.GetInputSettings(Device.Joystick).VerticalAxis.PosetiveButton = value;
                    break;
                case "joystickNegativeAxisX":
                    input.GetInputSettings(Device.Joystick).HorizontalAxis.NegativeButton = value;
                    break;
                case "joystickNegativeAxisY":
                    input.GetInputSettings(Device.Joystick).VerticalAxis.NegativeButton = value;
                    break;
                case "joystickAxisX":
                    input.GetInputSettings(Device.Joystick).HorizontalAxis.Axis = value;
                    break;
                case "joystickAxisY":
                    input.GetInputSettings(Device.Joystick).VerticalAxis.Axis = value;
                    break;
                case "mouseAxisX":
                    input.GetInputSettings(Device.KeyboardAndMouse).HorizontalAxis.Axis = value;
                    break;
                case "mouseAxisY":
                    input.GetInputSettings(Device.KeyboardAndMouse).VerticalAxis.Axis = value;
                    break;
                default:
                    Debug.LogError("");
                    break;
            }

            Repaint();
        }

        private Device GetDevice(string name)
        {
            if (name == m_DeviceNames[0])
                return Device.KeyboardAndMouse;
            else if(name == m_DeviceNames[1])
                return Device.Joystick;
            else
                return Device.Mobile;
        }

        private string GetDeviceName(Device device)
        {
            int i = (int)device;
            return m_DeviceNames[i];
        }
    }
}
