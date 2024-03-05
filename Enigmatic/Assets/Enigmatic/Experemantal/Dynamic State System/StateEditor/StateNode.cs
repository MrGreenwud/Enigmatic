using System.Collections.Generic;
using UnityEngine;
using XNode;
using Enigmatic.DynamicStateSystem;

namespace Enigmatic.Experemental.DynamicStateSystem
{
    public class StateNode : Node
    {
        [Input(dynamicPortList = true)]
        [SerializeField] private List<State> states;

        [Output]
        [SerializeField] private State state;
    }
}