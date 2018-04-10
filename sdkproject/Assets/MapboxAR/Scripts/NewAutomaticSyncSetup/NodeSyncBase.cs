namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class NodeSyncBase : MonoBehaviour
	{
		public abstract Node[] ReturnNodes();
		public abstract Node ReturnLatestNode();
	}

}
