namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Location;
	using System;

	/// <summary>
	///  Generates and filters ArNodes for ARLocationManager.
	/// </summary>
	public class ArNodesSync : NodeSyncBase
	{

		[SerializeField]
		GameObject _debugNodes;

		[SerializeField]
		Transform _targetTransform;

		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		float _timeBetweenNodeDrop, _minMagnitudeBetween;

		IEnumerator _saveARnodes;
		List<Node> _savedNodes;
		WaitForSeconds _waitFor;
		int _latestBestGPSAccuracy;

		// TODO: 
		// Average best here... The median scale of accuracy.. 
		//For map matching... to determine which points to send...
		// The median should have a callback point like "give me the latest 5 nodes.."
		// And that should return the median of the accuracy..
		//
		//Only add the best points if a point drops below median exclude it from the list..

		void Start()
		{
			//TODO : This needs to have InitializdedARMode notifier.
			//That is notified on ARPlanePlacement....
			//_map.Initialize();

			_waitFor = new WaitForSeconds(_timeBetweenNodeDrop);
			_savedNodes = new List<Node>();
			CentralizedARLocator.OnNewHighestAccuracyGPS += SavedGPSAccuracy;

			Action handler = null;
			handler = () =>
			{
				StartCoroutine(SaveArNodes(_targetTransform));
				_map.OnInitialized -= handler;
			};
			_map.OnInitialized += handler;

		}

		void SavedGPSAccuracy(Location location)
		{
			_latestBestGPSAccuracy = location.Accuracy;
		}

		IEnumerator SaveArNodes(Transform dropTransform)
		{
			while (true)
			{
				Debug.Log("this runs");
				ConvertToNodes(dropTransform);
				yield return _waitFor;
			}
		}

		void ConvertToNodes(Transform nodeDrop)
		{
			//TODO; check magnitude
			//Check here the magnetude of the node for AR.
			if (_savedNodes.Count >= 1)
			{
				var previousNodePos = _map.GeoToWorldPosition(_savedNodes[_savedNodes.Count - 1].LatLon, false);
				Debug.Log(_savedNodes[_savedNodes.Count - 1].LatLon);
				Debug.Log(previousNodePos);
				var currentMagnitude = nodeDrop.position - previousNodePos;
				Debug.Log("Current Magnitude: " + currentMagnitude.magnitude);

				if (currentMagnitude.magnitude >= _minMagnitudeBetween)
				{
					var node = new Node();
					node.LatLon = _map.WorldToGeoPosition(nodeDrop.position);
					node.Accuracy = _latestBestGPSAccuracy;
					_savedNodes.Add(node);
					Instantiate(_debugNodes, nodeDrop.position, Quaternion.identity);
				}
			}
			else
			{
				var node = new Node();
				Debug.Log(_map.WorldToGeoPosition(nodeDrop.position));
				node.LatLon = _map.WorldToGeoPosition(nodeDrop.position);
				node.Accuracy = _latestBestGPSAccuracy;
				_savedNodes.Add(node);
				Instantiate(_debugNodes, nodeDrop.position, Quaternion.identity);
			}

		}

		public override Node[] ReturnNodes()
		{
			return _savedNodes.ToArray();
		}

		public override Node ReturnLatestNode()
		{
			return _savedNodes[_savedNodes.Count];
		}
	}
}

