using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Enigmatic.Core;
using Enigmatic.Experemental.ENIX;
using NodeEditor;
using NodesTest;

//from https://forum.unity.com/threads/simple-node-editor.189230/
//from https://martinecker.com/martincodes/unity-editor-window-zooming/ "ZOOM"

public enum Direction
{
    VerticalUp,
    VerticalDown,
    HorizontalRight,
    HorizontalLeft,
}

public static class OpenAssetHendler
{
    [OnOpenAsset]
    public static bool OpenAsset(int instaceID, int line)
    {
        var asset = EditorUtility.InstanceIDToObject(instaceID);

        if (asset is NodeGraphContanerBase contaner)
        {
            NodeEditorWindow.OpenWindow(contaner);
            return true;
        }

        return false;
    }
}

public class NodeEditorWindow : EditorWindow
{
    public static NodeEditorWindow ActiveWindow { get; private set; }
    public static NodeGraphEditor s_NodeGraphEditor { get; private set; }

    private NodeGraphContanerBase m_NodeGraphContaner;

    public static void OpenWindow(NodeGraphContanerBase contaner)
    {
        NodeEditorWindow window = GetWindow<NodeEditorWindow>();
        window.titleContent = new GUIContent("Graph Editor", EnigmaticStyles.nodeEditorWindowIcon.normal.background);

        //DropDownMenu.Show(window.position.position);

        ActiveWindow = window;

        GraphDataManager.LoadGraph(contaner, out NodeGraph graph, out NodeGraphEditor graphEditor);
        s_NodeGraphEditor = graphEditor;
        s_NodeGraphEditor.Init(graph);
        NodeGraphEditor.Open(s_NodeGraphEditor);
        ActiveWindow.m_NodeGraphContaner = contaner;

        NodeGraphInspectorWindow.OpenWindow();
    }

    public static float ZoomScale { get; private set; } = 1f;
    public Vector2 zoomCordsOrigin { get; private set; } = Vector2.zero;

    public Rect ZoomedArea { get; private set; }

    private void OnGUI()
    {
        if (NodeGraphEditor.Active != null)
        {
            ZoomedArea = new Rect(Vector2.zero, position.size);

            EditorGUI.BeginChangeCheck();

            NodeGraphEditor.Active.Run();

            if (EditorGUI.EndChangeCheck())
                NodeGraphInspectorWindow.ActiveWindow.Repaint();
        }

        EnigmaticGUILayout.BeginHorizontal(new Rect(1, 0, 0, 0), EditorStyles.toolbar, 0, EnigmaticGUILayout.Width(position.width),
            EnigmaticGUILayout.Height(30), EnigmaticGUILayout.Padding(0), EnigmaticGUILayout.ElementSpacing(6));
        {
            EnigmaticGUILayout.Space(3);

            if (EnigmaticGUILayout.Button("Save Graph", EditorStyles.toolbarButton))
            {
                if (s_NodeGraphEditor != null)
                    GraphDataManager.SaveGraph(s_NodeGraphEditor.NodeGraph, s_NodeGraphEditor, m_NodeGraphContaner);
            }

            if (NodeGraphEditor.Active != null && NodeGraphEditor.Active.IsSubGraph)
            {
                if (EnigmaticGUILayout.Button("Return To Root", EditorStyles.toolbarButton))
                    NodeGraphEditor.Open(s_NodeGraphEditor);
            }
        }
        EnigmaticGUILayout.EndHorizontal();
    }

    public static void CustomRepaint()
    {
        if (ActiveWindow == null)
            return;

        ActiveWindow.Repaint();
        Debug.Log("Re");
    }
}

public class DropDownMenu : EditorWindow
{
    private GUIStyle roundedStyle;

    public static void Show(Vector2 position)
    {
        DropDownMenu menu = CreateInstance<DropDownMenu>();
        menu.position = new Rect(position, new Vector2(200, 200));
        menu.ShowPopup();
    }

    private void OnEnable()
    {
        // Initialize the rounded style
        roundedStyle = new GUIStyle();
        roundedStyle.normal.background = MakeSimpleTexture(2, 2, new Color(0.8f, 0.8f, 0.8f, 1.0f));
        roundedStyle.border = new RectOffset(12, 12, 12, 12);
        roundedStyle.margin = new RectOffset(0, 0, 0, 0);
        roundedStyle.padding = new RectOffset(6, 6, 6, 6);
    }

    private Texture2D MakeSimpleTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    void OnGUI()
    {
        // Use the rounded style
        GUILayout.BeginVertical(roundedStyle, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Close"))
        {
            Close();
        }

        GUILayout.EndVertical();
    }
}

public enum PortType
{
    Input,
    Output
}

public class Port
{
    private Node m_Node;

    public Node Node 
    {
        get
        {
            if (m_Node == null)
            {
                if (Connector.IsSubGraph)
                    m_Node = NodeGraphEditor.ActiveConnectionManager.GetNode(Connector);
                else
                    m_Node = Connector.Node;
            }

            return m_Node;
        }
    }

    public string FieldName => Connector.FieldName;

    public Connector Connector { get; private set; }

    public Vector2 MidelPoint;
    public Rect Rect;

    public PortType PortType { get; private set; }
    public bool IsMultiConnection { get; private set; }

    //public Port(Node node, FieldInfo fieldInfo, PortType portType)
    //{
    //    Node = node;
    //    FieldName = fieldInfo.Name;
    //    PortType = portType;

    //    if(portType == PortType.Input)
    //    {
    //        if(fieldInfo.FieldType.IsArray || fieldInfo.FieldType.IsList())
    //            IsMultiConnection = true;
    //        else 
    //            IsMultiConnection = false;
    //    }
    //    else
    //    {
    //        IsMultiConnection = true;
    //    }
    //}

    //public Port(Node node, FieldInfo fieldInfo, PortType portType, bool isMultiConnection = true) 
    //{
    //    Node = node;
    //    FieldName = fieldInfo.Name;
    //    PortType = portType;

    //    IsMultiConnection = isMultiConnection;

    //    if(IsMultiConnection == true && portType == PortType.Input 
    //        && fieldInfo.FieldType.IsList() == false && fieldInfo.FieldType.IsArray == false)
    //    {
    //        Debug.LogError($"You cannot create a multiconnection port " +
    //            $"with type input using data type {fieldInfo.FieldType}, " +
    //            $"for this use List or an Array");

    //        IsMultiConnection = false;
    //    }
    //}

    //public Port(Node node, string name, Type fieldType, PortType portType)
    //{
    //    Node = node;
    //    FieldName = name;
    //    PortType = portType;

    //    if (portType == PortType.Input)
    //    {
    //        if (fieldType.IsArray || fieldType.IsList())
    //            IsMultiConnection = true;
    //        else
    //            IsMultiConnection = false;
    //    }
    //    else
    //    {
    //        IsMultiConnection = true;
    //    }
    //}

    //public Port(Node node, string name, Type fieldType, PortType portType, bool isMultiConnection = true)
    //{
    //    Node = node;
    //    FieldName = name;
    //    PortType = portType;

    //    IsMultiConnection = isMultiConnection;

    //    if (IsMultiConnection == true && portType == PortType.Input
    //        && fieldType.IsList() == false && fieldType.IsArray == false)
    //    {
    //        Debug.LogError($"You cannot create a multiconnection port " +
    //            $"with type input using data type {fieldType}, " +
    //            $"for this use List or an Array");

    //        IsMultiConnection = false;
    //    }
    //}

    public Port(Connector connector, PortType portType)
    {
        Connector = connector;

        FieldInfo fieldInfo;

        if (portType == PortType.Input)
            ConnectionManager.TryGetInputProperty(connector, out fieldInfo);
        else
            ConnectionManager.TryGetOutputProperty(connector, out fieldInfo);

        PortType = portType;

        if (portType == PortType.Input)
        {
            if (fieldInfo.FieldType.IsArray || fieldInfo.FieldType.IsList())
                IsMultiConnection = true;
            else
                IsMultiConnection = false;
        }
        else
        {
            IsMultiConnection = true;
        }
    }

    public Port(Connector connector)
    {
        Connector = connector;

        FieldInfo fieldInfo;

        if (ConnectionManager.TryGetInputProperty(connector, out fieldInfo))
            PortType = PortType.Input;
        else if (ConnectionManager.TryGetOutputProperty(connector, out fieldInfo))
            PortType = PortType.Output;

        if (PortType == PortType.Input)
        {
            if (fieldInfo.FieldType.IsArray || fieldInfo.FieldType.IsList())
                IsMultiConnection = true;
            else
                IsMultiConnection = false;
        }
        else
        {
            IsMultiConnection = true;
        }
    }

    public Port(Connector connector, PortType portType, bool isMultiConnection = true)
    {
        Connector = connector;

        FieldInfo fieldInfo;

        if (portType == PortType.Input)
            ConnectionManager.TryGetInputProperty(connector, out fieldInfo);
        else
            ConnectionManager.TryGetOutputProperty(connector, out fieldInfo);

        PortType = portType;

        IsMultiConnection = isMultiConnection;

        if (IsMultiConnection == true && portType == PortType.Input
            && fieldInfo.FieldType.IsList() == false && fieldInfo.FieldType.IsArray == false)
        {
            Debug.LogError($"You cannot create a multiconnection port " +
                $"with type input using data type {fieldInfo.FieldType}, " +
                $"for this use List or an Array");

            IsMultiConnection = false;
        }
    }

    public bool TryConnect(Port port, out Connection connection)
    {
        connection = new Connection();

        if (PortType == port.PortType)
            return false;

        Port output = null;
        Port input = null;

        if (PortType == PortType.Output)
        {
            output = this;
            input = port;
        }
        else
        {
            output = port;
            input = this;
        }

        ConnectionManager connectionManager = NodeGraphEditor.ActiveConnectionManager;

        Connector outputConnector = output.Connector;
        Connector inputConnector = input.Connector;

        bool canConnect = connectionManager.CanConnect(outputConnector, inputConnector);

        if (canConnect)
        {
            if (output.IsMultiConnection == false)
            {
                if (connectionManager.GetConnections(outputConnector).Length > 0)
                    connectionManager.RemoveConnection(outputConnector);
            }

            if (input.IsMultiConnection == false)
            {
                if (connectionManager.GetConnections(inputConnector).Length > 0)
                    connectionManager.RemoveConnection(inputConnector);
            }

            connection = new Connection(outputConnector, inputConnector);
        }

        return canConnect;
    }
}

[SerializebleObject]
public class Node : ScriptableObject
{
    [SerializebleProperty] public NodeGraph Graph;
    [SerializebleProperty] public string Name;

    public virtual void Init(NodeGraph graph)
    {
        Graph = graph;

        if (string.IsNullOrEmpty(Name))
            Name = "New Node";
    }
}

[SerializebleObject]
public class PortNode : Node { }

[SerializebleObject]
public class InputNode : PortNode
{
    public override void Init(NodeGraph graph)
    {
        base.Init(graph);

        if (string.IsNullOrEmpty(Name))
            Name = "Input Port";
    }
}

[SerializebleObject]
public class OutputNode : PortNode
{
    public override void Init(NodeGraph graph)
    {
        base.Init(graph);

        if (string.IsNullOrEmpty(Name))
            Name = "Output Port";
    }
}

[SerializebleObject]
public class ParameterNode : PortNode
{
    public override void Init(NodeGraph graph)
    {
        base.Init(graph);

        if (string.IsNullOrEmpty(Name))
            Name = "Parameter";
    }
}

[SerializebleObject]
public class SubGraphNode : Node
{
    [SerializebleProperty] private NodeGraph m_OwnGraph;

    public NodeGraph OwnGraph => m_OwnGraph;

    public override void Init(NodeGraph graph)
    {
        base.Init(graph);

        if (m_OwnGraph == null)
        {
            m_OwnGraph = new NodeGraph();
            m_OwnGraph.IsSubGraph = true;
        }

        m_OwnGraph.Init();
    }
}

[SerializebleObject]
public class ObservableProperty<T>
{
    [SerializebleProperty] private T m_Value;

    public T Value
    {
        get
        {
            return m_Value;
        }
        set
        {
            m_Value = value;
            ValueChanged?.Invoke(value);
        }
    }

    public Type ValueType => typeof(T);

    public event Action<T> ValueChanged;

    public ObservableProperty() { }

    public ObservableProperty(T value)
    {
        m_Value = value;
    }

    public void Track(Action<T> action)
    {
        ValueChanged += action;
        action?.Invoke(Value);
    }

    public void StopTracking(Action<T> action)
    {
        ValueChanged -= action;
    }
}

[SerializebleObject]
public class NodeEditorView
{
    [SerializebleProperty] private NodeTransform m_Transform;
    private NodeDrawer m_Drawer;

    public NodeDrawer Drawer => m_Drawer;
    public NodeTransform Transform => m_Transform;

    public Rect Rect => new Rect(m_Transform.Position, m_Drawer.Size);
    public Vector2 Center => Rect.position + Rect.size / 2;

    public Rect DragingArea
    {
        get
        {
            Rect dragingArea = new Rect(Transform.Position,
                new Vector2(m_Drawer.Size.x, m_Drawer.TitlePudding));

            return dragingArea;
        }
    }

    public NodeEditorView(NodeDrawer drower, NodeTransform transform)
    {
        m_Drawer = drower;
        m_Transform = transform;
    }

    public NodeEditorView() { }

    public void UpdateView()
    {
        m_Transform.UpdatePosition();
    }

    public void Draw(bool isOnScreen = true) => Drawer.Draw(Transform.Position, isOnScreen);
}

[CustomNodeDrawer(typeof(Node))]
public class NodeDrawer
{
    public bool IsSelected;

    public string NodeTitle { get; set; }

    //For calculate rects
    protected float NodeBorderPudding;
    protected Rect NodeTextureRect;

    protected List<ParameterDrawer> ParameterDrawers = new List<ParameterDrawer>();
    protected List<PortDrawer> InputPortDrawers = new List<PortDrawer>();
    protected List<PortDrawer> OutputPortDrawers = new List<PortDrawer>();

    public int InputPortCount => InputPortDrawers.Count;
    public int OutputPortCount => OutputPortDrawers.Count;
    public int ParametersCount => ParameterDrawers.Count; 

    public Rect Rect { get; protected set; }

    public Vector2 Size { get; protected set; }
    public Vector2 OriginSize { get; protected set; }
    protected Node Node { get; private set; }

    public float TitlePudding { get; protected set; }

    public float InputPortAreaWidth { get; protected set; }
    public float OutputPortAreaWidth { get; protected set; }

    protected SerializedObject SerializedNode { get; private set; }

    public virtual void Init(Node node)
    {
        Node = node;
        SerializedNode = new SerializedObject(node);

        NodeTitle = Node.GetType().ToString();
        Size = new Vector2(200, 110);
        OriginSize = Size;
        TitlePudding = 30;
        NodeBorderPudding = 12;

        InitPorts();
    }

    protected virtual void InitPorts()
    {
        if (NodePropertyCasher.TryGetInputPropertes(Node.GetType(), out FieldInfo[] inputFields))
        {
            foreach (FieldInfo fieldInfo in inputFields)
            {
                PortDrawer drawer = PortDrawer.Instantiate(Node, SerializedNode, fieldInfo);
                InputPortDrawers.Add(drawer);
            }
        }

        if (NodePropertyCasher.TryGetOutputPropertes(Node.GetType(), out FieldInfo[] outputFields))
        {
            foreach (FieldInfo fieldInfo in outputFields)
            {
                PortDrawer drawer = PortDrawer.Instantiate(Node, SerializedNode, fieldInfo);
                OutputPortDrawers.Add(drawer);
            }
        }

        if (NodePropertyCasher.TryGetParametorPropertes(Node.GetType(), out FieldInfo[] parametorFields))
        {
            foreach (FieldInfo fieldInfo in parametorFields)
            {
                ParameterDrawer drawer = ParameterDrawer.Instantiate(SerializedNode, fieldInfo);
                ParameterDrawers.Add(drawer);
            }
        }
    }

    public void LoopThroughInputPortDawers(Action<PortDrawer> action)
    {
        foreach (PortDrawer drawer in InputPortDrawers)
            action?.Invoke(drawer);
    }

    public void LoopThroughInputParameters(Action<ParameterDrawer> action)
    {
        foreach (ParameterDrawer drawer in ParameterDrawers)
            action?.Invoke(drawer);
    }

    public PortDrawer GetPortDrawer(string fieldName)
    {
        Debug.Log(fieldName);

        foreach (PortDrawer drawer in InputPortDrawers)
        {
            if (drawer.PropertyName == fieldName)
                return drawer;
        }

        foreach (PortDrawer drawer in OutputPortDrawers)
        {
            if (drawer.PropertyName == fieldName)
                return drawer;
        }

        throw new Exception();
    }

    public virtual void Draw(Vector2 position, bool isOnScreen = true)
    {
        float nodeHeight = GetPortAreaHeight() + TitlePudding + 24;
        Size = new Vector2(Size.x, nodeHeight);

        Rect = new Rect(position, Size);
        Rect rect = EnigmaticGUI.GetFixedBox(Rect, new RectOffset(11, 13, 11, 13));

        CalculatePortArea();

        if (IsSelected)
            GUI.Box(rect, "", EnigmaticStyles.selectedOutLine);

        EnigmaticGUILayout.BeginVertical(Rect, EnigmaticStyles.SnewNodeBox, 12, EnigmaticGUILayout.ExpandHeight(false),
            EnigmaticGUILayout.ExpandWidth(false), EnigmaticGUILayout.ElementSpacing(3));
        {
            EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true),
                EnigmaticGUILayout.ExpandWidth(true));
            {
                OnDrawTitle();
            }
            EnigmaticGUILayout.EndHorizontal();

            EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true),
                EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Padding(0));
            {
                EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.Width(InputPortAreaWidth),
                    EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(false),
                    EnigmaticGUILayout.ElementSpacing(6));
                {
                    OnDrawInputPortArea();
                }
                EnigmaticGUILayout.EndVertical();

                EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.Width(OutputPortAreaWidth),
                    EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(false),
                    EnigmaticGUILayout.ElementSpacing(6));
                {
                    OnDrawOutputPortArea();
                }
                EnigmaticGUILayout.EndVertical();
            }
            EnigmaticGUILayout.EndHorizontal();
        }
        EnigmaticGUILayout.EndVertical();

        ApplyModifiedProperties();
    }

    protected virtual void ApplyModifiedProperties()
    {
        SerializedNode.ApplyModifiedProperties();
    }

    public virtual void CalculatePortArea()
    {
        InputPortAreaWidth = Rect.width / 2;
        OutputPortAreaWidth = Rect.width / 2;

        if (InputPortDrawers.Count == 0)
        {
            InputPortAreaWidth = 0;
            OutputPortAreaWidth = Rect.width;
        }

        if (OutputPortDrawers.Count == 0)
        {
            InputPortAreaWidth = Rect.width - 6;
            OutputPortAreaWidth = 0;
        }
    }

    public float GetPortAreaHeight()
    {
        float inputPortHeight = 0;
        float outputPortHeight = 0;

        foreach (PortDrawer drawer in InputPortDrawers)
            inputPortHeight += drawer.GetPortHeight() + 5;

        foreach (PortDrawer drawer in OutputPortDrawers)
            outputPortHeight += drawer.GetPortHeight() + 5;

        if (inputPortHeight > outputPortHeight)
            return inputPortHeight;
        else
            return outputPortHeight;
    }

    protected virtual void OnDrawTitle()
    {
        //GUILayout.Label(NodeTitle, EnigmaticStyles.nodeTitleStyle);
        EnigmaticGUILayout.Lable(NodeTitle, EnigmaticStyles.nodeTitleStyle);
    }

    protected virtual void OnDrawInputPortArea()
    {
        foreach (PortDrawer drawer in InputPortDrawers)
        {
            drawer.Draw(InputPortAreaWidth);
        }
    }

    protected virtual void OnDrawOutputPortArea()
    {
        foreach (PortDrawer drawer in OutputPortDrawers)
        {
            drawer.Draw(OutputPortAreaWidth);
        }
    }
}

[CustomNodeDrawer(typeof(PortNode))]
public class NodePortDrawer : NodeDrawer
{
    public override void Init(Node node)
    {
        base.Init(node);

        NodeTitle = Node.Name;
    }
}

[CustomNodeDrawer(typeof(SubGraphNode))]
public class SubGraphNodeDrawer : NodeDrawer
{
    private List<SerializedObject> m_SerializedNodes = new List<SerializedObject>();

    private NodeGraphEditor m_Graph;

    public override void Init(Node node)
    {
        base.Init(node);

        NodeTitle = Node.Name;
    }

    protected override void InitPorts()
    {
        if (Node is SubGraphNode subGraphNode == false)
            return;

        NodeGraph graph = subGraphNode.OwnGraph;

        Debug.Log(graph.GetType());

        graph.TryGetAllNodeByType<InputNode>(out Node[] inputNodes);
        graph.TryGetAllNodeByType<OutputNode>(out Node[] outputNodes);
        graph.TryGetAllNodeByType<ParameterNode>(out Node[] parameter);

        NodeGraphEditor nodeGraphEditor = NodeGraphEditor.Active.GetSubNodeGraphEditor(subGraphNode);

        foreach (InputNode node in inputNodes)
        {
            if (NodePropertyCasher.TryGetOutputPropertes(node.GetType(), out FieldInfo[] fieldInfos))
            {
                SerializedObject serializedInputNode = new SerializedObject(node);
                m_SerializedNodes.Add(serializedInputNode);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    PortDrawer drawer = PortDrawer.Instantiate(subGraphNode, serializedInputNode,
                        fieldInfo, PortType.Input, null, node.Name);

                    InputPortDrawers.Add(drawer);
                }
            }
        }

        foreach (OutputNode node in outputNodes)
        {
            if (NodePropertyCasher.TryGetInputPropertes(node.GetType(), out FieldInfo[] fieldInfos))
            {
                SerializedObject serializedOutputNode = new SerializedObject(node);
                m_SerializedNodes.Add(serializedOutputNode);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    PortDrawer drawer = PortDrawer.Instantiate(subGraphNode, serializedOutputNode,
                        fieldInfo, PortType.Output, null, node.Name);

                    OutputPortDrawers.Add(drawer);
                }
            }
        }

        foreach (ParameterNode node in parameter)
        {
            if (NodePropertyCasher.TryGetOutputPropertes(node.GetType(), out FieldInfo[] fieldInfos))
            {
                SerializedObject serializedParametorNode = new SerializedObject(node);
                m_SerializedNodes.Add(serializedParametorNode);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    ParameterDrawer drawer = ParameterDrawer.Instantiate(serializedParametorNode, fieldInfo, null, node.Name);

                    ParameterDrawers.Add(drawer);
                }
            }
        }
    }

    protected override void ApplyModifiedProperties()
    {
        foreach (SerializedObject serializedNode in m_SerializedNodes)
            serializedNode.ApplyModifiedProperties();
    }

    protected override void OnDrawTitle()
    {
        EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.subgraphNodeIcon);

        EnigmaticGUILayout.Space(3);

        EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
            EnigmaticGUILayout.Padding(-5), EnigmaticGUILayout.ElementSpacing(-5));
        {
            EnigmaticGUILayout.Lable("SubGraph", EnigmaticStyles.nodeLitleTitleStyle);
            base.OnDrawTitle();
        }
        EnigmaticGUILayout.EndVertical();

        EnigmaticGUILayout.Space(6);

        if (EnigmaticGUILayout.Button("Open Graph"))
        {
            if (Node is SubGraphNode subGraphNode)
            {
                if (m_Graph == null)
                    m_Graph = NodeGraphEditor.Active.GetSubNodeGraphEditor(subGraphNode);

                NodeGraphEditor.Open(m_Graph);
            }
        }
    }
}

public class PortDrawer
{
    public bool IsConnected;

    protected float Size = 12f;

    protected PortType Type;
    public string Name;

    public UnityEditor.SerializedProperty Property { get; private set; }
    public Vector2 Offset = Vector2.zero;

    public Port Port { get; private set; }

    public string PropertyName
    {
        get
        {
            if (Name == Property.displayName)
                return Property.name;

            return Name;
        }
    }

    public static PortDrawer Instantiate(Node node, SerializedObject serializedNode, FieldInfo fieldInfo,
        NodeGraphEditor nodeGraphEditor = null, string customName = "")
    {
        UnityEditor.SerializedProperty property = serializedNode.FindProperty(fieldInfo.Name);

        if (property == null)
        {
            Debug.LogError($"Could not find serialized property '{fieldInfo.Name}' for node with type " +
                $"'{node.GetType()}' ! Perhaps you forgot to add an attribute [SerializeField] " +
                $"or fieldType {fieldInfo.FieldType} doesn't have attribute [Serializable]");

            return null;
        }

        if (nodeGraphEditor == null)
            nodeGraphEditor = NodeGraphEditor.Active;

        string fieldName = customName;

        if (string.IsNullOrEmpty(customName))
            fieldName = fieldInfo.Name;

        Port port = nodeGraphEditor.PortManager.GetPort(node, fieldName);

        Type drawerType = NodeGraphEditorRegister.GetPortDrawer(NodeGraphEditor
            .ActiveNodeGraph.GetType(), fieldInfo.FieldType);

        PortDrawer drawer = Activator.CreateInstance(drawerType, port, property, customName) as PortDrawer;
        return drawer;
    }

    public static PortDrawer Instantiate(Node node, SerializedObject serializedNode,
        FieldInfo fieldInfo, PortType type, NodeGraphEditor nodeGraphEditor = null, string customName = "")
    {
        PortDrawer drawer = Instantiate(node, serializedNode, fieldInfo, nodeGraphEditor, customName);
        drawer.Type = type;

        return drawer;
    }

    public PortDrawer(Port port, UnityEditor.SerializedProperty property, string customName = "")
    {
        Port = port;
        Property = property;
        Type = port.PortType;

        Name = customName;

        if (string.IsNullOrEmpty(customName))
            Name = Property.displayName;
    }

    public virtual float GetPortHeight()
    {
        return EditorGUI.GetPropertyHeight(Property, true);
        //return 18;
    }

    public void Draw(float widthArea, bool isOnScreen = true, bool IsDrawPort = true)
    {
        //EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.Width(widthArea),
        //    EnigmaticGUILayout.ExpandHeight(true));
        //{
        //    DrawPort(isOnScreen, IsDrawPort);
        //}
        //EnigmaticGUILayout.EndHorizontal(); 
    } //Delete

    protected virtual void DrawPort(bool isOnScreen, bool IsDrawPort)
    {
        //if (m_Type == PortType.Input)
        //{
        //    if (IsConnected == false)
        //    {
        //        if (IsDrawPort)
        //        {
        //            EnigmaticGUILayout.Port(Vector2.one * Size, EnigmaticStyles.Port);
        //            UpdatePortRect();
        //        }

        //        if(isOnScreen)
        //            EnigmaticGUILayout.Property(Property);
        //    }
        //    else
        //    {
        //        if (IsDrawPort)
        //        {
        //            EnigmaticGUILayout.Port(Vector2.one * Size, EnigmaticStyles.Port);
        //            UpdatePortRect();
        //        }

        //        if (isOnScreen)
        //            EnigmaticGUILayout.Lable(Property.Name);
        //    }
        //}
        //else
        //{
        //    GUIGrup grup = EnigmaticGUILayout.GetActiveGrup();
        //    float offset = grup.ElementSpacing * 2 + grup.Pudding * 2 + 26;

        //    EnigmaticGUILayout.Space(grup.Rect.width - 
        //        EnigmaticGUILayout.CalculateSize(Property.Name).x - 
        //        EnigmaticGUILayout.CalculateSize(Vector2.one * Size).x
        //        - offset);

        //    if (isOnScreen)
        //        EnigmaticGUILayout.Lable(Property.Name);

        //    if (IsDrawPort)
        //    {
        //        if (IsConnected == false)
        //            EnigmaticGUILayout.Port(Vector2.one * Size, EnigmaticStyles.Port);
        //        else
        //            EnigmaticGUILayout.Port(Vector2.one * Size, EnigmaticStyles.Port);

        //        UpdatePortRect();
        //    }
        //}
    } //Delete

    //public virtual void Draw(SerializedObject serializedNode, float widthArea)
    //{
    //    GUILayout.BeginHorizontal();
    //    {
    //        if (m_Type == PortType.Input)
    //        {
    //            if (IsConnected == false)
    //            {
    //                EnigmaticGUI.Image(Vector2.one * Size, "", EnigmaticStyles.Port);
    //                UpdatePortRect();

    //                EnigmaticGUI.PropertyField(Property, widthArea - Size);
    //            }
    //            else
    //            {
    //                EnigmaticGUI.Image(Vector2.one * Size, "", EnigmaticStyles.Port);
    //                UpdatePortRect();

    //                EnigmaticGUI.sLabel(Property.displayName);
    //            }
    //        }
    //        else
    //        {
    //            float space = widthArea - (EnigmaticGUILayout.CalculateSize(Property.displayName).x + Size * 3f);

    //            GUILayout.Space(space);

    //            EnigmaticGUI.sLabel(Property.displayName);

    //            UpdatePortRect();
    //            if (IsConnected == false)
    //                EnigmaticGUI.Image(Vector2.one * Size, "", EnigmaticStyles.Port);
    //            else
    //                EnigmaticGUI.Image(Vector2.one * Size, "", EnigmaticStyles.Port);
    //        }
    //    }
    //    GUILayout.EndHorizontal();
    //}

    public virtual void Draw(float widthArea)
    {
        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.Width(widthArea),
            EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.Padding(0));
        {
            if (Type == PortType.Input)
            {
                DrawPort();
                DrawField();
            }
            else
            {
                float space = widthArea - EnigmaticGUILayout.CalculateSize(Name).x - 36;

                EnigmaticGUILayout.Space(space);

                DrawField();
                DrawPort();
            }
        }
        EnigmaticGUILayout.EndHorizontal();
    }

    public virtual void DrawPort()
    {
        if (IsConnected)
            EnigmaticGUILayout.Port(Vector2.one * Size, EnigmaticStyles.Port);
        else
            EnigmaticGUILayout.Port(Vector2.one * Size, EnigmaticStyles.Port);

        UpdatePortRect();
    }

    public virtual void DrawField()
    {
        if (Type == PortType.Input)
        {
            if (IsConnected)
                EnigmaticGUILayout.Lable(Name);
            else
                EnigmaticGUILayout.PropertyField(Property, -1, Name);
        }
        else
        {
            EnigmaticGUILayout.Lable(Name);
        }
    }

    private void UpdatePortRect()
    {
        Vector2 size = Vector2.one * Size;
        Vector2 position = EnigmaticGUILayout.GetLastGUIRect().position;

        Port.Rect = new Rect(position, size);
        Port.MidelPoint = position - size / 2;
    }
}

[CustomPortDrawer(typeof(NodeGraph), typeof(Trace))]
public class TracePortDrawer : PortDrawer
{
    public TracePortDrawer(Port port, UnityEditor.SerializedProperty property, string customName = "") 
        : base(port, property, customName) { }

    public override void Draw(float widthArea)
    {
        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.Width(widthArea),
            EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.Padding(0));
        {
            if (Type == PortType.Output)
            {
                float space = widthArea - 33;
                EnigmaticGUILayout.Space(space);
            }

            DrawPort();
        }
        EnigmaticGUILayout.EndHorizontal();
    }
}

namespace Enigmatic.Core
{
    public static class SerializedObjectExtention
    {
        public static Type GetObjectType(this SerializedObject serializedObject)
        {
            UnityEngine.Object obj = serializedObject.targetObject;
            return obj.GetType();
        }
    }
}

public class ParameterDrawer
{
    public string Name;
    public UnityEditor.SerializedProperty Property { get; private set; }

    public static ParameterDrawer Instantiate(SerializedObject serializedNode, FieldInfo fieldInfo,
        NodeGraphEditor nodeGraphEditor = null, string customName = "")
    {
        UnityEditor.SerializedProperty property = serializedNode.FindProperty(fieldInfo.Name);

        if (property == null)
        {
            Debug.LogError($"Could not find serialized property '{fieldInfo.Name}' for node with type " +
                $"'{serializedNode.GetObjectType()}' ! Perhaps you forgot to add an attribute [SerializeField]");

            return null;
        }

        string fieldName = customName;

        if (string.IsNullOrEmpty(customName))
            fieldName = fieldInfo.Name;

        Type drawerType = NodeGraphEditorRegister.GetParameterDrawer(NodeGraphEditor
            .ActiveNodeGraph.GetType(), fieldInfo.FieldType);

        ParameterDrawer drawer = Activator.CreateInstance(drawerType, property, customName) as ParameterDrawer;
        return drawer;
    }

    public ParameterDrawer(UnityEditor.SerializedProperty property, string customName = "")
    {
        Property = property;
        Name = customName;

        if (string.IsNullOrEmpty(customName))
            Name = Property.displayName;
    }

    public virtual void Draw(float widthArea)
    {
        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.Width(widthArea),
            EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.Padding(0));
        {
            DrawField();
        }
        EnigmaticGUILayout.EndHorizontal();
    }

    public virtual void DrawField()
    {
        EnigmaticGUILayout.PropertyField(Property, -1, Name);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class CustomPortDrawer : Attribute
{
    public readonly Type NodeGraphType;
    public readonly Type PropertyType;

    public CustomPortDrawer(Type nodeGraphType, Type propertyType)
    {
        NodeGraphType = nodeGraphType;
        PropertyType = propertyType;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class CustomParametorDrawer : Attribute
{
    public readonly Type NodeGraphType;
    public readonly Type PropertyType;

    public CustomParametorDrawer(Type nodeGraphType, Type propertyType)
    {
        NodeGraphType = nodeGraphType;
        PropertyType = propertyType;
    }
}

public class NodePropertyCasher
{
    private static Dictionary<Type, List<FieldInfo>> sm_Input = new Dictionary<Type, List<FieldInfo>>();
    private static Dictionary<Type, List<FieldInfo>> sm_Output = new Dictionary<Type, List<FieldInfo>>();
    private static Dictionary<Type, List<FieldInfo>> sm_Parameters = new Dictionary<Type, List<FieldInfo>>();

    private Dictionary<Type, List<FieldInfo>> m_Input = new Dictionary<Type, List<FieldInfo>>();
    private Dictionary<Type, List<FieldInfo>> m_Output = new Dictionary<Type, List<FieldInfo>>();
    private Dictionary<Type, List<FieldInfo>> m_Parameters = new Dictionary<Type, List<FieldInfo>>();

    //public void Cash(Type type)
    //{
    //    if (typeof(Node).IsAssignableFrom(type) == false)
    //    {
    //        Debug.LogError("This type is not a node!");
    //        return;
    //    }

    //    if (m_Input.ContainsKey(type) || m_Output.ContainsKey(type)
    //        || m_Output.ContainsKey(type) || m_Parameters.ContainsKey(type))
    //    {
    //        return;
    //    }

    //    m_Input.Add(type, new List<FieldInfo>());
    //    m_Output.Add(type, new List<FieldInfo>());
    //    m_Parameters.Add(type, new List<FieldInfo>());

    //    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    //    foreach (FieldInfo field in fields)
    //    {
    //        InputPort inputPort = field.GetAttribute<InputPort>();
    //        OutputPort outputPort = field.GetAttribute<OutputPort>();
    //        Parameter parameter = field.GetAttribute<Parameter>();

    //        if (inputPort != null)
    //            m_Input[type].Add(field);
    //        else if (outputPort != null)
    //            m_Output[type].Add(field);
    //        else if (parameter != null)
    //            m_Parameters[type].Add(field);
    //    }
    //}

    public static void Cash(Type type)
    {
        if (typeof(Node).IsAssignableFrom(type) == false)
        {
            Debug.LogError("This type is not a node!");
            return;
        }

        if (sm_Input.ContainsKey(type) || sm_Output.ContainsKey(type)
            || sm_Parameters.ContainsKey(type))
        {
            return;
        }

        sm_Input.Add(type, new List<FieldInfo>());
        sm_Output.Add(type, new List<FieldInfo>());
        sm_Parameters.Add(type, new List<FieldInfo>());

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            InputPort inputPort = field.GetAttribute<InputPort>();
            OutputPort outputPort = field.GetAttribute<OutputPort>();
            Parameter parameter = field.GetAttribute<Parameter>();

            if (inputPort != null)
                sm_Input[type].Add(field);
            else if (outputPort != null)
                sm_Output[type].Add(field);
            else if (parameter != null)
                sm_Parameters[type].Add(field);
        }
    }

    public static bool TryGetInputProperty(Type type, string fieldName, out FieldInfo fieldInfo)
    {
        fieldInfo = null;

        if (sm_Input.ContainsKey(type) == false)
            return false;

        foreach (FieldInfo field in sm_Input[type])
        {
            if (field.Name == fieldName)
            {
                fieldInfo = field;
                return true;
            }
        }

        return false;
    }

    public static bool TryGetInputPropertes(Type type, out FieldInfo[] fieldInfos)
    {
        fieldInfos = null;

        if (sm_Input.ContainsKey(type) == false)
            return false;

        fieldInfos = sm_Input[type].ToArray();
        return true;
    }

    public static bool TryGetOutputPropertes(Type type, out FieldInfo[] fieldInfos)
    {
        fieldInfos = null;

        if (sm_Output.ContainsKey(type) == false)
            return false;

        fieldInfos = sm_Output[type].ToArray();
        return true;
    }

    public static bool TryGetOutputProperty(Type type, string fieldName, out FieldInfo fieldInfo)
    {
        fieldInfo = null;

        if (sm_Output.ContainsKey(type) == false)
            return false;

        foreach (FieldInfo field in sm_Output[type])
        {
            if (field.Name == fieldName)
            {
                fieldInfo = field;
                return true;
            }
        }

        return false;
    }

    public static bool TryGetParametorPropertes(Type type, out FieldInfo[] fieldInfos)
    {
        fieldInfos = null;

        if (sm_Parameters.ContainsKey(type) == false)
            return false;

        fieldInfos = sm_Parameters[type].ToArray();
        return true;
    }
}

[Serializable]
[SerializebleObject]
public class NodeTransform
{
    [SerializebleProperty] private Vector2 m_Position;

    public Vector2 OriginPosition => m_Position;
    public Vector2 Position { get; private set; }
    public Vector2 LastPosition { get; private set; }

    public NodeTransform(Vector2 position)
    {
        m_Position = position;
    }

    public NodeTransform() { }

    public void Move(Vector2 newPosition) => m_Position = newPosition;

    public void UpdateLastPosition() => LastPosition = m_Position;

    public void UpdatePosition()
    {
        Position = m_Position - NodeGraphEditor.Active.ZoomCordsOrigin;
    }
}

//Test
public class FloatNode : Node
{
    private float x;
    [InputPort] private float y;
    [OutputPort][DisableMultiConnection] private float w;
    private string name = nameof(FloatNode);
}

//Test
[GraphCreateWindow(typeof(NodeGraph), "NodeGraph/AkiNode")]
public class AkiNode : Node { }

//Test
[CustomNodeDrawer(typeof(AkiNode))]
public class AkiNodeDrawer : NodeDrawer
{
    public GUIStyle GetFrame(int frame)
    {
        GUIStyle frameResult;

        frameResult = new GUIStyle(GUI.skin.box);

        if (frame == 0)
            frameResult.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Experemantal/Akipatpat/frame_0_delay-0.06s.gif") as Texture2D;
        else if (frame == 1)
            frameResult.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Experemantal/Akipatpat/frame_1_delay-0.06s.gif") as Texture2D;
        else if (frame == 2)
            frameResult.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Experemantal/Akipatpat/frame_2_delay-0.06s.gif") as Texture2D;
        else if (frame == 3)
            frameResult.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Experemantal/Akipatpat/frame_3_delay-0.06s.gif") as Texture2D;
        else
            frameResult.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Experemantal/Akipatpat/frame_4_delay-0.06s.gif") as Texture2D;

        return frameResult;
    }

    int currentFrame = 0;

    public override void Draw(Vector2 position, bool isOnSceen = true)
    {
        //base.Draw(position);

        Rect rect = new Rect(position, Size);
        GUI.Box(rect, "", GetFrame(currentFrame / 20));

        if (currentFrame < 100)
            currentFrame++;
        else
            currentFrame = 0;
    }
}

//Test
[CustomNodeDrawer(typeof(Root))]
public class RootDrower : NodeDrawer
{
    public override void Init(Node node)
    {
        base.Init(node);

        NodeTitle = "NPC Phrase";
        Size = new Vector2(Size.x * 2, Size.y);
    }
}

[CustomNodeDrawer(typeof(RootSubGraphNode))]
public class RootSubGraphNodeDrower : SubGraphNodeDrawer
{
    public override void Init(Node node)
    {
        base.Init(node);

        Size = new Vector2(Size.x * 2, Size.y);
    }
}

//Test
namespace NodesTest
{
    [SerializebleObject]
    [GraphCreateWindow(typeof(NodeGraph), "NodeGraph/Root")]
    public class Root : Node
    {
        public string[] names = new string[10];

        //[InputPort] private Node Input;
        //[InputPort] private float Input;

        [SerializeField][InputPort] private Trace m_TraceInput;
        [SerializeField][InputPort] private float[] Input1;
        [SerializeField][InputPort] private float Inputs;
        [SerializeField][InputPort] private float Inputs1;
        [SerializeField][InputPort] private float Inputs2 = 5;
        [SerializeField][InputPort] private float Inputs3;
        [SerializeField][InputPort] private float Inputs4;
        [SerializeField][InputPort] private float Inputs5;
        [SerializeField][InputPort] private float Inputs6;
        [SerializeField][InputPort] private float Inputs7;
        [SerializeField][InputPort] private float Inputs8;
        [SerializeField][InputPort] private float Inputs9;
        [SerializeField][InputPort] private float Inputs0;
        [SerializeField][InputPort] private float Inputs10;

        //[OutputPort] private Node Output;
        [SerializeField] [OutputPort] [DisableMultiConnection] private Trace m_TraceOut;
        [SerializeField] [OutputPort] private float Output = 5;
        [SerializeField] [OutputPort] private float Output2323;
        [SerializeField] [OutputPort] private float Output3;
        [SerializeField] [OutputPort] private float Output4;

        [SerializeField] [Parameter] private int PhraseIndex;
        [SerializeField] [Parameter] private int PoseIndex;
        [SerializeField] [Parameter] private int VoiceIndex;
    }

    [SerializebleObject]
    [GraphCreateWindow(typeof(NodeGraph), "NodeGraph/SubGraphNode")]
    public class RootSubGraphNode : SubGraphNode { }

    [SerializebleObject]
    [GraphCreateWindow(typeof(NodeGraph), "NodeGraph/Ports/Input/Float", true)]
    public class RootFloatInput : InputNode
    {
        [SerializeField][OutputPort] private float m_Value;
    }

    [SerializebleObject]
    [GraphCreateWindow(typeof(NodeGraph), "NodeGraph/Ports/Input/Array/Float", true)]
    public class RootFloatArrayInput : InputNode
    {
        [SerializeField][OutputPort] private float[] m_Value;
    }

    [SerializebleObject]
    [GraphCreateWindow(typeof(NodeGraph), "NodeGraph/Ports/Output/Float", true)]
    public class RootFloatOutput : OutputNode
    {
        [SerializeField][InputPort] private float m_Value;
    }

    [SerializebleObject]
    [GraphCreateWindow(typeof(NodeGraph), "NodeGraph/Ports/Parameters/Float", true)]
    public class RootParameters : ParameterNode
    {
        [SerializeField] [OutputPort] private float m_Value;
    }
}

[InitializeOnLoad]
public static class NodeGraphEditorRegister
{
    private static Dictionary<Type, Type> s_EditorNodeDrawerMap;
    private static Dictionary<Type, Dictionary<Type, Type>> s_EditorPortDrawerMap;
    private static Dictionary<Type, Dictionary<Type, Type>> s_EditorNodeParameterDrawerMap;

    static NodeGraphEditorRegister()
    {
        s_EditorNodeDrawerMap = new Dictionary<Type, Type>();
        s_EditorPortDrawerMap = new Dictionary<Type, Dictionary<Type, Type>>();
        s_EditorNodeParameterDrawerMap = new Dictionary<Type, Dictionary<Type, Type>>();

        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        RegisterEditorsFromAssembly(currentAssembly);
    }

    public static void RegisterEditorsFromAssembly(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(NodeDrawer).IsAssignableFrom(type))
                RegistorEditorNodeDrawer(type);
            else if (typeof(PortDrawer).IsAssignableFrom(type))
                RegistorEditorPortDrawer(type);
            else if(typeof(ParameterDrawer).IsAssignableFrom(type))
                RegistorEditorParameterDrawer(type);
        }
    }

    private static void RegistorEditorNodeDrawer(Type drawerType)
    {
        CustomNodeDrawer customNodeDrawer = (CustomNodeDrawer)Attribute.GetCustomAttribute(drawerType, typeof(CustomNodeDrawer));

        if (customNodeDrawer == null)
            return;

        if (s_EditorPortDrawerMap.ContainsKey(customNodeDrawer.NodeType))
            throw new InvalidOperationException($"Non register Custom Editor Node form {drawerType}");

        s_EditorNodeDrawerMap.Add(customNodeDrawer.NodeType, drawerType);
    }

    private static void RegistorEditorPortDrawer(Type drawerType)
    {
        CustomPortDrawer customPropertyDrawer = (CustomPortDrawer)Attribute.GetCustomAttribute(drawerType, typeof(CustomPortDrawer));

        if (customPropertyDrawer == null)
            return;

        Type graphType = customPropertyDrawer.NodeGraphType;
        Type properyType = customPropertyDrawer.PropertyType;

        if (typeof(NodeGraph).IsAssignableFrom(graphType) == false)
        {
            Debug.LogError($"{drawerType} : {graphType}");
            return;
        }

        if (s_EditorPortDrawerMap.ContainsKey(graphType) == false)
            s_EditorPortDrawerMap.Add(graphType, new Dictionary<Type, Type>());

        if (s_EditorPortDrawerMap[graphType].ContainsKey(properyType))
            throw new InvalidOperationException($"Non register Custom Editor Node form {drawerType}");

        s_EditorPortDrawerMap[graphType].Add(properyType, drawerType);
    }

    private static void RegistorEditorParameterDrawer(Type drawerType)
    {
        CustomParametorDrawer customPropertyDrawer = (CustomParametorDrawer)Attribute.GetCustomAttribute(drawerType, typeof(CustomParametorDrawer));

        if (customPropertyDrawer == null)
            return;

        Type graphType = customPropertyDrawer.NodeGraphType;
        Type properyType = customPropertyDrawer.PropertyType;

        if (typeof(NodeGraph).IsAssignableFrom(graphType))
        {
            Debug.LogError($"{drawerType} : {graphType}");
            return;
        }

        if (s_EditorNodeParameterDrawerMap.ContainsKey(graphType) == false)
            s_EditorNodeParameterDrawerMap.Add(graphType, new Dictionary<Type, Type>());

        if (s_EditorNodeParameterDrawerMap[graphType].ContainsKey(properyType))
            throw new InvalidOperationException($"Non register Custom Editor Node form {drawerType}");

        s_EditorNodeParameterDrawerMap[graphType].Add(properyType, drawerType);
    }

    public static Type GetNodeDrawer(Type nodeType)
    {
        if (s_EditorNodeDrawerMap.ContainsKey(nodeType) == false)
        {
            if (nodeType.BaseType != null)
                return GetNodeDrawer(nodeType.BaseType);
        }

        return s_EditorNodeDrawerMap[nodeType];
    }

    public static Type GetPortDrawer(Type graphType, Type propertyType)
    {
        if (s_EditorPortDrawerMap.ContainsKey(graphType) == false)
        {
            if (graphType.BaseType != null)
                return GetPortDrawer(graphType.BaseType, propertyType);

            return typeof(PortDrawer);
        }

        if (s_EditorPortDrawerMap[graphType].ContainsKey(propertyType) == false)
        {
            if (graphType.BaseType != null)
                return GetPortDrawer(graphType.BaseType, propertyType);

            return typeof(PortDrawer);
        }

        return s_EditorPortDrawerMap[graphType][propertyType];
    }

    public static Type GetParameterDrawer(Type graphType, Type propertyType)
    {
        if (s_EditorNodeParameterDrawerMap.ContainsKey(graphType) == false)
        {
            if (graphType.BaseType != null)
                return GetParameterDrawer(graphType.BaseType, propertyType);

            return typeof(ParameterDrawer);
        }

        if (s_EditorNodeParameterDrawerMap[graphType].ContainsKey(propertyType) == false)
        {
            if (graphType.BaseType != null)
                return GetParameterDrawer(graphType.BaseType, propertyType);

            return typeof(ParameterDrawer);
        }

        return s_EditorNodeParameterDrawerMap[graphType][propertyType];
    }
}

//No use
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ConvertorFlagType : Attribute
{
    public readonly Type Type;

    public ConvertorFlagType(Type type)
    {
        Type = type;
    }
}

//No use
public class ConvertorRegister
{
    private static Dictionary<Type, Convertor> s_Convertors = new Dictionary<Type, Convertor>();

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    [InitializeOnLoadMethod]
    private static void RegistConvertors()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(Convertor).IsAssignableFrom(type))
                RegistConvertor(type);
        }
    }

    private static void RegistConvertor(Type type)
    {
        ConvertorFlagType convertorFlagType = (ConvertorFlagType)Attribute.GetCustomAttribute(type, typeof(ConvertorFlagType));

        if (convertorFlagType == null)
            return;

        Convertor convertor = Activator.CreateInstance(type) as Convertor;
        s_Convertors.Add(convertorFlagType.Type, convertor);
    }

    public static Convertor GetConvertor(Type type)
    {
        if (s_Convertors.ContainsKey(type) == false)
            return null;

        return s_Convertors[type];
    }
}

//No use
public class Convertor
{
    public virtual object Convert<T>(T value) { return null; }

    public static object Convert<T>(T value, Type type)
    {
        Convertor convertor = ConvertorRegister.GetConvertor(type);

        if (convertor == null)
        {
            Debug.LogWarning($"Convert operation failed! {value.GetType()} to {type}");
            return value;
        }

        if (convertor.Convert(value) == null)
            return value;

        return convertor.Convert(value);
    }
}

//No use
[ConvertorFlagType(typeof(Root))]
public class RootConvertor : Convertor
{
    public override object Convert<T>(T value) => value as Root;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CustomNodeDrawer : Attribute
{
    public readonly Type NodeType;

    public CustomNodeDrawer(Type NodeType)
    {
        this.NodeType = NodeType;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CustomPropertyDrawer : Attribute
{
    public readonly Type PropertyType;

    public CustomPropertyDrawer(Type propertyType)
    {
        PropertyType = propertyType;
    }
} //Move to ...

namespace NodeEditor
{
    public class PortAttribute : SerializebleProperty
    {
        public PortAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InputPort : PortAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class OutputPort : PortAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class DisableMultiConnection : Attribute { }
}

[AttributeUsage(AttributeTargets.Field)]
public class Parameter : Attribute { }

public class PortManager
{
    private Dictionary<Node, List<Port>> m_RegestryPorts = new Dictionary<Node, List<Port>>();
    private SubGraphNode m_Node;

    public void LoopThroughPorts(Action<Port> action)
    {
        foreach (Node node in m_RegestryPorts.Keys)
        {
            foreach (Port port in m_RegestryPorts[node])
                action?.Invoke(port);
        }
    }

    public void RegisterPorts<T>(T node) where T : Node
    {
        //if (m_RegestryPorts.ContainsKey(node))
        //    m_RegestryPorts[node].Clear();
        //else
        //    m_RegestryPorts.Add(node, new List<Port>());

        //if (node is SubGraphNode subGraphNode)
        //    RegisterSubGraphNodePorts(subGraphNode);
        //else
        //    RegisterSimpleNodePorts(node);

        if (m_RegestryPorts.ContainsKey(node))
            m_RegestryPorts[node].Clear();
        else
            m_RegestryPorts.Add(node, new List<Port>());

        Connector[] connectors = NodeGraphEditor.ActiveConnectionManager.GetConnectorsByNode(node);

        foreach (Connector connector in connectors)
            m_RegestryPorts[node].Add(new Port(connector));
    }

    //public void RegisterSimpleNodePorts<T>(T node) where T : Node
    //{
    //    if (NodePropertyCasher.TryGetInputPropertes(node.GetType(), out FieldInfo[] inputFields))
    //    {
    //        foreach (FieldInfo field in inputFields)
    //        {
    //            DisableMultiConnection multiConnection = field.GetAttribute<DisableMultiConnection>();

    //            if (multiConnection == null)
    //                m_RegestryPorts[node].Add(new Port(node, field, PortType.Input));
    //            else
    //                m_RegestryPorts[node].Add(new Port(node, field, PortType.Input, false));
    //        }
    //    }

    //    if (NodePropertyCasher.TryGetOutputPropertes(node.GetType(), out FieldInfo[] outputFields))
    //    {
    //        foreach (FieldInfo field in outputFields)
    //        {
    //            DisableMultiConnection multiConnection = field.GetAttribute<DisableMultiConnection>();

    //            if (multiConnection == null)
    //                m_RegestryPorts[node].Add(new Port(node, field, PortType.Output));
    //            else
    //                m_RegestryPorts[node].Add(new Port(node, field, PortType.Output, false));
    //        }
    //    }
    //}

    //public void RegisterSubGraphNodePorts<T>(T subGraphNode) where T : SubGraphNode
    //{
    //    m_Node = subGraphNode;

    //    NodeGraph graph = subGraphNode.OwnGraph;

    //    if (graph == null)
    //        return;

    //    graph.TryGetAllNodeByType<InputNode>(out Node[] inputNodes);
    //    graph.TryGetAllNodeByType<OutputNode>(out Node[] outputNodes);

    //    foreach(InputNode node in inputNodes)
    //    {
    //        if(NodePropertyCasher.TryGetOutputPropertes(node.GetType(), out FieldInfo[] outputFields))
    //        {
    //            foreach(FieldInfo field in outputFields)
    //            {
    //                DisableMultiConnection multiConnection = field.GetAttribute<DisableMultiConnection>();

    //                if (multiConnection == null)
    //                    m_RegestryPorts[subGraphNode].Add(new Port(subGraphNode, node.Name, field.FieldType, PortType.Input));
    //                else
    //                    m_RegestryPorts[subGraphNode].Add(new Port(subGraphNode, node.Name, field.FieldType, PortType.Input, false));
    //            }
    //        }
    //    }

    //    foreach (OutputNode node in outputNodes)
    //    {
    //        if (NodePropertyCasher.TryGetInputPropertes(node.GetType(), out FieldInfo[] inputFields))
    //        {
    //            foreach (FieldInfo field in inputFields)
    //            {
    //                DisableMultiConnection multiConnection = field.GetAttribute<DisableMultiConnection>();

    //                if (multiConnection == null)
    //                    m_RegestryPorts[subGraphNode].Add(new Port(subGraphNode, node.Name, field.FieldType, PortType.Output));
    //                else
    //                    m_RegestryPorts[subGraphNode].Add(new Port(subGraphNode, node.Name, field.FieldType, PortType.Output, false));
    //            }
    //        }
    //    }

    //    Debug.Log(m_RegestryPorts[subGraphNode].Count());
    //}

    public void ClearRegestryPorts()
    {
        m_RegestryPorts.Clear();
        Debug.LogWarning("Regestry Ports clearing!");
    }

    public Dictionary<Node, List<string>> GetConnectors()
    {
        Dictionary<Node, List<string>> tempConnectors = new Dictionary<Node, List<string>>();

        foreach (Node node in m_RegestryPorts.Keys)
        {
            tempConnectors.Add(node, new List<string>());

            foreach (Port port in m_RegestryPorts[node])
                tempConnectors[node].Add(port.FieldName);
        }

        return tempConnectors;
    }

    public void RemovePortsByNode(Node node)
    {
        m_RegestryPorts.Remove(node);
    }

    public Port GetPort(Node node, string name)
    {
        if (node == null)
            return null;

        if (m_RegestryPorts.ContainsKey(node) == false)
        {
            foreach (Node n in m_RegestryPorts.Keys)
            {
                foreach (Port p in m_RegestryPorts[n])
                {
                    Debug.Log($"{n.GetType()} {p.FieldName}");
                }
            }
        }

        Debug.LogWarning(node);

        if (m_RegestryPorts.ContainsKey(node) == false)
            throw new Exception("Null");

        foreach (Port port in m_RegestryPorts[node])
        {
            if (port.FieldName == name)
                return port;
        }

        Debug.Log(name);
        throw new Exception();
    }

    public Port GetPort(Connector connector)
    {
        if (connector.IsSubGraph)
        {
            Node node = NodeGraphEditor.ActiveConnectionManager.GetNode(connector);
            return GetPort(node, connector.FieldName);
        }
        else
        {
            return GetPort(connector.Node, connector.FieldName);
        }
    }
}

[Serializable]
[SerializebleObject]
public class Trace { }

[Serializable]
[SerializebleObject]
public class ConnectionManager
{
    [SerializebleProperty] private Dictionary<Connector, List<Connector>> m_Connections = new Dictionary<Connector, List<Connector>>();
    [SerializebleProperty] private Dictionary<Node, List<Connector>> m_Connectors = new Dictionary<Node, List<Connector>>();

    public event Action<Connection> AddedConnection;
    public event Action<Connection> RemovedConnection;

    public event Action ChengededConnection;

    public void RegisterConnectors<T>(T node) where T : Node
    {
        if (m_Connectors.ContainsKey(node) == true)
            m_Connectors.Remove(node);

        m_Connectors.Add(node, new List<Connector>());

        m_Connectors[node] = MakeConnectors(node);
    }

    private List<Connector> MakeConnectors<T>(T node) where T : Node
    {
        if (typeof(SubGraphNode).IsAssignableFrom(node.GetType()))
            return RegisterConnectorsForSubGraphNode(node as SubGraphNode);
        else
            return RegisterConnectorsForSimpleNode(node);
    }

    private List<Connector> RegisterConnectorsForSimpleNode<T>(T node) where T : Node
    {
        List<Connector> result = new List<Connector>();

        if (NodePropertyCasher.TryGetInputPropertes(node.GetType(), out FieldInfo[] inputFields))
        {
            foreach (FieldInfo field in inputFields)
            {
                Connector connector = new Connector();
                connector.Init(node, field.Name);

                result.Add(connector);
            }
        }

        if (NodePropertyCasher.TryGetOutputPropertes(node.GetType(), out FieldInfo[] outputFields))
        {
            foreach (FieldInfo field in outputFields)
            {
                Connector connector = new Connector();
                connector.Init(node, field.Name);

                result.Add(connector);
            }
        }

        return result;
    }

    private List<Connector> RegisterConnectorsForSubGraphNode<T>(T subgraphNode) where T : SubGraphNode
    {
        List<Connector> result = new List<Connector>();

        NodeGraph nodeGraph = subgraphNode.OwnGraph;

        if (nodeGraph == null)
            return result;

        nodeGraph.TryGetAllNodeByType<InputNode>(out Node[] inputNodes);
        nodeGraph.TryGetAllNodeByType<OutputNode>(out Node[] outputNodes);

        foreach (InputNode inputNode in inputNodes)
        {
            if (NodePropertyCasher.TryGetOutputPropertes(inputNode.GetType(), out FieldInfo[] inputFields))
            {
                foreach (FieldInfo field in inputFields)
                {
                    Connector connector = new Connector();
                    connector.Init(inputNode, inputNode.Name, true);

                    result.Add(connector);
                }
            }
        }

        foreach (OutputNode outputNode in outputNodes)
        {
            if (NodePropertyCasher.TryGetInputPropertes(outputNode.GetType(), out FieldInfo[] outputFields))
            {
                foreach (FieldInfo field in outputFields)
                {
                    Connector connector = new Connector();

                    Debug.Log(outputNode.Name);
                    connector.Init(outputNode, outputNode.Name, true);

                    result.Add(connector);
                }
            }
        }

        return result;
    }

    public void FixConnectors()
    {
        Dictionary<Node, List<Connector>> newConnectors = new Dictionary<Node, List<Connector>>();

        foreach (Node node in m_Connectors.Keys)
        {
            List<Connector> validConnectors = new List<Connector>();

            foreach (Connector connector in m_Connectors[node])
            {
                if (TryGetInputProperty(connector, out FieldInfo inputField)
                    || TryGetOutputProperty(connector, out FieldInfo outputField))
                {
                    validConnectors.Add(connector);
                    continue;
                }

                if (m_Connections.ContainsKey(connector))
                {
                    m_Connections.Remove(connector);
                    continue;
                }

                foreach (Connector outputConnector in m_Connections.Keys)
                {
                    if (m_Connections[outputConnector].Contains(connector))
                        m_Connections[outputConnector].Remove(connector);
                }
            }

            List<Connector> newConnecors = new List<Connector>();

            foreach (Connector connector in MakeConnectors(node))
            {
                if (validConnectors.Contains(connector))
                    continue;

                newConnecors.Add(connector);
            }

            newConnectors.Add(node, validConnectors.Combine(newConnecors));
        }

        m_Connectors = newConnectors;
    }

    public Connector GetConnector(Node node, string fieldName)
    {
        if (m_Connectors.ContainsKey(node) == false)
        {
            Debug.Log($"Connector with this node {node} is not found");
            return null;
        }

        foreach (Connector connector in m_Connectors[node])
        {
            if (connector.FieldName == fieldName)
                return connector;
        }

        Debug.Log($"Connector with this name {fieldName} from {node} node is not found");
        return null;
    }

    public Connector[] GetConnectorsByNode(Node node)
    {
        if (m_Connectors.ContainsKey(node) == false)
            throw new Exception();

        return m_Connectors[node].ToArray();
    }

    public Node GetNode(Connector connector)
    {
        foreach (Node node in m_Connectors.Keys)
        {
            if (m_Connectors[node].Contains(connector))
                return node;
        }

        return null;
    }

    public Connector[] GetConnections(Connector connector)
    {
        List<Connector> result = new List<Connector>();

        foreach (Connector output in m_Connections.Keys)
        {
            if (connector == output)
            {
                if (m_Connections[output].Count > 0)
                    return m_Connections[output].ToArray();
            }

            foreach (Connector input in m_Connections[output])
            {
                if (connector == input)
                    result.Add(output);
            }
        }

        return result.ToArray();
    }

    public void LoopThroughConnections(Action<Connector, Connector> action)
    {
        foreach (Connector output in m_Connections.Keys)
        {
            foreach (Connector input in m_Connections[output])
                action?.Invoke(output, input);
        }
    }

    public void RemoveConnectorsByNode(Node node)
    {
        if (m_Connectors.ContainsKey(node) == false)
            throw new ArgumentException();

        RemoveConnectionByNode(node);
        m_Connectors.Remove(node);
    }

    public void AddConnection(Connection connection)
    {
        if (CanConnect(connection.Output, connection.Input) == false)
        {
            Debug.LogError("These connectors cannot be connected " +
                "to each other due to their types");
            return;
        }

        if (m_Connections.ContainsKey(connection.Output) == false)
            m_Connections.Add(connection.Output, new List<Connector>());

        m_Connections[connection.Output].Add(connection.Input);

        AddedConnection?.Invoke(connection);
        ChengededConnection?.Invoke();

        UpdateData(connection.Output);
    }

    public void AddConnection(Connector output, Connector input) => AddConnection(new Connection(output, input));

    public void RemoveConnection(Connector connector)
    {
        Queue<Connection> deleteConnection = new Queue<Connection>();

        foreach (Connector outputConnector in m_Connections.Keys)
        {
            if (outputConnector == connector)
            {
                deleteConnection.Enqueue(new Connection(outputConnector, null));
                continue;
            }

            foreach (Connector inputConnector in m_Connections[outputConnector])
            {
                if (inputConnector == connector)
                    deleteConnection.Enqueue(new Connection(outputConnector, inputConnector));
            }
        }

        while (deleteConnection.Count > 0)
        {
            Connection connection = deleteConnection.Dequeue();

            Connector output = connection.Output;
            Connector input = connection.Input;

            if (input == null)
                m_Connections.Remove(output);
            else
                m_Connections[output].Remove(input);
        }
    }

    public void RemoveConnection(Connector output, Connector input)
    {
        if (m_Connections.ContainsKey(output) == false
            || m_Connections[output].Contains(input) == false)
        {
            return;
        }

        m_Connections[output].Remove(input);
    }

    public void RemoveConnectionByNode(Node node)
    {
        Queue<Connection> deleteConnection = new Queue<Connection>();

        foreach (Connector outputConnector in m_Connections.Keys)
        {
            if (outputConnector.Node == node)
            {
                deleteConnection.Enqueue(new Connection(outputConnector, null));
                continue;
            }

            foreach (Connector inputConnector in m_Connections[outputConnector])
            {
                if (inputConnector.Node == node)
                    deleteConnection.Enqueue(new Connection(outputConnector, inputConnector));
            }
        }

        while (deleteConnection.Count > 0)
        {
            Connection connection = deleteConnection.Dequeue();

            Connector output = connection.Output;
            Connector input = connection.Input;

            if (input == null)
                m_Connections.Remove(output);
            else
                m_Connections[output].Remove(input);
        }
    }

    public void FixConnectors(List<Node> nodes)
    {
        foreach (Connector outputConnector in m_Connections.Keys)
        {
            FixConnector(outputConnector, nodes);

            foreach (Connector inputConnector in m_Connections[outputConnector])
                FixConnector(inputConnector, nodes);
        }
    }

    public void UpdateData()
    {
        foreach (Connector output in m_Connections.Keys)
            UpdateData(output);
    }

    public void UpdateData(Connector output)
    {
        if (m_Connections.ContainsKey(output) == false)
        {
            Debug.LogError($"No connection found with this output connector! FieldName : {output.FieldName}");
            return;
        }

        if (TryGetOutputProperty(output, out FieldInfo outputField) == false)
        {
            Debug.LogError($"No field found with the same name " +
                $"{output.FieldName} for type {output.Node.GetType()}");
            return;
        }

        foreach (Connector input in m_Connections[output])
        {
            if (TryGetInputProperty(input, out FieldInfo inputField) == false)
            {
                Debug.LogWarning($"No field found with the same name {input.FieldName} for type {input.Node.GetType()}");
                continue;
            }

            Type inputFieldType = inputField.FieldType;

            if (inputFieldType.IsArray || inputFieldType.IsList())
            {
                List<object> values = new List<object>();

                foreach (Connector outputConnector in m_Connections.Keys)
                {
                    if (m_Connections[outputConnector].Contains(input))
                    {
                        if (TryGetOutputProperty(outputConnector, out FieldInfo field) == false)
                        {
                            Debug.LogWarning($"No field found with the same name {output.FieldName} for type {output.Node.GetType()}");
                            return;
                        }

                        Type outputFieldType = field.GetType();
                        object value = field.GetValue(outputConnector.Node);

                        if (outputFieldType.IsArray)
                        {
                            Array array = (Array)value;

                            for (int i = 0; i < array.Length; i++)
                                values.Add(array.GetValue(i));
                        }
                        else if (outputFieldType.IsList())
                        {
                            IList list = (IList)value;

                            foreach (object element in list)
                                values.Add(element);
                        }
                        else
                        {
                            values.Add(value);
                        }
                    }
                }

                if (inputFieldType.IsArray)
                {
                    Array result = Array.CreateInstance(inputFieldType.GetElementType(), values.Count);

                    for (int i = 0; i < result.Length; i++)
                        result.SetValue(values[i], i);

                    inputField.SetValue(input.Node, result);
                }
                else
                {
                    inputField.SetValue(input.Node, values);
                }
            }
            else if (outputField.FieldType == inputFieldType)
            {
                object value = outputField.GetValue(output.Node);
                inputField.SetValue(input.Node, value);
            }
        }
    }

    public void UpdateData(Node node)
    {
        if (m_Connectors.ContainsKey(node) == false)
        {
            Debug.LogError("");
            return;
        }

    }

    public static bool TryGetInputProperty(Connector connector, out FieldInfo field)
    {
        bool result = false;
        field = null;

        if (connector.Node == null)
            return result;

        if (connector.IsSubGraph)
        {
            NodePropertyCasher.TryGetOutputPropertes(connector.Node.GetType(), out FieldInfo[] fields);

            result = fields.Length > 0;

            if (result)
                field = fields[0];
        }
        else
        {
            result = NodePropertyCasher.TryGetInputProperty(connector.Node.GetType(),
                connector.FieldName, out FieldInfo outputField);

            field = outputField;
        }

        return result;
    }

    public static bool TryGetOutputProperty(Connector connector, out FieldInfo field)
    {
        bool result = false;
        field = null;

        Debug.Log(connector);

        if (connector.Node == null)
            return result;

        if (connector.IsSubGraph)
        {
            NodePropertyCasher.TryGetInputPropertes(connector.Node.GetType(), out FieldInfo[] fields);

            result = fields.Length > 0;
            Debug.Log(connector.Node);

            if (result)
                field = fields[0];
        }
        else
        {
            result = NodePropertyCasher.TryGetOutputProperty(connector.Node.GetType(), connector.FieldName, out FieldInfo outputField);
            field = outputField;
        }

        return result;
    }

    public void UpdateData(FieldInfo outputField, FieldInfo inputField)
    {

    }

    public bool CanConnect(Connector output, Connector input)
    {
        Type outputConnectorType = null;
        Type inputConnectorType = null;

        if (TryGetOutputProperty(output, out FieldInfo outputField))
        {
            outputConnectorType = outputField.FieldType;
        }

        if (TryGetInputProperty(input, out FieldInfo inputField))
        {
            inputConnectorType = inputField.FieldType;
        }

        bool result = outputConnectorType == inputConnectorType;

        Debug.Log(result);

        if (outputConnectorType != inputConnectorType)
        {
            Type outputFieldElementType = null;
            Type inputFieldElementType = null;

            if (outputConnectorType.IsArray)
                outputFieldElementType = outputConnectorType.GetElementType();
            else if (outputConnectorType.IsList())
                outputFieldElementType = outputConnectorType.GetGenericArguments()[0];

            if (inputConnectorType.IsArray)
                inputFieldElementType = inputConnectorType.GetElementType();
            else if (inputConnectorType.IsList())
                inputFieldElementType = inputConnectorType.GetGenericArguments()[0];

            if (outputFieldElementType != null && inputFieldElementType != null)
                result = outputFieldElementType == inputFieldElementType;
            else if (outputFieldElementType == null && inputFieldElementType != null)
                result = outputConnectorType == inputFieldElementType;
        }

        if (m_Connections.ContainsKey(output) && result == true)
            result = !m_Connections[output].Contains(input);

        return result;
    }

    private void FixConnector(Connector connector, List<Node> nodes)
    {
        if (nodes.Contains(connector.Node) == false)
        {
            RemoveConnection(connector);
            m_Connectors.Remove(connector.Node);
            return;
        }

        Type nodeType = connector.Node.GetType();

        if (NodePropertyCasher.TryGetInputProperty(nodeType, connector.FieldName,
            out FieldInfo field) == false)
        {
            RemoveConnection(connector);
        }

        if (m_Connectors[connector.Node].Contains(connector))
            m_Connectors[connector.Node].Remove(connector);
    }
}

public struct Connection
{
    public Connector Output { get; private set; }
    public Connector Input { get; private set; }

    //public Connection(Port output, Port input)
    //{
    //    if (output.PortType != PortType.Output
    //        && input.PortType != PortType.Input)
    //        throw new Exception();

    //    Connector connectorOutput = new Connector();
    //    connectorOutput.Init(output.Node, output.FieldName);

    //    Connector connectorInput = new Connector();
    //    connectorInput.Init(input.Node, input.FieldName);

    //    Output = connectorOutput;
    //    Input = connectorInput;
    //}

    public Connection(Connector output, Connector input)
    {
        Output = output;
        Input = input;
    }

    public static bool operator ==(Connection a, Connection b)
    {
        return a.Output == b.Output
            && a.Input == b.Input;
    }

    public static bool operator !=(Connection a, Connection b)
    {
        return a.Output != b.Output
            && a.Input != b.Input;
    }

    public override bool Equals(object obj)
    {
        if (obj is Connection connection)
            return this == connection;

        return false;
    }

    public override int GetHashCode()
    {
        return Input.GetHashCode() + Output.GetHashCode();
    }
}

[SerializebleObject]
public class Connector
{
    [SerializebleProperty] private Node m_Node;
    [SerializebleProperty] private string m_FieldName;
    [SerializebleProperty] private bool m_IsSubGraph;

    public Node Node => m_Node;
    public string FieldName => m_FieldName;
    public bool IsSubGraph => m_IsSubGraph;

    public void Init(Node node, string fieldName, bool isSubGraph = false)
    {
        m_Node = node;
        m_FieldName = fieldName;
        m_IsSubGraph = isSubGraph;
    }

    public Type GetNodeType() => m_Node.GetType();

    public static bool operator ==(Connector a, Connector b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        return a.Node == b.Node
            && a.FieldName == b.FieldName;
    }

    public static bool operator !=(Connector a, Connector b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return $"({Node.GetType()} : {FieldName})";
    }

    public override bool Equals(object obj)
    {
        Connector connector = (Connector)obj;

        if (connector == null)
            return false;

        if (FieldName == connector.FieldName
            && Node == connector.Node)
            return true;

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Node, FieldName);
    }
}

public class SearchWindowStingProvider : ScriptableObject, ISearchWindowProvider
{
    private string[] m_Items;
    public event Action<string> OnSelect;

    public void Init(string[] elements, Action<string> action)
    {
        m_Items = elements;
        OnSelect += action;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>();

        List<string> sortedListItems = m_Items.ToList();
        sortedListItems.Sort((a, b) =>
        {
            string[] splitedA = a.Split("/");
            string[] splitedB = b.Split("/");

            for (int i = 0; i < splitedA.Length; i++)
            {
                if (i >= splitedB.Length)
                    return 1;

                int value = splitedA[i].CompareTo(splitedB[i]);

                if (value != 0)
                {
                    if (splitedA.Length != splitedB.Length &&
                    (i == splitedA.Length - 1 || i == splitedB.Length - 1))
                    {
                        return splitedA.Length < splitedB.Length ? 1 : -1;
                    }

                    return value;
                }
            }

            return 0;
        });

        List<string> grups = new List<string>();

        foreach (string item in sortedListItems)
        {
            string[] entryTitle = item.Split("/");

            for (int i = 0; i < entryTitle.Length - 1; i++)
            {
                if (grups.Contains(entryTitle[i]) == false)
                {
                    searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i));
                    grups.Add(entryTitle[i]);
                    //Debug.Log(entryTitle[i]);
                }
            }

            SearchTreeEntry searchTreeEntry = new SearchTreeEntry(new GUIContent(entryTitle.Last()));
            searchTreeEntry.level = entryTitle.Length - 1;
            searchTreeEntry.userData = item;
            searchTreeEntries.Add(searchTreeEntry);
        }

        return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        OnSelect?.Invoke(SearchTreeEntry.userData.ToString());
        return true;
    }
}

public struct NodeSearchWindowElement
{
    public string Path { get; private set; }
    public Type NodeType { get; private set; }
    public bool IsOnlySubGraph { get; private set; }

    public NodeSearchWindowElement(string path, Type nodeType, bool isOnlySubGraph)
    {
        Path = path;
        NodeType = nodeType;
        IsOnlySubGraph = isOnlySubGraph;
    }
}

public static class NodeRegisterToSearchWindow
{
    private static Dictionary<Type, List<NodeSearchWindowElement>> s_RegisteredNode = new Dictionary<Type, List<NodeSearchWindowElement>>();

    [InitializeOnLoadMethod]
    public static void Register()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(Node).IsAssignableFrom(type))
                RegisterNode(type);
        }
    }

    private static void RegisterNode(Type type)
    {
        GraphCreateWindow graphElement = (GraphCreateWindow)Attribute.GetCustomAttribute(type, typeof(GraphCreateWindow));

        if (graphElement == null)
            return;

        if (s_RegisteredNode.ContainsKey(graphElement.GraphType) == false)
            s_RegisteredNode.Add(graphElement.GraphType, new List<NodeSearchWindowElement>());

        NodeSearchWindowElement element = new NodeSearchWindowElement(graphElement.Path,
            type, graphElement.IsOnlySubGraph);

        s_RegisteredNode[graphElement.GraphType].Add(element);
    }

    public static string[] GetPaths(Type graphType, bool isSubGraph)
    {
        List<string> result = new List<string>();

        foreach (NodeSearchWindowElement element in s_RegisteredNode[graphType])
        {
            if (element.IsOnlySubGraph && isSubGraph == false)
                continue;

            result.Add(element.Path);
        }

        return result.ToArray();
    }

    public static Type GetType(Type graphType, string path)
    {
        foreach (NodeSearchWindowElement element in s_RegisteredNode[graphType])
        {
            if (element.Path != path)
                continue;

            return element.NodeType;
        }

        return null;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GraphCreateWindow : Attribute
{
    public readonly Type GraphType;
    public readonly string Path;
    public readonly bool IsOnlySubGraph = false;

    public GraphCreateWindow(Type graphType, string path)
    {
        GraphType = graphType;
        Path = path;
    }

    public GraphCreateWindow(Type graphType, string path, bool isOnlySubGraph)
    {
        GraphType = graphType;
        Path = path;
        IsOnlySubGraph = isOnlySubGraph;
    }
}

[InitializeOnLoad]
public static class EnigmaticStyles
{
    private static GUIStyle sm_FoldoutButtonClose;
    private static GUIStyle sm_FoldoutButtonOpen;
    private static GUIStyle sm_FoldoutBackground;
    private static GUIStyle sm_ToolBarButton;

    private static GUIStyle sm_WhiteBox;

    private static GUIStyle sm_AddButton;
    private static GUIStyle sm_SubstractButton;

    private static GUIStyle sm_AddFoldoutButton;

    private static GUIStyle sm_NodeTitle;
    private static GUIStyle sm_NodeLitleTitle;

    private static GUIStyle sm_NodeBox;
    private static GUIStyle sm_NewNodeBox;
    private static GUIStyle sm_SNewNodeBox;
    private static GUIStyle sm_NodeMidelBox;
    private static GUIStyle sm_NodeBG;
    private static GUIStyle sm_SelectedOutLine;
    private static GUIStyle sm_NodeOutLine;
    private static GUIStyle sm_SelectedNodeBox;
    private static GUIStyle sm_Port;

    private static GUIStyle sm_InputDataIcon;
    private static GUIStyle sm_OutputDataIcon;
    private static GUIStyle sm_Parametor;
    private static GUIStyle sm_SubgraphNodeIcon;

    private static GUIStyle sm_NodeEditorWindowIcon;
    private static GUIStyle sm_NodeEditorInspectorWindowIcon;

    private static GUIStyle sm_ErrorIcon;
    private static GUIStyle sm_WarningIcon;

    public static GUIStyle errorIcon
    {
        get
        {
            if(sm_ErrorIcon == null)
            {
                sm_ErrorIcon = new GUIStyle(GUI.skin.box);
                sm_ErrorIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/ErrorIcon.png") as Texture2D;
            }

            return sm_ErrorIcon;
        }
    }

    public static GUIStyle nodeEditorWindowIcon
    {
        get
        {
            if (sm_NodeEditorWindowIcon == null)
            {
                sm_NodeEditorWindowIcon = new GUIStyle(GUI.skin.box);
                sm_NodeEditorWindowIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeEditorGraphWindowIcon.png") as Texture2D;
            }

            return sm_NodeEditorWindowIcon;
        }
    }

    public static GUIStyle nodeEditorInspectorWindowIcon
    {
        get
        {
            if (sm_NodeEditorInspectorWindowIcon == null)
            {
                sm_NodeEditorInspectorWindowIcon = new GUIStyle(GUI.skin.box);
                sm_NodeEditorInspectorWindowIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeEditorGraphInspectorWindowIcon.png") as Texture2D;
            }

            return sm_NodeEditorInspectorWindowIcon;
        }
    }

    public static GUIStyle whiteBox
    {
        get
        {
            if (sm_WhiteBox == null)
            {
                Texture2D whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, new Color(1, 1, 1, 0.05f));
                whiteTexture.Apply();

                sm_WhiteBox = new GUIStyle(GUI.skin.box);
                sm_WhiteBox.normal.background = whiteTexture;
            }

            return sm_WhiteBox;
        }
    }

    public static GUIStyle foldoutButtonClose
    {
        get
        {
            if (sm_FoldoutButtonClose == null)
            {
                sm_FoldoutButtonClose = new GUIStyle(GUI.skin.box);
                sm_FoldoutButtonClose.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/FoldoutButtonClose.png") as Texture2D;
            }

            return sm_FoldoutButtonClose;
        }
    }

    public static GUIStyle foldoutButtonOpen
    {
        get
        {
            if (sm_FoldoutButtonOpen == null)
            {
                sm_FoldoutButtonOpen = new GUIStyle(GUI.skin.box);
                sm_FoldoutButtonOpen.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/FoldoutButtonOpen.png") as Texture2D;
            }

            return sm_FoldoutButtonOpen;
        }
    }

    public static GUIStyle foldoutBackground
    {
        get
        {
            if (sm_FoldoutBackground == null)
            {
                sm_FoldoutBackground = new GUIStyle(GUI.skin.box);
                sm_FoldoutBackground.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/FoldoutBackground.png") as Texture2D;
                sm_FoldoutBackground.border = new RectOffset(9, 9, 9, 9); // Регулируйте отступы, чтобы избежать обрезания текста
                sm_FoldoutBackground.padding = new RectOffset(7, 7, 7, 7);
            }

            return sm_FoldoutBackground;
        }
    }

    public static GUIStyle toolBarButton
    {
        get
        {
            if (sm_ToolBarButton == null)
            {
                sm_ToolBarButton = new GUIStyle(GUI.skin.box);
                sm_ToolBarButton.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/ToolBarButton.png") as Texture2D;
                sm_ToolBarButton.border = new RectOffset(17, 17, 2, 2); // Регулируйте отступы, чтобы избежать обрезания текста
                sm_ToolBarButton.padding = new RectOffset(6, 6, 6, 6);
            }

            return sm_ToolBarButton;
        }
    }

    public static GUIStyle addButton
    {
        get
        {
            if (sm_AddButton == null)
            {
                sm_AddButton = new GUIStyle(GUI.skin.box);
                sm_AddButton.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/AddButton.png") as Texture2D;
            }

            return sm_AddButton;
        }
    }

    public static GUIStyle addFoldoutButton
    {
        get
        {
            if (sm_AddFoldoutButton == null)
            {
                sm_AddFoldoutButton = new GUIStyle(GUI.skin.box);
                sm_AddFoldoutButton.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/AddButtonFoldout.png") as Texture2D;
            }

            return sm_AddFoldoutButton;
        }
    }

    public static GUIStyle substractButton
    {
        get
        {
            if (sm_SubstractButton == null)
            {
                sm_SubstractButton = new GUIStyle(GUI.skin.box);
                sm_SubstractButton.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/SubstractButton.png") as Texture2D;
            }

            return sm_SubstractButton;
        }
    }

    public static GUIStyle nodeTitleStyle
    {
        get
        {
            if (sm_NodeTitle == null)
            {
                sm_NodeTitle = new GUIStyle(GUI.skin.label);

                sm_NodeTitle.fontSize = 13;
                sm_NodeTitle.fontStyle = FontStyle.Bold;
                sm_NodeTitle.alignment = TextAnchor.MiddleCenter;
            }

            return sm_NodeTitle;
        }
    }

    public static GUIStyle nodeLitleTitleStyle
    {
        get
        {
            if (sm_NodeLitleTitle == null)
            {
                sm_NodeLitleTitle = new GUIStyle(GUI.skin.label);

                sm_NodeLitleTitle.fontSize = 8;
                sm_NodeLitleTitle.alignment = TextAnchor.MiddleCenter;
            }

            return sm_NodeLitleTitle;
        }
    }

    public static GUIStyle nodeBox
    {
        get
        {
            if (sm_NodeBox == null)
            {
                sm_NodeBox = new GUIStyle(GUI.skin.box);
                sm_NodeBox.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/node_gray.png") as Texture2D;
                sm_NodeBox.border = new RectOffset(12, 12, 12, 12); // Регулируйте отступы, чтобы избежать обрезания текста
            }

            return sm_NodeBox;
        }
    } //Delete

    public static GUIStyle newNodeBox
    {
        get
        {
            if (sm_NewNodeBox == null)
            {
                sm_NewNodeBox = new GUIStyle(GUI.skin.box);
                sm_NewNodeBox.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/node.png") as Texture2D;
                sm_NewNodeBox.border = new RectOffset(16, 16, 16, 16); // Регулируйте отступы, чтобы избежать обрезания текста
            }

            return sm_NewNodeBox;
        }
    } // Rename

    public static GUIStyle SnewNodeBox
    {
        get
        {
            if (sm_SNewNodeBox == null)
            {
                sm_SNewNodeBox = new GUIStyle(GUI.skin.box);
                sm_SNewNodeBox.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeBox.png") as Texture2D;
                sm_SNewNodeBox.border = new RectOffset(22, 22, 45, 22); // Регулируйте отступы, чтобы избежать обрезания текста
                sm_SNewNodeBox.padding = new RectOffset(12, 12, 12, 12); // Регулируйте отступы, чтобы избежать обрезания текста
            }

            return sm_SNewNodeBox;
        }
    } //Rename

    public static GUIStyle nodeBG
    {
        get
        {
            if (sm_NodeBG == null)
            {
                sm_NodeBG = new GUIStyle(GUI.skin.box);
                sm_NodeBG.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeBG.png") as Texture2D;
                sm_NodeBG.border = new RectOffset(16, 16, 16, 16); // Регулируйте отступы, чтобы избежать обрезания текста
            }

            return sm_NodeBG;
        }
    } //Delete

    public static GUIStyle nodeMidelBox
    {
        get
        {
            if (sm_NodeMidelBox == null)
            {
                sm_NodeMidelBox = new GUIStyle(GUI.skin.box);
                sm_NodeMidelBox.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeMidel.png") as Texture2D;
                sm_NodeMidelBox.border = new RectOffset(16, 16, 16, 16); // Регулируйте отступы, чтобы избежать обрезания текста
            }

            return sm_NodeMidelBox;
        }
    } //Delete

    public static GUIStyle selectedOutLine
    {
        get
        {
            if (sm_SelectedOutLine == null)
            {
                sm_SelectedOutLine = new GUIStyle(GUI.skin.box);
                sm_SelectedOutLine.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeSelected.png") as Texture2D;
                sm_SelectedOutLine.border = new RectOffset(16, 16, 16, 16); // Регулируйте отступы, чтобы избежать обрезания текста
            }

            return sm_SelectedOutLine;
        }
    }

    public static GUIStyle nodeOutLine
    {
        get
        {
            if (sm_NodeOutLine == null)
            {
                sm_NodeOutLine = new GUIStyle(GUI.skin.box);
                sm_NodeOutLine.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/NodeOutline.png") as Texture2D;
                sm_NodeOutLine.border = new RectOffset(16, 16, 16, 16);
            }

            return sm_NodeOutLine;
        }
    } //Delete

    public static GUIStyle selectedNodeBox
    {
        get
        {
            if (sm_SelectedNodeBox == null)
            {
                sm_SelectedNodeBox = new GUIStyle(GUI.skin.box);
                sm_SelectedNodeBox.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/node_blue.png") as Texture2D;
                sm_SelectedNodeBox.border = new RectOffset(12, 12, 12, 12); // Регулируйте отступы, чтобы избежать обрезания текста
            }

            return sm_SelectedNodeBox;
        }
    } //Retexture

    public static GUIStyle Port
    {
        get
        {
            if (sm_Port == null)
            {
                sm_Port = new GUIStyle();
                sm_Port.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/Port.png") as Texture2D;
            }

            return sm_Port;
        }
    }

    public static GUIStyle inputData
    {
        get
        {
            if (sm_InputDataIcon == null)
            {
                sm_InputDataIcon = new GUIStyle();
                sm_InputDataIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/InputData.png") as Texture2D;
            }

            return sm_InputDataIcon;
        }
    }

    public static GUIStyle outputData
    {
        get
        {
            if (sm_OutputDataIcon == null)
            {
                sm_OutputDataIcon = new GUIStyle();
                sm_OutputDataIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/OutputData.png") as Texture2D;
            }

            return sm_OutputDataIcon;
        }
    }

    public static GUIStyle parameter
    {
        get
        {
            if (sm_Parametor == null)
            {
                sm_Parametor = new GUIStyle();
                sm_Parametor.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/Parametor.png") as Texture2D;
            }

            return sm_Parametor;
        }
    }

    public static GUIStyle subgraphNodeIcon
    {
        get
        {
            if (sm_SubgraphNodeIcon == null)
            {
                sm_SubgraphNodeIcon = new GUIStyle();
                sm_SubgraphNodeIcon.normal.background = EditorGUIUtility.Load("Assets/Enigmatic/Source/Editor/Texture/SubGraphNode.png") as Texture2D;
            }

            return sm_SubgraphNodeIcon;
        }
    }
}

public class EnigmaticStylesSettings : ScriptableObject
{
    private GUIStyle m_NodeTitleStyle;

    public GUIStyle NodeTitleStyle
    {
        get
        {
            if (m_NodeTitleStyle == null)
            {
                m_NodeTitleStyle = new GUIStyle(GUI.skin.label);

                m_NodeTitleStyle.fontSize = 15;
                m_NodeTitleStyle.fontStyle = FontStyle.Bold;
                m_NodeTitleStyle.alignment = TextAnchor.MiddleCenter;

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }

            return m_NodeTitleStyle;
        }
    }
}

public static class RectExtensions
{
    /// <summary>
    /// Scale Rect arount top left pivod
    /// </summary>
    public static Rect ScaleSize(this Rect rect, float scale)
    {
        return rect.ScaleSize(scale, rect.TopLeft());
    }

    /// <summary>
    /// Sacle rect arount pivot
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="scale"></param>
    /// <param name="pivotPosition"></param>
    /// <returns></returns>
    public static Rect ScaleSize(this Rect rect, float scale, Vector2 pivotPosition)
    {
        Rect result = rect;

        result.x += pivotPosition.x;
        result.y += pivotPosition.y;

        result.xMin *= scale;
        result.xMax *= scale;

        result.yMin *= scale;
        result.yMax *= scale;

        result.x += pivotPosition.x;
        result.y += pivotPosition.y;

        return result;
    }

    public static Vector2 TopLeft(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }

    public static bool Overlap(this Rect rect1, Rect rect2)
    {
        if (rect1.x + rect1.width < rect2.x || rect2.x + rect2.width < rect1.x
            || rect1.y + rect1.height < rect2.y || rect2.y + rect2.height < rect1.y)
        {
            return false;
        }

        return true;
    }

    public static Vector2 Center(this Rect rect)
    {
        return rect.position + rect.size / 2;
    }
}

public static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(dictionary.Count);

        foreach (TKey key in dictionary.Keys)
            result.Add(key, dictionary[key]);

        return result;
    }
}

public static class ListExtansions
{
    public static List<T> Clone<T>(this List<T> list)
    {
        List<T> tempList = new List<T>(list.Count);

        foreach (T item in list)
            tempList.Add(item);

        return tempList;
    }

    public static List<T> Combine<T>(this List<T> listA, List<T> listB)
    {
        List<T> result = new List<T>(listA.Count + listB.Count);

        foreach (T value in listA)
            result.Add(value);

        foreach (T value in listB)
            result.Add(value);

        return result;
    }

    public static List<T> CombineNoSeem<T>(this List<T> list, T[] array)
    {
        List<T> result = new List<T>(list.Count + array.Length);

        result.AddRange(list);

        foreach (T item in array)
        {
            if (result.Contains(item))
                continue;

            result.Add(item);
        }

        return result;
    }
}