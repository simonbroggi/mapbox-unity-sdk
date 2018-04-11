namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using Mapbox.Unity.Location;
	using System;

	public class CentralizedARLocator : MonoBehaviour
	{

		// TODO : Snap should happening here for things to happen...
		// Lol. Snap Snap Snap... after yeach new better GPS val...

		[SerializeField]
		bool _useSnapping;

		[SerializeField]
		float _desiredStartingAccuracy = 5f;

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
				var mean = CheckMeanAccuracy(nodeSync, 5);
				if (mean <= _desiredAccuracy)
				{
					// TODO: Do map matching based on Nodes.
				}
			}
		}

		int CheckMeanAccuracy(NodeSyncBase syncBase, int howManyNodes)
		{
			var nodes = syncBase.ReturnNodes();
			int accuracy = 0;

			for (int i = 1; i < howManyNodes; i++)
			{
				accuracy += nodes[nodes.Length - i].Accuracy;
			}

			var mean = accuracy / howManyNodes;
			return mean;
		}

		void SnapMapToNode(Node node)
		{

		}

		// TODO: Check trackingQuality in AR 
		// and snap to GPS nodes if tracking goes bad..
	}
}
