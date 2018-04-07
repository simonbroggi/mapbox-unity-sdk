using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.MapMatching;
using Mapbox.Utils;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityARInterface;

public class AutomaticMapMatching : MonoBehaviour
{

	[SerializeField]
	AbstractMap _map;

	[SerializeField]
	GameObject _player;

	[SerializeField]
	GameObject _debugCube;

	private List<Location> _gpsNodes;
	private List<Vector2d> _arNodes;

	MapMatchingResource _mapMatcher;

	// Use this for initialization
	void Start()
	{
		_gpsNodes = new List<Location>();
		LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated += StoreLatLonFromGPS;
		//_map.OnInitialized += SnapAfterInit;
	}

	//This is called everytime a user walks through a cube.
	//See how many of this you need to have in order to have accurate
	//Localiationss.s.s.s..s

	public void StoreLatLonFromUnityCoordSpace()
	{
		//TODO: Add and convert locations from the map coord space to Vector2D to 
		// Map matching. Will this even work? If the map itself is mis-aligned 
		// regardless..
		//Map matching works best with a sample rate of 5 seconds between points.
	}

	public void StoreLatLonFromGPS(Location location)
	{
		_gpsNodes.Add(location);
	}

	private void MapMatching()
	{
		//So basically get the last waypoint and re-align the map to that...
		// you need to do it instantly when the call is returned...
		//otherwise it will be mis-aligned.. yaaay. cool man. cool beans.
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SnapAfterInit();
		}
	}
	private void SnapAfterInit()
	{

		//TODO: Find the nearest road and snap to it.
		//Make the user walk through two points to derive heading..
		//Get the approx heading to align the map in front of the
		var closestRoad = SnapToRoad.Instance.ReturnClosestRoadToVector(_player.transform.position);
		Debug.Log(closestRoad.name);
		var vectorToSnap = SnapToRoad.Instance.ReturnSnapPoint(closestRoad, _player.transform.position);
		Debug.Log(vectorToSnap);
		_debugCube.transform.position = vectorToSnap;
	}
}

