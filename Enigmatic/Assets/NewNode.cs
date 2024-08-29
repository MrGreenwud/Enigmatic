using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class NewNode : XNode.Node {

	[Input] [SerializeField] private List<float> floats;
	[Input] [SerializeField] private float floatsss;
	[Output] [SerializeField] private float sdad;

	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}