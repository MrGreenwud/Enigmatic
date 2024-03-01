using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EngineUtitlity.SearchedWindow;

#if UNITY_EDITOR
public class InputEditorWindow : EditorWindow
{
    private List<KFInput> m_KFInputs = new List<KFInput>();
    private KFInput m_SelectionKFInput;

    private KFInput m_TempInput = new KFInput("");

    private Vector2 m_ScrollPosition;
    private static bool s_IsAutoSave;

    private bool m_IsOpenAxisX;
    private bool m_IsOpenAxisY;

    public static void Open()
    {
        InputEditorWindow window = GetWindow<InputEditorWindow>();
        window.titleContent = new GUIContent("Input Editor");
        window.minSize = new Vector2(797, 490);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
        {
            if (GUILayout.Button("Save", GUILayout.Width(75))) { }
            if (GUILayout.Button("Apply", GUILayout.Width(75))) { }

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
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(250),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
                {
                    GUILayout.Label("Inputs", EditorStyles.boldLabel);

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        m_KFInputs.Add(new KFInput("New Input"));

                    EditorGUI.BeginDisabledGroup(m_SelectionKFInput == null);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        m_KFInputs.Remove(m_SelectionKFInput);
                        m_SelectionKFInput = null;
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

                    DrowInputList();

                    GUILayout.EndScrollView();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
                {
                    if (GUILayout.Button("Clear"))
                        m_KFInputs.Clear();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            ///Settings 
            EditorGUILayout.BeginVertical("box",
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
                {
                    GUILayout.Label("Settings", EditorStyles.boldLabel);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical("box",
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    if (m_SelectionKFInput != null)
                    {
                        m_TempInput.Tag = EditorGUILayout.TextField("Tag:", m_TempInput.Tag);

                        EditorGUILayout.Space(3);

                        EditorGUILayout.BeginHorizontal();
                        {
                            InputEditorUtils.DrowSelectableElement
                                ("Type", TreeTags.InputType.ToString(), "type", OnSelectValue, m_SelectionKFInput.Type);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space(3);

                        EditorGUILayout.BeginHorizontal();
                        {
                            InputEditorUtils.DrowSelectableElement
                                ("Device", TreeTags.Device.ToString(), "device", OnSelectValue, m_SelectionKFInput.Device);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space(3);

                        EditorGUILayout.BeginVertical("box");
                        {
                            if (m_TempInput.Device != "None")
                            {
                                if (SearchedTree.GetAncestor(m_TempInput.Type)[0] == "Button")
                                {
                                    InputEditorUtils.DrowButton("Button", OnSelectValue,
                                        m_TempInput.Button, m_TempInput.Device, "button");
                                }
                                else if (SearchedTree.GetAncestor(m_TempInput.Type)[0] == "Axis")
                                {
                                    if (SearchedTree.GetAncestor(m_TempInput.Type)[1] == "Float")
                                    {
                                        InputEditorUtils.DrowAxis("Axis", m_TempInput.AxisX, m_TempInput.Device,
                                            OnSelectValue, "AxisX", ref m_IsOpenAxisX);
                                    }
                                    else
                                    {
                                        InputEditorUtils.DrowAxis("AxisX", m_TempInput.AxisX, m_TempInput.Device,
                                            OnSelectValue, "AxisX", ref m_IsOpenAxisX);

                                        EditorGUILayout.Space(3);

                                        InputEditorUtils.DrowAxis("AxisY", m_TempInput.AxisY, m_TempInput.Device,
                                            OnSelectValue, "AxisY", ref m_IsOpenAxisY);
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();

                        ApplySettings();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrowInputList()
    {
        for (int i = 0; i < m_KFInputs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(i == 0);
                {
                    if (GUILayout.Button("↑", GUILayout.Width(25)))
                        MoveUp(m_KFInputs[i]);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(i == m_KFInputs.Count - 1);
                {
                    if (GUILayout.Button("↓", GUILayout.Width(25)))
                        MoveDown(m_KFInputs[i]);
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.Space(3);

                if (GUILayout.Button(m_KFInputs[i].Tag, EditorStyles.miniButtonMid))
                {
                    m_SelectionKFInput = m_KFInputs[i];
                    
                    GUI.FocusControl(null);

                    m_TempInput.Tag = m_SelectionKFInput.Tag;
                    m_TempInput.Type = m_SelectionKFInput.Type;
                    m_TempInput.Device = m_SelectionKFInput.Device;
                    m_TempInput.Button = m_SelectionKFInput.Button;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void ApplySettings()
    {
        if (m_SelectionKFInput == null)
            return;

        m_SelectionKFInput.Tag = m_TempInput.Tag;
        m_SelectionKFInput.Type = m_TempInput.Type;
        m_SelectionKFInput.Device = m_TempInput.Device;
    }

    private void MoveUp(KFInput kFInput) => Move(kFInput, -1);

    private void MoveDown(KFInput kFInput) => Move(kFInput, +1);

    private void OnSelectValue(SearchedTree tree, string senderCode)
    {
        string value = InputEditorUtils.GetSearchedTree(tree);

        if(senderCode == "type")
            m_TempInput.Type = value;
        else if(senderCode == "device")
            m_TempInput.Device = value;
        else if(senderCode == "button")
            m_TempInput.Button = value;
    }

    private void Move(KFInput kFInput, int direction)
    {
        if (direction == 0)
            return;
        else if (direction > 1)
            direction = 1;
        else if (direction < -1)
            direction = -1;

        List<KFInput> kFInputs = new List<KFInput>();

        for (int i = 0; i < m_KFInputs.Count; i++)
            kFInputs.Add(null);

        for (int i = 0; i < m_KFInputs.Count; i++)
        {
            if (m_KFInputs[i] == kFInput)
            {
                kFInputs[i + direction] = kFInput;
                kFInputs[i] = m_KFInputs[i + direction];

                break;
            }
        }

        for (int i = 0; i < m_KFInputs.Count; i++)
            if (kFInputs[i] == null)
                kFInputs[i] = m_KFInputs[i];

        m_KFInputs = kFInputs;

    }

///         [
///             Name: "tagX",
///             Value: "axis",
///             PosetiveButton: "button",
///             NegativeButton: "button",
///         ],
}

public class Axis
{
    public string Value;
    public string PosetiveButton;
    public string NegativeButton;

    public Axis(string value, string posetiveButton, string negativeButton)
    {
        Value= value;
        PosetiveButton = posetiveButton;
        NegativeButton = negativeButton;
    }

    public Axis() 
    {
        Value = "None";
        PosetiveButton = "None";
        NegativeButton = "None";
    }

}

public class KFInput
{
    public string Tag { get; set; }
    public string Type { get; set; }
    public string Device { get; set; }
    public Axis AxisX { get; set; }
    public Axis AxisY { get; set; }
    public string Button { get; set; }

    public KFInput(string tag, string type, string device, Axis axisX, Axis axisY, string button)
    {
        Tag = tag;
        Type = type;
        Device = device;
        AxisX = axisX;
        AxisY = axisY;
        Button = button;
    }

    public KFInput(string tag)
    {
        Tag = tag;

        Type = "None";
        Device = "None";
        AxisX = new Axis();
        AxisY = new Axis();
        Button = "None";
    }
}

#endif