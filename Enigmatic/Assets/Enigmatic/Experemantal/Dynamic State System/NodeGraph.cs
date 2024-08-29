using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Enigmatic.Experemental.ENIX;

[SerializebleObject]
public class NodeGraph
{
    [SerializebleProperty] private List<Node> m_Nodes = new List<Node>();
    [SerializebleProperty] private ConnectionManager m_ConnectionManager;
    [SerializebleProperty] public bool IsSubGraph;

    private Dictionary<Node, string> m_NodesTypeRegestry = new Dictionary<Node, string>(); //Delete

    //public NodePropertyCasher NodePropertyCasher { get; private set; }

    public ConnectionManager ConnectionManager => m_ConnectionManager;
    public Node[] Nodes => m_Nodes.ToArray();

    public NodeGraph()
    {
        //NodePropertyCasher = new NodePropertyCasher();

        m_ConnectionManager = new ConnectionManager();
        //m_ConnectionManager.Init(NodePropertyCasher);
    }

    public virtual void Init()
    {
        foreach (Node node in m_Nodes)
        {
            node.Init(this);
            NodePropertyCasher.Cash(node.GetType());
        }

        //m_ConnectionManager.Init(NodePropertyCasher);
    }

    public virtual void OnVerifay()
    {
        Queue<Node> destroyed = new Queue<Node>();

        foreach (Node node in m_Nodes)
        {
            Type type = Type.GetType(m_NodesTypeRegestry[node]);

            if (type == null || typeof(Node).IsAssignableFrom(type) == false)
            {
                Debug.LogWarning("");
                destroyed.Enqueue(node);
            }
        }

        while (destroyed.Count > 0)
            RemoveNode(destroyed.Dequeue());
    }

    public void AddNode<T>(T node) where T : Node
    {
        if (node == null)
            throw new ArgumentNullException();

        m_Nodes.Add(node);

        NodePropertyCasher.Cash(node.GetType());
        m_ConnectionManager.RegisterConnectors(node);

        node.Init(this);

        m_NodesTypeRegestry.Add(node, typeof(T).FullName); //Delete
    }

    public void RemoveNode(Node node)
    {
        m_Nodes.Remove(node);
        m_NodesTypeRegestry.Remove(node); //Delete
    } //Delete

    public Type GetInitialNodeType(Node node)
    {
        if (m_NodesTypeRegestry.ContainsKey(node) == false)
            return null;

        Type type = Type.GetType(m_NodesTypeRegestry[node]);

        if (type == null)
        {
            Debug.LogWarning("");
            RemoveNode(node);
        }

        return type;
    } //Delete

    public bool TryGetInitialNodeType(Node node, out Type type)
    {
        type = GetInitialNodeType(node);

        if (type == null)
            return false;

        return true;
    } //Delete

    public bool TryGetAllNodeByType<T>(out Node[] nodes) where T : Node
    {
        List<Node> result = new List<Node>();

        foreach(Node node in m_Nodes)
        {
            if(node is T tNode)
                result.Add(node);
        }

        nodes = result.ToArray();
        return result.Count > 0;
    }

    public void Clear()
    {
        m_Nodes.Clear();
        m_NodesTypeRegestry.Clear(); //Delete
    }

    public NodeGraph Clone()
    {
        return null;
    }

    public void DeleteNode(Node node)
    {
        if (node == null)
            throw new ArgumentNullException();

        if (m_Nodes.Contains(node) == false)
            throw new InvalidOperationException();

        m_Nodes.Remove(node);
        ConnectionManager.RemoveConnectorsByNode(node);
    }
}

[CreateAssetMenu(fileName = "TestNodeGraph", menuName = "Enigmatic/TestNodeGraph")]
[CustomNodeGraphContaner(typeof(NodeGraph))]
public class NodeGraphContanerBase : ENIXContaner { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CustomNodeGraphContaner : Attribute
{
    public Type ContanerType { get; private set; }

    public CustomNodeGraphContaner(Type contanerType)
    {
        ContanerType = contanerType;
    }
}

public static class NodeGraphContanerRegister
{
    private static Dictionary<Type, Type> sm_RegisteredNodeGraphContaner = new Dictionary<Type, Type>();

    [InitializeOnLoadMethod]
    private static void Register()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach(Type type in assembly.GetTypes())
        {
            if (typeof(NodeGraphContanerBase).IsAssignableFrom(type))
                ContanerRegister(type);
        }
    }

    private static void ContanerRegister(Type type)
    {
        CustomNodeGraphContaner contaner = (CustomNodeGraphContaner)Attribute.GetCustomAttribute(type, typeof(CustomNodeGraphContaner));

        if (contaner == null)
            return;

        sm_RegisteredNodeGraphContaner.Add(type, contaner.ContanerType);
    }

    public static Type GetNodeGraphType(Type contanerType)
    {
        return sm_RegisteredNodeGraphContaner[contanerType];
    }
}