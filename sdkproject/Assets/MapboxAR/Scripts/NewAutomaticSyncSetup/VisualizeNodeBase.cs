namespace Mapbox.Utils
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Ar;
	using Mapbox.Unity.Map;

	public class VisualizeNodeBase : MonoBehaviour
	{
		[SerializeField]
		NodeSyncBase _nodeBase;

		[SerializeField]
		LineRenderer _lineRend;

		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		Material _nodeMat;

		int _count;

		private void Start()
		{
			_nodeBase.NodeAdded += PlotRoute;
		}

		void PlotRoute()
		{
			Debug.Log("called");

			var nodePos = _map.GeoToWorldPosition(_nodeBase.ReturnLatestNode().LatLon);
			Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), nodePos, Quaternion.identity, _map.gameObject.transform);

			// All this trouble for a line rend. Lol.
			var nodes = _nodeBase.ReturnNodes();
			_lineRend.positionCount = nodes.Length;

			for (int i = 0; i < _nodeBase.ReturnNodes().Length; i++)
			{
				_lineRend.SetPosition(i, _map.GeoToWorldPosition(nodes[i].LatLon));
			}
		}

		private void OnDisable()
		{
			_nodeBase.NodeAdded += PlotRoute;
		}
	}
}
