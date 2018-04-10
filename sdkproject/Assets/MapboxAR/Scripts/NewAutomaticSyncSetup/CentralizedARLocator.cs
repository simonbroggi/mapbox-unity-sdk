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
		NodeSyncBase[] _nodeSyncs;
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

		void CalculateARPosition()
		{
			foreach (var nodeSync in _nodeSyncs)
			{
				_nodes = nodeSync.ReturnNodes();

				foreach (var node in _nodes)
				{

				}
			}
		}

		void SnapPlayerToTheMostAccurateNode()
		{

		}

		// TODO: Check trackingQuality in AR 
		// and snap to GPS nodes if tracking goes bad..
	}
}
