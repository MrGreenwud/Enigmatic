using UnityEngine;
using Enigmatic.Experemental.ENIX;

public class NodeGraphContanerEditor : ENIXContaner
{
    [SerializeField] private NodeGraphContanerBase m_NodeGraph;
    public NodeGraphContanerBase NodeGraph => m_NodeGraph;

    public bool TrySetNodeGraph(NodeGraphContanerBase nodeGraph)
    {
        if (m_NodeGraph != null)
            return false;

        if (nodeGraph == null)
            return false;

        m_NodeGraph = nodeGraph;
        return true;
    }
}