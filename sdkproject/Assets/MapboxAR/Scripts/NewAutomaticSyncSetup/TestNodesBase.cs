using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Ar;

public class TestNodesBase : NodeSyncBase
{

	public override Node[] ReturnNodes()
	{
		var n = new Node[0];
		return n;
	}
	public override Node ReturnLatestNode()
	{
		var n = new Node();
		return n;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (base.NodeAdded != null)
			{
				base.NodeAdded();
			}
		}
	}
}
