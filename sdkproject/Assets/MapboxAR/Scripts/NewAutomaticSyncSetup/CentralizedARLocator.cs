namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using Mapbox.Unity.Location;
	using System;
	using Mapbox.MapMatching;
	using Mapbox.Utils;

	public class CentralizedARLocator : MonoBehaviour
	{

		// TODO : Snap should happening here for things to happen...
		// Lol. Snap Snap Snap... after yeach new better GPS val...

		[SerializeField]
		ARMapMatching _mapMathching;

		[SerializeField]
		bool _useSnapping;

		[SerializeField]
		float _desiredStartingAccuracy = 5f;

		[SerializeField]
		int _amountOfNodesToCheck;

		[SerializeField]
		int _desiredAccuracy;

		[SerializeField]
		NodeSyncBase[] _nodeSyncs;

		[SerializeField]
		Transform _player;

		Node[] _nodes;
		Location _highestLocation;

		public static Action<Location> OnNewHighestAccuracyGPS;

		void Start()
		{
			LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated += SaveHighestAccuracy;
		}

		void SaveHighestAccuracy(Location location)
		{
			if (location.Accuracy <= _desiredStartingAccuracy)
			{
				_highestLocation = location;
				_desiredStartingAccuracy = location.Accuracy;

				if (OnNewHighestAccuracyGPS != null)
				{
					OnNewHighestAccuracyGPS(location);
				}

				// TODO:
				// And snap player to there...
			}
		}

		void FindBestNodes()
		{
			foreach (var nodeSync in _nodeSyncs)
			{
				var average = CheckAverageAccuracy(nodeSync, _amountOfNodesToCheck);
				if (average <= _desiredAccuracy)
				{
					// TODO: Do map matching based on Nodes.
				}
			}
		}

		int CheckAverageAccuracy(NodeSyncBase syncBase, int howManyNodes)
		{
			var nodes = syncBase.ReturnNodes();
			int accuracy = 0;

			for (int i = 1; i < howManyNodes; i++)
			{
				accuracy += nodes[nodes.Length - i].Accuracy;
			}

			var average = accuracy / howManyNodes;
			return average;
		}

		void SnapMapToNode(Node node)
		{

		}

		// TODO: Check trackingQuality in AR 
		// and snap to GPS nodes if tracking goes bad..
	}
}
