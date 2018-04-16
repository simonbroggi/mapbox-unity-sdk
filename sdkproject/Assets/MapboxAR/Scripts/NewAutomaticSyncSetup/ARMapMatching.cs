namespace Mapbox.Unity.Ar
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.MapMatching;
	using Mapbox.Utils;
	using Mapbox.Platform;
	using Mapbox.Unity.Map;

	public class ARMapMatching : NodeSyncBase
	{
		[SerializeField]
		MapMatching.Profile _profile;

		[SerializeField]
		AbstractMap _map;

		public Action<Node[]> ReturnMapMatchCoords;
		Node[] _savedNodes;
		IEnumerator _mapMatching, _waitForRequest;
		WaitForSeconds _waitFor;
		FileSource _fs;
		private int _timeout = 10;

		void Awake()
		{
			_fs = new FileSource(MapboxAccess.Instance.Configuration.AccessToken);
			_timeout = MapboxAccess.Instance.Configuration.DefaultTimeout;
		}

		public void MapMatchQuery(Node[] nodes)
		{

			Vector2d[] coordinates = new Vector2d[nodes.Length];
			for (int i = 0; i < nodes.Length; i++)
			{
				coordinates[i] = nodes[i].LatLon;
			}

			_mapMatching = SimpleQuery(coordinates);
			StartCoroutine(_mapMatching);
		}

		IEnumerator SimpleQuery(Vector2d[] coords)
		{
			MapMatchingResource resource = new MapMatchingResource();
			resource.Coordinates = coords;
			resource.Profile = _profile;
			MapMatcher matcher = new MapMatcher(_fs, _timeout);
			MapMatchingResponse matchResponse = null;
			matcher.Match(
				resource,
				(MapMatchingResponse responce) =>
			 {
				 matchResponse = responce;
			 }
			);

			IEnumerator enumerator = _fs.WaitForAllRequests();

			while (enumerator.MoveNext())
			{
				yield return null;
			}

			SendResponseCoords(matchResponse);
		}

		void SendResponseCoords(MapMatchingResponse response)
		{
			var coordinates = response.Matchings[0].Geometry;
			var quality = response.Matchings[0].Confidence;
			var nodes = new Node[coordinates.Count];

			if (ReturnMapMatchCoords != null)
			{
				for (int i = 0; i < coordinates.Count; i++)
				{
					nodes[i].Confidence = quality;
					nodes[i].LatLon = coordinates[i];
				}
				ReturnMapMatchCoords(nodes);
				_savedNodes = nodes;
			}

			if (NodeAdded != null)
			{
				NodeAdded();
			}
		}

		public override Node[] ReturnNodes()
		{
			return _savedNodes;
		}

		public override Node ReturnLatestNode()
		{
			return _savedNodes[_savedNodes.Length - 1];
		}
	}
}