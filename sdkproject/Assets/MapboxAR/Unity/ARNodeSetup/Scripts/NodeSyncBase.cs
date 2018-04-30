namespace Mapbox.Unity.Ar
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Location;
	using Mapbox.Unity.Map;

	public abstract class NodeSyncBase : MonoBehaviour
	{
		/// <summary>
		/// Returns the nodes that the sync base has collected.
		/// </summary>
		/// <returns>The nodes.</returns>
		public abstract int ReturnNodeCount();
		/// <summary>
		/// Returns the latest node added to sync base.
		/// </summary>
		/// <returns>The latest node.</returns>
		public abstract Node ReturnLatestNode();
		/// <summary>
		/// Returns node at the index. Position is reversed. 0 returns Latest node.
		/// </summary>
		/// <returns>The node at index.</returns>
		/// <param name="index">Index.</param>
		public abstract Node ReturnNodeAtIndex(int index);
		/// <summary>
		/// Initializes the node base.
		/// </summary>
		public abstract void InitializeNodeBase(AbstractMap map);
		/// <summary>
		/// Saves the node.
		/// </summary>
		public abstract void SaveNode();
		/// <summary>
		/// Returns true if Node Base is Intialized for filtering.
		/// </summary>
		public bool IsNodeBaseInitialized;
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
		/// <summary>
		/// Represents the Confidence of a Map Matching node. Not valid on ARNode or GPSNode.
		/// </summary>
		public float Confidence;
	}


}
