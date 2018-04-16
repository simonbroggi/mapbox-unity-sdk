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

		private void Start()
		{
			_nodeBase.NodeAdded += PlotRoute;
		}

		private void PlotRoute()
		{
			// TODO: pooling here for new Spheres... This won't work for MapMatching Nodes.

			var nodePos = _map.GeoToWorldPosition(_nodeBase.ReturnLatestNode().LatLon);
			Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), nodePos, Quaternion.identity, _map.gameObject.transform);

			var nodes = _nodeBase.ReturnNodes();
			_lineRend.positionCount = nodes.Length;

			for (int i = 0; i < _nodeBase.ReturnNodes().Length; i++)
			{
				_lineRend.SetPosition(i, _map.GeoToWorldPosition(nodes[i].LatLon));
			}
		}

		private void OnDisable()
		{
			_nodeBase.NodeAdded -= PlotRoute;
		}
	}
}
