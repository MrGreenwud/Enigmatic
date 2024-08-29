using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Enigmatic;
using Enigmatic.Experemental.ENIX;
using Enigmatic.Core;

[SerializebleObject]
public class NodeGraphEditor
{
    public enum ActionMode
    {
        None,
        DragingNode,
        SelectionBox,
        PortConnected
    }

    public static NodeGraphEditor Last { get; private set; }
    public static NodeGraphEditor Active { get; private set; }
    
    public static NodeGraph ActiveNodeGraph => Active.NodeGraph;
    public static PortManager ActivePortManager => Active.PortManager;
    public static ConnectionManager ActiveConnectionManager => ActiveNodeGraph.ConnectionManager;

    [SerializebleProperty] private Dictionary<Node, NodeEditorView> m_RegisteredNodes = new Dictionary<Node, NodeEditorView>();
    [SerializebleProperty] private Dictionary<SubGraphNode, NodeGraphEditor> m_SubGraphNodeEditors = new Dictionary<SubGraphNode, NodeGraphEditor>();

    [SerializebleProperty] private float m_ZoomScale = 1;
    [SerializebleProperty] private Vector2 m_ZoomCordsOrigin;

    [SerializebleProperty] private bool m_IsSubGraph;

    public bool IsInit;
    public bool IsChangedEditor;

    private Rect WindowRect;
    private ActionMode m_ActionMode;
    private List<Node> m_SelectionNode = new List<Node>();
    private List<Port> m_SelectedPorts = new List<Port>();

    public float ZoomScale
    {
        get { return m_ZoomScale; }
        set 
        {
            IsChangedEditor = true;
            m_ZoomScale = value; 
        }
    }

    public Vector2 ZoomCordsOrigin
    {
        get { return m_ZoomCordsOrigin; }
        set
        {
            IsChangedEditor = true;
            m_ZoomCordsOrigin = value;
        }
    }

    public NodeGraph NodeGraph { get; private set; }
    public PortManager PortManager { get; private set; }
    public bool IsSubGraph => m_IsSubGraph;

    private Rect m_SelectBox
    {
        get
        {
            Vector2 mousePosition = EditorInput.GetMousePosition();
            Vector2 lastMousePosition = EditorInput.GetLastMousePosition();

            Vector2 size = Vector2.zero;
            Vector2 position = Vector2.zero;

            if (lastMousePosition.y < mousePosition.y)
            {
                size.y = mousePosition.y - lastMousePosition.y;
                position.y = lastMousePosition.y;
            }
            else
            {
                size.y = lastMousePosition.y - mousePosition.y;
                position.y = mousePosition.y;
            }

            if (lastMousePosition.x < mousePosition.x)
            {
                size.x = mousePosition.x - lastMousePosition.x;
                position.x = lastMousePosition.x;
            }
            else
            {
                size.x = lastMousePosition.x - mousePosition.x;
                position.x = mousePosition.x;
            }

            return new Rect(position, size);
        }
    }

    public event Action<Connector, Connector> OnChangeConnection;
    public event Action OnUpdateState;

    public static event Action OnOpenGraph;

    public static void Open(NodeGraphEditor nodeGraphEditor)
    {
        if (nodeGraphEditor.IsInit == false)
        {
            Debug.LogError("");
            return;
        }

        if(Active != null)
        {
            nodeGraphEditor.ZoomCordsOrigin = Active.ZoomCordsOrigin;
            nodeGraphEditor.ZoomScale = Active.ZoomScale;
        }

        Last = Active;
        Active = nodeGraphEditor;

        Active.NodeGraph.ConnectionManager.FixConnectors();
        Active.ReregisterNodes();
        Active.PortManager.LoopThroughPorts(Active.FixPort);

        OnOpenGraph?.Invoke();
    }

    public void Init(NodeGraph nodeGraph)
    {
        NodeGraphEditor lastActive = Active;
        Active = this;

        NodeGraph = nodeGraph;
        PortManager = new PortManager();

        ReregisterNodes();

        Dictionary<Node, List<string>> connectors = PortManager.GetConnectors();
        NodeGraph.ConnectionManager.LoopThroughConnections(UpdatePortView);
        OnChangeConnection += UpdatePortView;

        IsChangedEditor = true;
        IsInit = true;

        Active = lastActive;
    }

    public void Run()
    {
        HendleInput();

        EnigmaticGUI.BeginZoomedArea(ZoomScale, NodeEditorWindow.ActiveWindow.ZoomedArea);

        OnChangedEditor();
        Draw();
        Update();

        EnigmaticGUI.EndZoomedArea(ZoomScale, NodeEditorWindow.ActiveWindow.position);

        BezierCasher.RemoveUnused("NodeGraph");
    }

    private void HendleInput()
    {
        if (Event.current.type == EventType.ScrollWheel)
        {
            float oldZoom = ZoomScale;
            float zoom = ZoomScale;

            Vector2 screenCoordsMousePosition = NodeEditorWindow.ActiveWindow.ZoomedArea.size / 2;

            Vector2 zoomCordsMousePosition
                = EnigmaticGUI.ConvertScreenCoordsToZoomCoords
                (screenCoordsMousePosition, NodeEditorWindow.ActiveWindow.ZoomedArea, zoom, ZoomCordsOrigin);

            zoom -= 0.1f * ZoomScale * Math.Sign(Event.current.delta.y);

            ZoomScale = Math.Clamp(zoom, 0.5f, 3f);

            ZoomCordsOrigin +=
                (zoomCordsMousePosition - ZoomCordsOrigin) 
                - (oldZoom / ZoomScale) * (zoomCordsMousePosition - ZoomCordsOrigin);

            NodeEditorWindow.CustomRepaint();
        }

        if (Event.current.button == 2 && Event.current.type == EventType.MouseDrag)
        {
            ZoomCordsOrigin -= Event.current.delta / ZoomScale;
            NodeEditorWindow.CustomRepaint();
        }
    }

    protected virtual void OnChangedEditor()
    {
        if (IsChangedEditor == false)
            return;

        foreach (Node node in m_RegisteredNodes.Keys)
            m_RegisteredNodes[node].UpdateView();

        IsChangedEditor = false;
    }

    public virtual void Update()
    {
        EditorInput.UpdateInput();

        if(EditorInput.GetMouseButtonDown(1))
            AddNode();
        
        if(EditorInput.GetButtonDown(KeyCode.X))
            DeleteNode();

        PortManager.LoopThroughPorts(ActionWithPort);
        
        if (EditorInput.GetMouseButtonUp(0) && m_ActionMode == ActionMode.PortConnected)
            DeselectAllPorts();

        CheckDraging();
        SelectNode();

        CheckDraging();
        DragNodes();

        if (EditorInput.GetButtonDown(KeyCode.A))
        {
            if (m_SelectionNode.Count == m_RegisteredNodes.Count)
                DeselectAllNodes();
            else
                SelectAllNode();
        }

        if(EditorInput.GetButtonDown(KeyCode.F))
            FocusOnSelectedNode();

        ValidMouseInputState();
    }

    public virtual void Draw()
    {
        Vector2 size = NodeEditorWindow.ActiveWindow.position.size * (1 / m_ZoomScale);
        WindowRect = new Rect(Vector2.zero, size);
        GUI.Box(WindowRect, "", "box");

        Debug.Log("Draw");
        NodeGraph.ConnectionManager.LoopThroughConnections(DrawConnection);

        if (m_ActionMode == ActionMode.PortConnected)
        {
            Debug.Log("Connecting!");

            foreach (Port port in m_SelectedPorts)
            {
                Rect mouseRect = new Rect(EditorInput.GetMousePosition(), Vector2.one);
                Direction direction = Direction.HorizontalRight;

                if (port.PortType == PortType.Input)
                    direction = Direction.HorizontalLeft;

                Color color = new Color(0.705f, 0.639f, 0.752f);

                DrawConnection(port.Rect, mouseRect, direction, Color.white);
                NodeEditorWindow.CustomRepaint();
            }
        }

        DrawNods();

        if (m_ActionMode == ActionMode.SelectionBox)
            DrawSelectionBox();
    }

    public void LoopThroughSelectionNods(Action<Node> action)
    {
        foreach (Node node in m_SelectionNode)
            action?.Invoke(node);
    }

    public bool TryGetNodeView(Node node, out NodeEditorView view)
    {
        view = null;

        if (m_RegisteredNodes.ContainsKey(node) == false)
            return false;

        view = m_RegisteredNodes[node];
        return true;
    }

    public void DrawSelectionBox()
    {
        Handles.color = new Color(0.9431f, 0.6215f, 0.0452f, 1f);

        GUI.Box(m_SelectBox, "", "box");

        float screenSpaceSize = 20000f;

        Handles.DrawDottedLine(m_SelectBox.position, new Vector3(m_SelectBox.xMax, m_SelectBox.yMin), screenSpaceSize);
        Handles.DrawDottedLine(m_SelectBox.position, new Vector3(m_SelectBox.xMin, m_SelectBox.yMax), screenSpaceSize);
        Handles.DrawDottedLine(new Vector3(m_SelectBox.xMax, m_SelectBox.yMin), new Vector3(m_SelectBox.xMax, m_SelectBox.yMax), screenSpaceSize);
        Handles.DrawDottedLine(new Vector3(m_SelectBox.xMin, m_SelectBox.yMax), new Vector3(m_SelectBox.xMax, m_SelectBox.yMax), screenSpaceSize);

        NodeEditorWindow.CustomRepaint();
    }

    public void DrawNods()
    {
        foreach (NodeEditorView view in m_RegisteredNodes.Values)
            view.Draw(true);//WindowRect.Overlap(view.Rect)
    }

    public void ActionWithPort(Port port)
    {
        if (port.Rect.Contains(EditorInput.GetMousePosition()))
        {
            if (EditorInput.GetMouseButtonDown(0))
            {
                if (EditorInput.GetButtonPress(KeyCode.LeftShift))
                    DisconnectPort(port);
                else if (EditorInput.GetButtonPress(KeyCode.LeftControl))
                    DisconnectPortMulti(port);
                else
                    SelectPort(port);
            }
            else if (EditorInput.GetMouseButtonUp(0))
            {
                ConnectPort(port);
            }
        }
    }

    public void SelectPort(Port port)
    {
        m_SelectedPorts.Add(port);
        m_ActionMode = ActionMode.PortConnected;
        Debug.Log("SELECT PORT");
    }

    public void DisconnectPort(Port port)
    {
        Connector connector = port.Connector;
        Connector[] connectors = NodeGraph.ConnectionManager.GetConnections(connector);

        if (connectors.Length == 0)
            return;

        Connector output = null;
        Connector input = null;

        if (port.PortType == PortType.Output)
        {
            output = connector;
            input = connectors[0];
        }
        else
        {
            output = connectors[0];
            input = connector;
        }

        NodeGraph.ConnectionManager.RemoveConnection(output, input);
        Port otherPort = PortManager.GetPort(connectors[0]);
        SelectPort(otherPort);

        OnChangeConnection?.Invoke(output, input);
    }

    public void DisconnectPortMulti(Port port)
    {
        Connector connector = port.Connector;
        Connector[] connectors = NodeGraph.ConnectionManager.GetConnections(connector);

        if (connectors.Length == 0)
            return;

        for (int i = 0; i < connectors.Length; i++)
        {
            Connector output = null;
            Connector input = null;

            if (port.PortType == PortType.Output)
            {
                output = connector;
                input = connectors[i];
            }
            else
            {
                output = connectors[i];
                input = connector;
            }

            NodeGraph.ConnectionManager.RemoveConnection(output, input);
            Port otherPort = PortManager.GetPort(connectors[i]);
            SelectPort(otherPort);

            OnChangeConnection?.Invoke(output, input);
        }
    }

    public void ConnectPort(Port port)
    {
        if (m_SelectedPorts.Count > 0)
        {
            foreach (Port p in m_SelectedPorts)
            {
                if (p.TryConnect(port, out Connection connection))
                {
                    NodeGraph.ConnectionManager.AddConnection(connection);

                    OnChangeConnection?.Invoke(connection.Output, connection.Input);
                    OnUpdateState?.Invoke();

                    NodeEditorWindow.CustomRepaint();
                }
            }
        }

        DeselectAllPorts();
    }

    public void DeselectAllPorts()
    {
        m_SelectedPorts.Clear();
        m_ActionMode = ActionMode.None;
    }

    public void DragNodes()
    {
        if (m_ActionMode == ActionMode.SelectionBox)
            return;

        if (m_ActionMode != ActionMode.DragingNode)
            return;

        NodeEditorView originNodeView = m_RegisteredNodes[m_SelectionNode[0]];

        Vector2 mouseDelta = EditorInput.GetLastMousePosition() - EditorInput.GetMousePosition();
        Vector2 newPositionOriginNode = originNodeView.Transform.LastPosition - mouseDelta;

        newPositionOriginNode.x = Mathf.Round(newPositionOriginNode.x / 20) * 20;
        newPositionOriginNode.y = Mathf.Round(newPositionOriginNode.y / 20) * 20;

        originNodeView.Transform.Move(newPositionOriginNode);

        Vector2 offset = (originNodeView.Transform.LastPosition - originNodeView.Transform.OriginPosition);

        foreach (Node node in m_SelectionNode)
        {
            NodeEditorView view = m_RegisteredNodes[node];

            Vector2 newPosition = view.Transform.LastPosition - offset;
            view.Transform.Move(newPosition);
        }

        if (EditorInput.GetMouseButtonUp(0))
        {
            m_ActionMode = ActionMode.None;
        }

        IsChangedEditor = true;
        NodeEditorWindow.CustomRepaint();
    }

    private void CheckDraging()
    {
        if (EditorInput.GetMouseButtonDown(0))
        {
            EditorInput.UpdateLastMousePosition();

            foreach (Node node in m_SelectionNode)
            {
                NodeEditorView view = m_RegisteredNodes[node];

                if (view.DragingArea.Contains(EditorInput.GetMousePosition()))
                {
                    GUI.FocusControl(null);
                    m_ActionMode = ActionMode.DragingNode;
                }

                view.Transform.UpdateLastPosition();
            }
        }
    }

    private void SelectNode()
    {
        if (m_ActionMode == ActionMode.DragingNode ||
            m_ActionMode == ActionMode.PortConnected)
        {
            return;
        }

        if (EditorInput.GetMouseButtonDown(0))
        {
            EditorInput.UpdateLastMousePosition();

            bool cursorOnNode = false;

            foreach (Node node in m_RegisteredNodes.Keys)
            {
                NodeEditorView view = m_RegisteredNodes[node];

                if (view.Rect.Contains(EditorInput.GetMousePosition()))
                    cursorOnNode = true;

                if (view.DragingArea.Contains(EditorInput.GetMousePosition()))
                {
                    DeselectAllNodes();
                    SelectNode(node);
                }
                else
                {
                    if (m_SelectionNode.Contains(node) == false)
                        continue;
                    
                    DeselectNode(node);
                }
            }

            if (cursorOnNode == false)
            {
                GUI.FocusControl(null);
                m_ActionMode = ActionMode.SelectionBox;
            }
        }
        else if (EditorInput.GetMouseButtonUp(0))
        {
            m_ActionMode = ActionMode.None;
        }
        else if (EditorInput.GetMouseDrag(0) && m_ActionMode == ActionMode.SelectionBox)
        {
            DeselectAllNodes();

            foreach (Node node in m_RegisteredNodes.Keys)
            {
                NodeEditorView view = m_RegisteredNodes[node];

                Rect nodeRect = new Rect(view.Transform.Position, view.Drawer.Size);

                if (m_SelectBox.Overlap(view.Rect))
                    SelectNode(node);
            }
        }
    }

    private void SelectNode(Node node)
    {
        NodeEditorView view = m_RegisteredNodes[node];

        m_SelectionNode.Add(node);
        view.Transform.UpdateLastPosition();
        view.Drawer.IsSelected = true;
        
        NodeEditorWindow.CustomRepaint();

        OnUpdateState?.Invoke();
    }

    private void DeselectNode(Node node)
    {
        if (m_SelectionNode.Contains(node) == false)
            throw new Exception($"This node is not selection!");

        m_RegisteredNodes[node].Drawer.IsSelected = false;
        m_SelectionNode.Remove(node);

        OnUpdateState?.Invoke();
    }

    private void SelectAllNode()
    {
        foreach (Node node in m_RegisteredNodes.Keys)
            SelectNode(node);

        NodeEditorWindow.CustomRepaint();
    }

    private void DeselectAllNodes()
    {
        foreach (Node node in m_SelectionNode)
            m_RegisteredNodes[node].Drawer.IsSelected = false;

        m_SelectionNode.Clear();
        NodeEditorWindow.CustomRepaint();
    }

    public void AddNode()
    {
        EditorInput.UpdateLastMousePosition();

        SearchWindowStingProvider provider = ScriptableObject.CreateInstance<SearchWindowStingProvider>();
        provider.Init(NodeRegisterToSearchWindow.GetPaths(typeof(NodeGraph), IsSubGraph), AddNode);

        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(EditorInput.GetMousePosition())), provider);
    }

    private void AddNode(string path)
    {
        Type nodeType = NodeRegisterToSearchWindow.GetType(typeof(NodeGraph), path);
        Node newNode = Activator.CreateInstance(nodeType) as Node;

        Vector2 newNodePosition = EditorInput.GetLastMousePosition() + m_ZoomCordsOrigin;

        AddNode(newNode, newNodePosition);
    }

    private void AddNode<T>(T node, Vector2 position) where T : Node
    {
        if (TryRegisterNode(node, position, node.GetType()) == false)
            return;

        NodeGraph.AddNode(node);
        IsChangedEditor = true;
    }

    private void DeleteNode()
    {
        if (m_ActionMode == ActionMode.SelectionBox)
            return;

        Queue<Node> deleteQueue = m_SelectionNode.ToQueue();
        DeselectAllNodes();

        while (deleteQueue.Count > 0)
        {
            Node node = deleteQueue.Dequeue();
            DeleteNode(node);
        }

        PortManager.LoopThroughPorts(FixPort);
    }

    private void DeleteNode(Node node)
    {
        if (node == null)
            throw new ArgumentNullException();

        m_RegisteredNodes.Remove(node);
        NodeGraph.DeleteNode(node);
        PortManager.RemovePortsByNode(node);

        if (node is SubGraphNode subGraphNode)
        {
            if(m_SubGraphNodeEditors.ContainsKey(subGraphNode))
                m_SubGraphNodeEditors.Remove(subGraphNode);
        }

        NodeEditorWindow.CustomRepaint();
    }

    private void FocusOnSelectedNode()
    {
        if(m_SelectionNode.Count == 0)
            return;

        Vector2 leftTop = Vector2.positiveInfinity;
        Vector2 rigtDown = Vector2.negativeInfinity;

        foreach (Node node in m_SelectionNode)
        {
            Rect rect = m_RegisteredNodes[node].Rect;

            if(rect.xMin < leftTop.x)
                leftTop.x = rect.xMin;

            if(rect.yMin < leftTop.y)
                leftTop.y = rect.yMin;

            if(rect.xMax > rigtDown.x)
                rigtDown.x = rect.xMax;

            if(rect.yMax > rigtDown.y)
                rigtDown.y = rect.yMax;
        }

        Vector2 centerPoint = leftTop + (rigtDown - leftTop) / 2;

        ZoomCordsOrigin = centerPoint + ZoomCordsOrigin - WindowRect.size / 2;
        NodeEditorWindow.CustomRepaint();
    }

    private void ReregisterNodes()
    {
        Dictionary<Node, NodeEditorView> oldRegisterNode = m_RegisteredNodes.Clone();
        
        m_RegisteredNodes.Clear();
        PortManager.ClearRegestryPorts();

        foreach (Node node in NodeGraph.Nodes)
        {
            Vector2 position = Vector2.zero;

            if (oldRegisterNode.ContainsKey(node))
                position = oldRegisterNode[node].Transform.OriginPosition;

            TryRegisterNode(node, position, node.GetType());
        }
    }

    private bool TryRegisterNode<T>(T node, Vector2 poistion) where T : Node
    {
        return TryRegisterNode(node, poistion, node.GetType());
    }

    private bool TryRegisterNode<T>(T node, Vector2 posistion, Type nodeType) where T : Node
    {
        Debug.Log(nodeType);

        Type nodeDrowerType = NodeGraphEditorRegister.GetNodeDrawer(nodeType);

        if (nodeDrowerType == null)
            nodeDrowerType = typeof(NodeDrawer);

        NodeDrawer nodeDrower = Activator.CreateInstance(nodeDrowerType) as NodeDrawer;

        if (nodeDrower == null)
        {
            Debug.LogWarning("!");
            nodeDrower = Activator.CreateInstance(typeof(NodeDrawer)) as NodeDrawer;
        }

        node.Init(NodeGraph);

        NodePropertyCasher.Cash(nodeType);
        ActiveConnectionManager.RegisterConnectors(node);
        PortManager.RegisterPorts(node);

        if (typeof(SubGraphNode).IsAssignableFrom(nodeType))
        {
            SubGraphNode subGraphNode = node as SubGraphNode;

            if (m_SubGraphNodeEditors.ContainsKey(subGraphNode) == false)
            {
                NodeGraphEditor graphEditor = new NodeGraphEditor();

                graphEditor.m_IsSubGraph = true;
                m_SubGraphNodeEditors.Add(subGraphNode, graphEditor);
            }

            m_SubGraphNodeEditors[subGraphNode].Init(subGraphNode.OwnGraph);
        }

        nodeDrower.Init(node);

        NodeEditorView view = new NodeEditorView(nodeDrower, new NodeTransform(posistion));
        m_RegisteredNodes.Add(node, view);

        return true;
    }

    public void DrawConnection(Connector output, Connector input)
    {
        Port outputPort = PortManager.GetPort(output);
        Port inputPort = PortManager.GetPort(input);

        if (outputPort == null || inputPort == null)
            return;

        Color color = Color.white;
        color = new Color(0.805f, 0.739f, 0.852f); //Mirfi style

        DrawConnection(outputPort.Rect, inputPort.Rect, Direction.HorizontalRight, Color.white);
    }

    public NodeGraphEditor GetSubNodeGraphEditor(SubGraphNode subGraphNode)
    {
        if(m_SubGraphNodeEditors.ContainsKey(subGraphNode) == false)
            return null;

        return m_SubGraphNodeEditors[subGraphNode];
    }

    public SubGraphNode GetSubGraphNode(NodeGraphEditor nodeGraphEditor)
    {
        foreach(SubGraphNode subGraphNode in m_SubGraphNodeEditors.Keys)
        {
            if(m_SubGraphNodeEditors[subGraphNode] == nodeGraphEditor)
                return subGraphNode;
        }

        return null;
    }

    private void DrawConnection(Rect start, Rect end, Direction orentation, Color color)
    {
        Vector3[] bezier = CalculeteConnectionBezier(start, end, orentation);

        Vector3 startPos = bezier[0];
        Vector3 startPos1 = bezier[1];

        Vector3 endPos = bezier[2];
        Vector3 endPos1 = bezier[3];

        Vector3 startTan = bezier[4];
        Vector3 endTan = bezier[5];

        Color shadowCol = new Color(0, 0, 0, 0.06f);

        //Handles.DrawBezier(startPos, startPos1, startPos, startPos1, color, null, 4);
        //Handles.DrawBezier(endPos, endPos1, endPos, endPos1, color, null, 4);

        //Handles.DrawBezier(startPos1, endPos1, startTan, endTan, color, null, 4);

        Bezier b = BezierCasher.GetBezier("NodeGraph", startPos1, endPos1, startTan, endTan, 16);
        Line.DrawConnection(b, startPos, endPos, 5, color);
    }

    private Vector3[] CalculeteConnectionBezier(Rect start, Rect end, Direction orentation)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
        Vector3 startPos1 = new Vector3(start.x + start.width / 2, start.y + start.height + 18, 0);

        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
        Vector3 endPos1 = new Vector3(end.x + end.width / 2, end.y - 18f, 0);

        Vector3 startTan = Vector3.zero;
        Vector3 endTan = Vector3.zero;

        float offset = 15;

        switch (orentation)
        {
            case Direction.VerticalUp:
                startPos1 = new Vector3(start.x + start.width / 2, start.y - offset, 0);
                endPos1 = new Vector3(end.x + end.width / 2, end.y + offset, 0);

                startTan = startPos1 + Vector3.up * 20;
                endTan = endPos1 + Vector3.down * 20;
                break;
            case Direction.VerticalDown:
                startPos1 = new Vector3(start.x + start.width / 2, start.y + start.height + offset, 0);
                endPos1 = new Vector3(end.x + end.width / 2, end.y - offset, 0);

                startTan = startPos1 + Vector3.up * 20;
                endTan = endPos1 + Vector3.down * 20;
                break;
            case Direction.HorizontalLeft:
                startPos1 = new Vector3(start.x + start.width / 2 - offset, start.y + start.height / 2, 0);
                endPos1 = new Vector3(end.x + end.width / 2 + offset, end.y + end.height / 2, 0);

                startTan = startPos1 + Vector3.left * 20;
                endTan = endPos1 + Vector3.right * 20;
                break;
            case Direction.HorizontalRight:
                startPos1 = new Vector3(start.x + start.width / 2 + offset, start.y + start.height / 2, 0);
                endPos1 = new Vector3(end.x + end.width / 2 - offset, end.y + end.height / 2, 0);

                startTan = startPos1 + Vector3.right * 20;
                endTan = endPos1 + Vector3.left * 20;
                break;
        }

        Vector3[] result =
        {
            startPos,
            startPos1,
            endPos,
            endPos1,
            startTan,
            endTan
        };

        return result;
    }

    private void FixConnect(Connector output, Connector input)
    {
        Debug.Log(1);
        //m_RegisteredNodes[output.Node].Drawer.GetPortDrawer(output.FieldName).IsConnected = true;
        //m_RegisteredNodes[input.Node].Drawer.GetPortDrawer(input.FieldName).IsConnected = true;

        UpdatePortView(output, input);
    }

    private void FixPort(Port port)
    {
        Debug.Log(2);
        //Connector connector = NodeGraph.ConnectionManager.GetConnector(port.Node, port.FieldName);
        Connector connector = port.Connector;
        bool isConnect = NodeGraph.ConnectionManager.GetConnections(connector).Length >= 1;
        m_RegisteredNodes[port.Node].Drawer.GetPortDrawer(port.FieldName).IsConnected = isConnect;
    }

    private void UpdatePortView(Connector output, Connector input)
    {
        UpdatePortView(output);
        UpdatePortView(input);
    }

    private void UpdatePortView(Connector connector)
    {
        PortDrawer drawer = null;

        if (connector.IsSubGraph)
        {
            Node node = ActiveConnectionManager.GetNode(connector);
            drawer = m_RegisteredNodes[node].Drawer.GetPortDrawer(connector.FieldName);
        }
        else
        {
            drawer = m_RegisteredNodes[connector.Node].Drawer.GetPortDrawer(connector.FieldName);
        }

        drawer.IsConnected = NodeGraph.ConnectionManager.GetConnections(connector).Length >= 1;
    }

    private void ValidMouseInputState()
    {
        if (EditorInput.EventType == EventType.Ignore)
            m_ActionMode = ActionMode.None;
    }
}

public static class EditorInput
{
    public static Event Current { get; private set; }
    public static Event Last { get; private set; }

    private static Vector2 sm_LastMousePosition;

    private static Dictionary<KeyCode, bool> sm_IsKeyPressed = new Dictionary<KeyCode, bool>();
    public static EventType EventType => Current.type;

    public static void UpdateInput()
    {
        Last = Current;
        Current = Event.current;

        if(Current.type == EventType.KeyDown || Current.type == EventType.KeyUp 
            && Current.isKey)
        {
            KeyCode keyCode = Current.keyCode;

            if (GetButtonDown(keyCode))
            {
                if (sm_IsKeyPressed.ContainsKey(keyCode) == false)
                    sm_IsKeyPressed.Add(keyCode, false);

                sm_IsKeyPressed[keyCode] = true;
            }
            else if (GetButtonUp(keyCode) 
                && sm_IsKeyPressed.ContainsKey(keyCode))
            {
                sm_IsKeyPressed[keyCode] = false;
            }
        }
    }

    public static Vector2 GetMousePosition()
    {
        return Current.mousePosition;
    }

    public static float GetMouseScrollWheel()
    {
        if(Current.type != EventType.ScrollWheel)
            return 0f;

        return Current.delta.y;
    }
    
    public static Vector2 GetLastMousePosition()
    {
        return sm_LastMousePosition;
    }

    public static void UpdateLastMousePosition()
    {
        sm_LastMousePosition = GetMousePosition();
    }

    public static bool GetMouseDrag(int button)
    {
        return Current.isMouse && Current.button == button 
            && Current.type == EventType.MouseDrag;
    }

    public static bool GetMouseButtonPress(int button)
    {
        if (button == 0)
            return GetMouseLeftButtonPress();
        else if (button == 1)
            return GetMouseRightButtonPress();

        return false;
    }

    public static bool GetMouseButtonDown(int button)
    {
        return Current.isMouse && Current.button == button 
            && Current.type == EventType.MouseDown;
    }

    public static bool GetMouseButtonUp(int button)
    {
        return Current.isMouse && Current.button == button
            && Current.type == EventType.MouseUp;
    }

    public static bool GetButtonPress(KeyCode keyCode)
    {
        if (sm_IsKeyPressed.ContainsKey(keyCode) == false)
            return false;

        if(sm_IsKeyPressed[keyCode] == false)
        {
            sm_IsKeyPressed.Remove(keyCode);
            return false;
        }

        return sm_IsKeyPressed[keyCode];
    }

    public static bool GetButtonDown(KeyCode keyCode)
    {
        return Current.isKey && Current.keyCode == keyCode 
            && Current.type == EventType.KeyDown;
    }

    public static bool GetButtonUp(KeyCode keyCode)
    {
        return Current.isKey && Current.keyCode == keyCode
            && Current.type == EventType.KeyUp;
    }

    private static bool GetMouseLeftButtonPress()
    {
        if (sm_IsKeyPressed.ContainsKey(KeyCode.Mouse0) == false)
            sm_IsKeyPressed.Add(KeyCode.Mouse0, false);

        if (GetMouseButtonDown(0) && sm_IsKeyPressed[KeyCode.Mouse0] == false)
            sm_IsKeyPressed[KeyCode.Mouse0] = true;
        else if (GetMouseButtonUp(0))
            sm_IsKeyPressed[KeyCode.Mouse0] = false;

        //Debug.LogWarning(sm_IsKeyPressed[KeyCode.Mouse0]);
        return sm_IsKeyPressed[KeyCode.Mouse0];
    }

    private static bool GetMouseRightButtonPress()
    {
        if (sm_IsKeyPressed.ContainsKey(KeyCode.Mouse1) == false)
            sm_IsKeyPressed.Add(KeyCode.Mouse1, false);

        if (GetMouseButtonDown(1))
        {
            sm_IsKeyPressed[KeyCode.Mouse1] = true;
        }
        else if (GetMouseButtonUp(1))
        {
            sm_IsKeyPressed.Remove(KeyCode.Mouse1);
            return false;
        }

        return sm_IsKeyPressed[KeyCode.Mouse1];
    }

}

public static class GraphDataManager
{
    public static T LoadGraph<T>(NodeGraphContanerBase contaner) where T : NodeGraph
    {
        object[] objects = ENIXFile.Deserialize(contaner);
        Type[] types = { typeof(NodeGraph) };

        Dictionary<Type, List<object>> filterObjects = ENIXFile.FilterObjectsByType(objects, types);

        if(filterObjects.ContainsKey(typeof(NodeGraph)))
        {
            if (filterObjects[typeof(NodeGraph)][0] is T nodeGraph)
            {
                nodeGraph.Init();
                return nodeGraph;
            }

            Debug.LogError("");
            return null;
        }

        Debug.LogError("");
        return null;
    }

#if UNITY_EDITOR
    public static void SaveGraph(NodeGraph graph, NodeGraphEditor graphEditor, NodeGraphContanerBase nodeGraphContaner)
    {
        object[] objects = { graph, graphEditor };

        List<string> serializedObject = ENIXFile.Serialize(objects);

        List<string> graphSerializedObjects = new List<string>();
        List<string> graphEditorSerializedObjects = new List<string>();

        foreach(string obj in serializedObject)
        {
            Type type = ENIXFile.GetSerializedObjectType(obj);

            if (typeof(NodeGraph).IsAssignableFrom(type) || typeof(Node).IsAssignableFrom(type)
                || typeof(ConnectionManager).IsAssignableFrom(type) || typeof(Connector).IsAssignableFrom(type))
            {
                graphSerializedObjects.Add(obj);
            }
            else
            {
                graphEditorSerializedObjects.Add(obj);
            }
        }

        NodeGraphContanerEditor info = GetEditorGraph(nodeGraphContaner);

        nodeGraphContaner.AddObjects(graphSerializedObjects);
        info.AddObjects(graphEditorSerializedObjects);

        EditorUtility.SetDirty(info);
        EditorUtility.SetDirty(nodeGraphContaner);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static NodeGraphContanerEditor GetEditorGraph(NodeGraphContanerBase contaner)
    {
        NodeGraphContanerEditor info;

        if (TryGetInfoFilePath(contaner, out string infoPath))
        {
            info = AssetDatabase.LoadAssetAtPath
                (infoPath, typeof(NodeGraphContanerEditor))
                as NodeGraphContanerEditor;
        }
        else
        {
            info = CreatInfoFile(contaner);
        }

        string infoFilePath = AssetDatabase.GetAssetPath(info);
        AssetDatabase.RenameAsset(infoFilePath, $"{contaner.GetInstanceID()}Info");
        info.TrySetNodeGraph(contaner);

        return info;
    }

    public static void LoadGraph(NodeGraphContanerBase contaner, out NodeGraph graph, out NodeGraphEditor graphEditor)
    {
        if(TryLoadGraph(contaner, out NodeGraph tempGraph, out NodeGraphEditor tempGraphEditor) == false)
        {
            if(tempGraph == null)
            {
                Type graphType = NodeGraphContanerRegister.GetNodeGraphType(contaner.GetType());
                tempGraph = Activator.CreateInstance(graphType) as NodeGraph;
            }

            if(tempGraphEditor == null)
                tempGraphEditor = new NodeGraphEditor();
        }

        graph = tempGraph;
        graphEditor = tempGraphEditor;

        tempGraph.Init();
    }

    public static bool TryLoadGraph(NodeGraphContanerBase contaner, out NodeGraph graph, out NodeGraphEditor graphEditor)
    {
        NodeGraphContanerEditor info = GetEditorGraph(contaner);

        List<string> nodeGraphObjects = contaner.SerializedObject.ToList();
        List<string> nodeGraphEditorObjects = info.SerializedObject.ToList();

        string[] serializedObject = nodeGraphObjects.Combine(nodeGraphEditorObjects).ToArray();

        object[] objects = ENIXFile.DeserializeObjects(serializedObject);

        NodeGraphEditor tempNodeGraphEditor = null;
        NodeGraph tempNodeGraph = null;

        Type[] filterType = { typeof(NodeGraphEditor), typeof(NodeGraph) };
        Dictionary<Type, List<object>> filterObjects = ENIXFile.FilterObjectsByType(objects, filterType);

        if (filterObjects.ContainsKey(typeof(NodeGraphEditor)))
        {
            foreach (object nodeGraphEditor in filterObjects[typeof(NodeGraphEditor)])
            {
                tempNodeGraphEditor = nodeGraphEditor as NodeGraphEditor;

                if (tempNodeGraphEditor.IsSubGraph == false)
                    break;
            }
        }

        if (filterObjects.ContainsKey(typeof(NodeGraph)))
        {
            foreach (object nodeGraph in filterObjects[typeof(NodeGraph)])
            {
                tempNodeGraph = nodeGraph as NodeGraph;

                if (tempNodeGraph.IsSubGraph == false)
                    break;
            }
        }

        graphEditor = tempNodeGraphEditor;
        graph = tempNodeGraph;

        if (tempNodeGraphEditor == null || tempNodeGraph == null)
            return false;

        return true;
    }

    public static bool TryGetInfoFilePath(NodeGraphContanerBase nodeGraphContaner, out string outPath)
    {
        if (Directory.Exists(EnigmaticData.GetFullPath(EnigmaticData.nodeGraphEditorInfo)) == false)
            Directory.CreateDirectory(EnigmaticData.GetFullPath(EnigmaticData.nodeGraphEditorInfo));

        List<NodeGraphContanerEditor> graphInfos = 
            EnigmaticData.LoadAllAssetsAtPathWithExtantion
            (EnigmaticData.nodeGraphEditorInfo, "*.asset", 
            typeof(NodeGraphContanerEditor))
            .Cast<NodeGraphContanerEditor>()
            .ToList();

        foreach (NodeGraphContanerEditor info in graphInfos)
        {
            if(info.NodeGraph == nodeGraphContaner)
            {
                outPath = AssetDatabase.GetAssetPath(info);
                return true;
            }
        }

        outPath = string.Empty;
        return false;
    }

    public static NodeGraphContanerEditor CreatInfoFile(NodeGraphContanerBase nodeGraphContaner)
    {
        Debug.Log($"Created info file for {nodeGraphContaner.name} graph!");
        NodeGraphContanerEditor info = ScriptableObject.CreateInstance<NodeGraphContanerEditor>();
        string pathWithFile = $"{EnigmaticData.GetUnityPath(EnigmaticData.nodeGraphEditorInfo)}" +
            $"/Info.asset";
        AssetDatabase.CreateAsset(info, pathWithFile);

        return info;
    }
#endif
}

namespace Enigmatic.Core
{
    public struct Bezier
    {
        private Vector3[] m_Points;

        public Vector3 StartPosition { get; private set; }
        public Vector3 EndPosition { get; private set; }
        public Vector3 StartTangent { get; private set; }
        public Vector3 EndTangent { get; private set; }

        public bool IsUsed { get; private set; }

        public int Count => m_Points.Length;
        public Vector3[] Points => m_Points.ToArray();

        public Bezier(Vector3 startPosition, Vector3 endPosition, 
            Vector3 startTangent, Vector3 endTangent, uint segmentCount)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartTangent = startTangent;
            EndTangent = endTangent;
            IsUsed = false;

            m_Points = new Vector3[segmentCount];
            CalculateBezierPoints(segmentCount);
        }

        public Vector3 GetPoint(uint index)
        {
            return m_Points[index];
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
        }

        public void ResetUsageFlag()
        {
            IsUsed = false;
        }

        private void CalculateBezierPoints(uint segmentCount)
        {
            for (int i = 0; i < segmentCount; i++)
            {
                float t = i / (segmentCount - 1f);
                m_Points[i] = CalculateBezierPoint(StartPosition, EndPosition, StartTangent, EndTangent, t);
            }
        }

        public static Vector3 CalculateBezierPoint(Vector3 startPosition, Vector3 endPosition,
            Vector3 startTangent, Vector3 endTangent, float t)
        {
            Vector3 p1 = Vector3.Lerp(startPosition, startTangent, t);
            Vector3 p2 = Vector3.Lerp(startTangent, endTangent, t);
            Vector3 p3 = Vector3.Lerp(endTangent, endPosition, t);

            Vector3 p4 = Vector3.Lerp(p1, p2, t);
            Vector3 p5 = Vector3.Lerp(p4, p3, t);

            Vector3 result = Vector3.Lerp(p4, p5, t);

            return result;
        }
    }

    public static class BezierCasher
    {
        private static Dictionary<string, List<Bezier>> m_Beziers = new Dictionary<string, List<Bezier>>();

        public static Bezier GetBezier(string tag, Vector3 startPosition, Vector3 endPosition,
            Vector3 startTangent, Vector3 endTangent, uint segmentCount)
        {
            if (m_Beziers.ContainsKey(tag) == false)
                m_Beziers.Add(tag, new List<Bezier>());

            foreach(Bezier b in m_Beziers[tag])
            {
                if(b.StartPosition == startPosition && b.EndPosition == endPosition
                    && b.StartTangent == startTangent && b.EndTangent == endTangent 
                    && b.Count == segmentCount)
                {
                    b.MarkAsUsed();
                    return b;
                }
            }

            Bezier bezier = new Bezier(startPosition, endPosition, startTangent, endTangent, segmentCount);
            bezier.MarkAsUsed();
            m_Beziers[tag].Add(bezier);

            return bezier;
        }

        public static void RemoveUnused(string tag)
        {
            if (m_Beziers.ContainsKey(tag) == false)
                return;

            m_Beziers[tag].RemoveAll(x => x.IsUsed == false);
            
            foreach(Bezier bezier in m_Beziers[tag])
                bezier.ResetUsageFlag();
        }
    }

    public static class Line
    {
        public static void DrawLine(Vector3 startPosition, Vector3 endPosition, float thickness, Color color)
        {
            Color originColor = Handles.color;
            Handles.color = color;

            Handles.DrawAAPolyLine(thickness, startPosition, endPosition);

            Handles.color = originColor;
        }

        public static void DrawBezer(Bezier bezier, float thickness, Color color)
        {
            Color originColor = Handles.color;
            Handles.color = color;

            Handles.DrawAAPolyLine(thickness, bezier.Points);

            Handles.color = originColor;
        }

        public static void DrawConnection(Bezier bezier, Vector3 startPosition, Vector3 endPosition, float thickness, Color color)
        {
            Color originColor = Handles.color;
            Handles.color = color;

            Vector3[] bezierPoints = bezier.Points;
            Vector3[] points = new Vector3[bezierPoints.Length + 2];

            points[0] = startPosition;

            for (int i = 0; i < bezierPoints.Length; i++)
                points[i + 1] = bezierPoints[i];

            points[points.Length - 1] = endPosition;

            Handles.DrawAAPolyLine(thickness, points);

            Handles.color = originColor;
        }
    }
}

public static class EnigmaticHandlesUtility
{
    public static bool PointIsNearBezier(Vector3 startPosition, Vector3 endPosition,
        Vector3 startTangent, Vector3 endTangent, Vector3 point, float minDistance, int segments = 200)
    {
        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)segments;

            Vector3 pointOnCurve = CalculateBezierPoint(startPosition, endPosition, startTangent, endTangent, t);

            float distance = Vector3.Distance(point, pointOnCurve);
            
            if (distance <= minDistance)
                return true;
        }

        return false;
    }

    public static Vector3 CalculateBezierPoint(Vector3 startPosition, Vector3 endPosition, 
        Vector3 startTangent, Vector3 endTangent, float t)
    {
        Vector3 p1 = Vector3.Lerp(startPosition, startTangent, t);
        Vector3 p2 = Vector3.Lerp(startTangent, endTangent, t);
        Vector3 p3 = Vector3.Lerp(endTangent, endPosition, t);

        Vector3 p4 = Vector3.Lerp(p1, p2, t);
        Vector3 p5 = Vector3.Lerp(p4, p3, t);

        Vector3 p = Vector3.Lerp(p4, p5, t);

        return p;
    }
}