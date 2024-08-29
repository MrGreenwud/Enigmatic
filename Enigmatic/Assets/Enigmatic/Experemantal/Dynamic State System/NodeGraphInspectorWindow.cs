using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Enigmatic.Core;

public class NodeGraphInspectorWindow : EditorWindow
{
    public static NodeGraphInspectorWindow ActiveWindow;

    public NodeGraphEditor NodeGraphEditor { get; private set; }

    private Vector2 m_ScrollPosition;
    private Dictionary<Node, ExpandedElement> m_IsExpandedNodeParametors = new Dictionary<Node, ExpandedElement>();

    public static void OpenWindow()
    {
        NodeGraphInspectorWindow window = GetWindow<NodeGraphInspectorWindow>();
        window.titleContent = new GUIContent("Graph Inspector", EnigmaticStyles.nodeEditorInspectorWindowIcon.normal.background);

        ActiveWindow = window;

        NodeGraphEditor.OnOpenGraph += window.OnSwichGraph;
        window.OnSwichGraph();
    }

    public void OnSwichGraph()
    {
        if(NodeGraphEditor.Last != null)
            NodeGraphEditor.Last.OnUpdateState -= OnUpdateGraphInspector;

        if (NodeGraphEditor.Active != null)
            NodeGraphEditor.Active.OnUpdateState += OnUpdateGraphInspector;

        m_IsExpandedNodeParametors.Clear();
        OnUpdateGraphInspector();
    }

    private void OnDisable()
    {
        if (NodeGraphEditor.Last != null)
            NodeGraphEditor.Last.OnUpdateState -= OnUpdateGraphInspector;

        if (NodeGraphEditor.Active != null)
            NodeGraphEditor.Active.OnUpdateState -= OnUpdateGraphInspector;
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        NodeGraphEditor = NodeGraphEditor.Active;

        if (NodeGraphEditor == null)
            return;

        EditorInput.UpdateInput();

        EnigmaticGUILayout.BeginVerticalScrollView(new Rect(Vector2.zero, position.size),
            new Rect(Vector2.zero, position.size), m_ScrollPosition, EnigmaticGUILayout.Padding(0), EnigmaticGUILayout.ElementSpacing(-1));
        {
            if (NodeGraphEditor.IsSubGraph)
                DrawSubgraphParametersArea();

            EnigmaticGUILayout.BeginHorizontal("box", 0, EnigmaticGUILayout.ExpandHeight(false),
                EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.Height(22),
                EnigmaticGUILayout.Padding(-1));
            {
                EnigmaticGUILayout.Space(1);

                EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.ExpandHeight(true),
                    EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Padding(3));
                {
                    if (EnigmaticGUILayout.Button("", new Vector2(18, 18), EnigmaticStyles.addFoldoutButton))
                        NodeGraphEditor.AddNode();
                }
                EnigmaticGUILayout.EndVertical();

                EnigmaticGUILayout.Space(-15);

                EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.ExpandHeight(true),
                    EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Padding(2));
                {
                    EnigmaticGUILayout.Lable("Nodes", EnigmaticStyles.nodeTitleStyle);
                }
                EnigmaticGUILayout.EndVertical();
            }
            EnigmaticGUILayout.EndHorizontal();

            NodeGraphEditor.LoopThroughSelectionNods(DrawNodeParameters);
        }
        m_ScrollPosition = EnigmaticGUILayout.EndScrollView(Repaint);

        if (EditorGUI.EndChangeCheck())
            NodeEditorWindow.ActiveWindow.Repaint();

        //Repaint();
    }

    public void DrawNodeParameters(Node node)
    {
        if (node is InputNode || node is OutputNode || node is ParameterNode)
            return;

        if (NodeGraphEditor.TryGetNodeView(node, out NodeEditorView view) == false)
            return;

        NodeDrawer drawer = view.Drawer;

        if (m_IsExpandedNodeParametors.ContainsKey(node) == false)
            return;

        ExpandedElement nodeElement = m_IsExpandedNodeParametors[node];

        EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.ExpandHeight(true),
            EnigmaticGUILayout.ExpandWidth(false), EnigmaticGUILayout.ElementSpacing(0), EnigmaticGUILayout.Padding(0));
        {
            EnigmaticGUILayout.BeginHorizontal(EnigmaticStyles.foldoutBackground, 7, EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.ExpandHeight(true),
                EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Clickable(true), EnigmaticGUILayout.Padding(1));
            {
                if (nodeElement.IsExpanded)
                    EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonOpen);
                else
                    EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonClose);

                EnigmaticGUILayout.Lable(drawer.NodeTitle);
            }
            if (EnigmaticGUILayout.EndHorizontal())
                nodeElement.IsExpanded = !nodeElement.IsExpanded;

            if (nodeElement.IsExpanded)
            {
                EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.ExpandHeight(true),
                            EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Padding(0));
                {
                    if (drawer.InputPortCount > 0)
                    {
                        ExpandedElement inputElement = nodeElement.GetElement("input");

                        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                            EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.Clickable(true), EnigmaticGUILayout.Padding(0));
                        {
                            EnigmaticGUILayout.Space(18);

                            if (inputElement.IsExpanded)
                                EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonOpen);
                            else
                                EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonClose);

                            EnigmaticGUILayout.Lable("Input Ports");
                        }
                        if (EnigmaticGUILayout.EndHorizontal())
                            inputElement.IsExpanded = !inputElement.IsExpanded;

                        if (inputElement.IsExpanded)
                        {
                            EnigmaticGUILayout.BeginVertical("box", 0, EnigmaticGUILayout.ExpandHeight(true),
                                EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Padding(3));
                            {
                                drawer.LoopThroughInputPortDawers(DrawPort);
                            }
                            EnigmaticGUILayout.EndVertical();
                        }
                    }

                    if (drawer.ParametersCount > 0)
                    {
                        ExpandedElement parametersElement = nodeElement.GetElement("parameters");

                        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                            EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.Clickable(true), EnigmaticGUILayout.Padding(0));
                        {
                            EnigmaticGUILayout.Space(18);

                            if (parametersElement.IsExpanded)
                                EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonOpen);
                            else
                                EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonClose);

                            EnigmaticGUILayout.Lable("Parameters");
                        }
                        if (EnigmaticGUILayout.EndHorizontal())
                            parametersElement.IsExpanded = !parametersElement.IsExpanded;

                        if (parametersElement.IsExpanded)
                        {
                            EnigmaticGUILayout.BeginVertical("box", 0, EnigmaticGUILayout.ExpandHeight(true),
                               EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Padding(3));
                            {
                                drawer.LoopThroughInputParameters(DrawParameters);
                            }
                            EnigmaticGUILayout.EndHorizontal();
                        }
                    }
                }
                EnigmaticGUILayout.EndVertical();
            }
        }
        EnigmaticGUILayout.EndVertical();
    }

    public void DrawSubgraphParametersArea()
    {
        SubGraphNode subNode = NodeGraphEditor.Last.GetSubGraphNode(NodeGraphEditor);
        NodeGraph nodeGraph = subNode.OwnGraph;

        if (m_IsExpandedNodeParametors.ContainsKey(subNode) == false)
            return;

        nodeGraph.TryGetAllNodeByType<InputNode>(out Node[] inputNodes);
        nodeGraph.TryGetAllNodeByType<OutputNode>(out Node[] ouputNodes);
        nodeGraph.TryGetAllNodeByType<ParameterNode>(out Node[] parameterNodes);

        List<Node> errorNameNode = new List<Node>();

        foreach(Node inputNode in inputNodes)
        {
            bool isCoincidences = false;

            foreach(Node node in inputNodes)
            {
                if (node == inputNode || errorNameNode.Contains(node))
                    continue;

                if (node.Name == inputNode.Name)
                {
                    errorNameNode.Add(node);
                    isCoincidences = true;
                }
            }

            foreach(Node outputNode in ouputNodes)
            {
                if (outputNode.Name == inputNode.Name)
                {
                    errorNameNode.Add(outputNode);
                    isCoincidences = true;
                }

                foreach (Node parameterNode in parameterNodes)
                {
                    if (parameterNode.Name == inputNode.Name)
                    {
                        errorNameNode.Add(parameterNode);
                        isCoincidences = true;
                    }

                    if (parameterNode.Name == outputNode.Name)
                    {
                        if(errorNameNode.Contains(parameterNode) == false)
                            errorNameNode.Add(parameterNode);

                        if (errorNameNode.Contains(outputNode) == false)
                            errorNameNode.Add(outputNode);
                    }
                }

                if(isCoincidences)
                    errorNameNode.Add(inputNode);
            }
        }

        EnigmaticGUILayout.BeginVertical(EnigmaticStyles.foldoutBackground, 7, EnigmaticGUILayout.ExpandHeight(true),
            EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Width(position.width),
            EnigmaticGUILayout.Padding(0), EnigmaticGUILayout.ElementSpacing(-1));
        {
            EnigmaticGUILayout.BeginVertical(EnigmaticStyles.foldoutBackground, 7, EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.ExpandHeight(true),
                    EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Clickable(true), EnigmaticGUILayout.Padding(1));
            {
                EnigmaticGUILayout.Space(8);

                EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.ExpandHeight(true),
                    EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Clickable(true), EnigmaticGUILayout.Padding(1));
                {
                    EnigmaticGUILayout.Space(9);

                    EnigmaticGUILayout.Port(Vector2.one * 30, EnigmaticStyles.subgraphNodeIcon);

                    EnigmaticGUILayout.Space(9);

                    SubGraphNode subGraphNode = NodeGraphEditor.Last.GetSubGraphNode(NodeGraphEditor.Active);
                    subGraphNode.Name = EnigmaticGUILayout.TextField("", subGraphNode.Name, position.width - 70);
                }
                EnigmaticGUILayout.EndHorizontal();

                EnigmaticGUILayout.Space(1);
            }
            EnigmaticGUILayout.EndVertical();

            ExpandedElement nodeElement = m_IsExpandedNodeParametors[subNode];
            ExpandedElement inputElement = nodeElement.GetElement("input");

            EnigmaticGUILayout.BeginHorizontal(EnigmaticStyles.foldoutBackground, 7, EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.Clickable(true), EnigmaticGUILayout.Padding(0));
            {
                if (inputElement.IsExpanded)
                    EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonOpen);
                else
                    EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonClose);

                EnigmaticGUILayout.Space(-6);

                EnigmaticGUILayout.Port(new Vector2(16, 18), EnigmaticStyles.inputData);

                EnigmaticGUILayout.Lable("Input Ports", EnigmaticStyles.nodeTitleStyle);
            }
            if (EnigmaticGUILayout.EndHorizontal())
                inputElement.IsExpanded = !inputElement.IsExpanded;

            if (inputElement.IsExpanded)
            {
                EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                    EnigmaticGUILayout.ElementSpacing(2));
                {
                    foreach (InputNode node in inputNodes)
                    {
                        if (NodeGraphEditor.TryGetNodeView(node, out NodeEditorView view) == false)
                            continue;

                        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                            EnigmaticGUILayout.Padding(0));
                        {
                            EnigmaticGUILayout.Space(36);

                            if (errorNameNode.Contains(node))
                            {
                                EnigmaticGUILayout.Space(-22);
                                EnigmaticGUILayout.Port(new Vector2(18, 18), EnigmaticStyles.errorIcon);
                            }
                            
                            node.Name = EnigmaticGUILayout.TextField("", node.Name, position.width - 50);
                            view.Drawer.NodeTitle = node.Name;
                        }
                        EnigmaticGUILayout.EndHorizontal();
                    }
                }
                EnigmaticGUILayout.EndVertical();
            }

            ExpandedElement outputElement = nodeElement.GetElement("output");

            EnigmaticGUILayout.BeginHorizontal(EnigmaticStyles.foldoutBackground, 7, EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.Clickable(true), EnigmaticGUILayout.Padding(0));
            {
                if (outputElement.IsExpanded)
                    EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonOpen);
                else
                    EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonClose);

                EnigmaticGUILayout.Space(-6);

                EnigmaticGUILayout.Port(new Vector2(16, 18), EnigmaticStyles.outputData);

                EnigmaticGUILayout.Lable("Output Ports", EnigmaticStyles.nodeTitleStyle);
            }
            if (EnigmaticGUILayout.EndHorizontal())
                outputElement.IsExpanded = !outputElement.IsExpanded;

            if (outputElement.IsExpanded)
            {
                EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                    EnigmaticGUILayout.ElementSpacing(2));
                {
                    foreach (OutputNode node in ouputNodes)
                    {
                        if (NodeGraphEditor.TryGetNodeView(node, out NodeEditorView view) == false)
                            continue;

                        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                            EnigmaticGUILayout.Padding(0));
                        {
                            EnigmaticGUILayout.Space(36);

                            if (errorNameNode.Contains(node))
                            {
                                EnigmaticGUILayout.Space(-22);
                                EnigmaticGUILayout.Port(new Vector2(18, 18), EnigmaticStyles.errorIcon);
                            }

                            node.Name = EnigmaticGUILayout.TextField("", node.Name, position.width - 50);
                            view.Drawer.NodeTitle = node.Name;
                        }
                        EnigmaticGUILayout.EndHorizontal();
                    }
                }
                EnigmaticGUILayout.EndVertical();
            }

            ExpandedElement parametersElement = nodeElement.GetElement("parameters");

            EnigmaticGUILayout.BeginHorizontal(EnigmaticStyles.foldoutBackground, 7, EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                EnigmaticGUILayout.Width(position.width), EnigmaticGUILayout.Clickable(true), EnigmaticGUILayout.Padding(0));
            {

                if (parametersElement.IsExpanded)
                    EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonOpen);
                else
                    EnigmaticGUILayout.Port(Vector2.one * 20, EnigmaticStyles.foldoutButtonClose);

                EnigmaticGUILayout.Space(-6);

                EnigmaticGUILayout.Port(new Vector2(18, 20), EnigmaticStyles.parameter);

                EnigmaticGUILayout.Lable("Parameters", EnigmaticStyles.nodeTitleStyle);
            }
            if (EnigmaticGUILayout.EndHorizontal())
                parametersElement.IsExpanded = !parametersElement.IsExpanded;

            if(parametersElement.IsExpanded)
            {
                EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                    EnigmaticGUILayout.ElementSpacing(2));
                {
                    foreach (ParameterNode node in parameterNodes)
                    {
                        if (NodeGraphEditor.TryGetNodeView(node, out NodeEditorView view) == false)
                            continue;

                        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.ExpandWidth(true),
                            EnigmaticGUILayout.Padding(0));
                        {
                            EnigmaticGUILayout.Space(36);

                            if (errorNameNode.Contains(node))
                            {
                                EnigmaticGUILayout.Space(-22);
                                EnigmaticGUILayout.Port(new Vector2(18, 18), EnigmaticStyles.errorIcon);
                            }

                            node.Name = EnigmaticGUILayout.TextField("", node.Name, position.width - 50);
                            view.Drawer.NodeTitle = node.Name;
                        }
                        EnigmaticGUILayout.EndHorizontal();
                    }
                }
                EnigmaticGUILayout.EndVertical();
            }
        }
        EnigmaticGUILayout.EndHorizontal();
    }

    public void OnUpdateGraphInspector()
    {
        if (NodeGraphEditor == null)
        {
            m_IsExpandedNodeParametors.Clear();
            Repaint();
            return;
        }

        NodeGraphEditor.LoopThroughSelectionNods(AddNodeExpanded);

        if (NodeGraphEditor.IsSubGraph)
        {
            ExpandedElement nodeElement = new ExpandedElement("subNode");

            nodeElement.AddCiledElement("input");
            nodeElement.AddCiledElement("output");
            nodeElement.AddCiledElement("parameters");

            Node node = NodeGraphEditor.Last.GetSubGraphNode(NodeGraphEditor);

            if (node != null && m_IsExpandedNodeParametors.ContainsKey(node) == false)
            {
                if(m_IsExpandedNodeParametors.ContainsKey(node))
                    m_IsExpandedNodeParametors.Remove(node);

                m_IsExpandedNodeParametors.Add(node, nodeElement);
            }
        }

        Repaint();
    }

    public void AddNodeExpanded(Node node)
    {
        if (m_IsExpandedNodeParametors.ContainsKey(node) == true)
            return;

        ExpandedElement nodeElement = new ExpandedElement("node");

        nodeElement.AddCiledElement("input");
        nodeElement.AddCiledElement("output");
        nodeElement.AddCiledElement("parameters");

        m_IsExpandedNodeParametors.Add(node, nodeElement);
    }

    private void DrawPort(PortDrawer drawer)
    {
        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true),
            EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Width(position.width));
        {
            EnigmaticGUILayout.Space(36);
            drawer.DrawField();
        }
        EnigmaticGUILayout.EndHorizontal();
    }

    private void DrawParameters(ParameterDrawer drawer)
    {
        EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.ExpandHeight(true),
            EnigmaticGUILayout.ExpandWidth(true), EnigmaticGUILayout.Width(position.width));
        {
            EnigmaticGUILayout.Space(36);
            drawer.DrawField();
        }
        EnigmaticGUILayout.EndHorizontal();
    }

}

public class ExpandedElement
{
    public bool IsExpanded;

    private List<ExpandedElement> m_CildElement = new List<ExpandedElement>();

    public string Tag { get; private set; }

    public ExpandedElement(string tag)
    {
        Tag = tag;
    }

    public ExpandedElement AddCiledElement(string tag)
    {
        ExpandedElement element = new ExpandedElement(tag);
        m_CildElement.Add(element);

        return element;
    }

    public ExpandedElement GetElement(string tag)
    {
        foreach(ExpandedElement element in m_CildElement)
        {
            if(element.Tag == tag)
                return element;
        }

        return null;
    }

    public bool TryGetElement(string tag, out ExpandedElement element)
    {
        element = GetElement(tag);
        return element != null;
    }
}