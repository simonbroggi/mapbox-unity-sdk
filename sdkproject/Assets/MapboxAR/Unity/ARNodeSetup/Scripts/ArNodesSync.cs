namespace Mapbox.Unity.Ar
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Location;
	using Mapbox.Utils;

	//using System.Threading.Tasks;

	/// <summary>
	///  Generates and filters ArNodes for ARLocationManager.
	/// </summary>
	public class ArNodesSync : NodeSyncBase
	{

		[SerializeField]
		Transform _targetTransform;

		[SerializeField]
		float _minMagnitudeBetween;

		[SerializeField]
		int _capacityForNodes;

		float _latestBestGPSAccuracy;
		List<Node> _savedNodes;
		WaitForSeconds _waitFor;

		AbstractMap _map;

		CircularBuffer<Node> _nodeBuffer;

		public override void InitializeNodeBase(AbstractMap map)
		{
			_savedNodes = new List<Node>();
			CentralizedARLocator.OnNewHighestAccuracyGPS += SavedGPSAccuracy;
			_map = map;
			IsNodeBaseInitialized = true;
			Debug.Log("Initialized ARNodes");
			_nodeBuffer = new CircularBuffer<Node>(10);
			var node = _nodeBuffer[0];
		}

		void SavedGPSAccuracy(Location location)
		{
			_latestBestGPSAccuracy = location.Accuracy;
		}

		public override void SaveNode()
		{
			bool saveNode = false;

			if (_savedNodes.Count > 1)
			{
				var previousNodePos = _map.GeoToWorldPosition(_savedNodes[_savedNodes.Count - 1].LatLon, false);
				var currentMagnitude = _targetTransform.position - previousNodePos;

				if (currentMagnitude.magnitude >= _minMagnitudeBetween)
				{
					saveNode = true;
				}
			}
			else
			{
				saveNode = true;
			}
			if (saveNode == true)
			{
				Debug.Log("Saving AR Node");
				var node = new Node
				{
					LatLon = _map.WorldToGeoPosition(_targetTransform.position),
					Accuracy = _latestBestGPSAccuracy
				};
				_nodeBuffer.Add(node);
			}
		}

		public override Node ReturnNodeAtIndex(int index)
		{
			return _nodeBuffer[index];
		}

		public override int ReturnNodeCount()
		{
			return _nodeBuffer.Count;
		}

		public override Node ReturnLatestNode()
		{
			return _nodeBuffer[0];
		}
	}
}
