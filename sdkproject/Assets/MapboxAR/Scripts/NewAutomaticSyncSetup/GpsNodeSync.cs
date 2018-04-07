namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Map;
	using Mapbox.Utils;
	using Mapbox.Unity.Location;


	///<summary>
	///  Generates GPSNodes for ARLocationManager.
	/// </summary>
	public class GpsNodeSync : MonoBehaviour, INodeSync
	{
		[SerializeField]
		bool _filterNodes, _replaceLatestNodeIfHigherAccuracy;

		//If true show range..

		List<Node> _listOfCachedNodes;

		void Start()
		{
			LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated += SaveNodes;
		}

		//TODO : So basically the GPS can be 15 off and then 4 off.. And it might send the location 
		//Pretty often. So basically if the player has not moved over certain threshold in ARCoordsSpace replace the latest GPS Node location with the new location...
		//This is for pedestrians.

		//Also need to check it out if the ARNode accuracy is lower than GPS accuracy to erase all the ARnodes and
		// Start Placing AR nodes again...... Also if AR tracking goes really low then replace placement with the latest GPS node again... When tracking state jumps up...
		//All of this should happen in the CentralizedARLocator.

		private void SaveNodes(Location location)
		{
			if (_filterNodes)
			{
				//Check Node accuracy;
			}

			var latestNode = new Node();
			latestNode.LatLon = location.LatitudeLongitude;
			latestNode.Accuracy = location.Accuracy;
			_listOfCachedNodes.Add(latestNode);
		}

		public Node[] ReturnNodes()
		{
			return _listOfCachedNodes.ToArray();
		}

		private void FilterNodes()
		{

		}

	}

	public struct Node
	{
		/// <summary>
		/// Represents the saved Latitude Longitude value of the Node.
		/// </summary>
		public Vector2d LatLon;
		/// <summary>
		/// Accuracy of the Node. ARNodes accuracy is determined by the latest and most accurate GPS point.
		/// </summary>
		public float Accuracy;
	}

	public interface INodeSync
	{
		/// <summary>
		/// Returns an array of nodes.
		/// </summary>
		Node[] ReturnNodes();
	}

}

