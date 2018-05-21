namespace Mapbox.Unity.Ar
{
	using UnityEngine;
	using System;
	using UnityARInterface;
	using Mapbox.Unity.Location;
	using System.Linq;
	using Mapbox.Utils;

	public class ARLocalizationStrategy : ComputeARLocalizationStrategy
	{
		public override event Action<Alignment> OnLocalizationComplete;

		[SerializeField]
		Transform _arFpsCamera;

		[SerializeField]
		Transform _arRoot;

		[SerializeField]
		float _updateHeadingInterval = 30;

		Vector3 _mapMatchNode;
		float _timeToUpdateHeading;
		float _cacheHeading;

		ARInterface.CustomTrackingState _trackingState;
		ARInterface _arInterface;
		bool _isTrackingGood;
		float _planePosOnY = -.5f;

		private void Start()
		{
			_arInterface = ARInterface.GetInterface();
			_trackingState = new ARInterface.CustomTrackingState();
			ARInterface.planeAdded += GetPlaneCoords;
			ARInterface.planeUpdated += GetPlaneCoords;
			_timeToUpdateHeading = _updateHeadingInterval;
		}

		public override void ComputeLocalization(CentralizedLocator centralizedARLocator)
		{
			var currentLocation = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
			var aligment = new Alignment();
			var map = centralizedARLocator.CurrentMap;
			var averageHeading = centralizedARLocator.HeadingSync.ReturnAverageHeading();

			if (CheckTracking())
			{
				// If tracking is good. Keep map at same pos. Need to find way to update location
				// if it's better, because this assumes initial location is solid.

				var mapPos = map.GeoToWorldPosition(map.CenterLatitudeLongitude, false);
				var newPos = new Vector3(mapPos.x, _planePosOnY, mapPos.z);

				aligment.Position = newPos;
				aligment.Rotation = _cacheHeading;

				if (_timeToUpdateHeading <= 0)
				{
					Debug.Log("times up");

					// TODO; Get average heading...
					Debug.Log("average heading: " + averageHeading);
					aligment.IsAr = true;
					aligment.Rotation = averageHeading;
					aligment.Position = new Vector3(_arFpsCamera.position.x, _planePosOnY, _arFpsCamera.position.z);
					_cacheHeading = aligment.Rotation;

					Unity.Utilities.Console.Instance.Log(string.Format
														 ("New average heading: {0}", aligment.Rotation)
														 , "red");

					_timeToUpdateHeading = _updateHeadingInterval;
				}

				Unity.Utilities.Console.Instance.Log(string.Format("Keeping at Location")
														 , "green");
				OnLocalizationComplete(aligment);

				return;
			}

			// FOR MapMatching Node..
			if (CanSnatchMapMatchingNode(centralizedARLocator, ref _mapMatchNode))
			{
				Unity.Utilities.Console.Instance.Log(string.Format("Snatched MapMatchNode: {0}", _mapMatchNode)
					, "yellow"
				);

				aligment.Position = _mapMatchNode;
				aligment.Rotation = averageHeading;

				Unity.Utilities.Console.Instance.Log(string.Format("Aligning map by MapMatchingNode")
					, "yellow"
				);

				SetPlayerOnAR(_mapMatchNode);

				OnLocalizationComplete(aligment);
				return;
			}

			// FOR GPS...
			foreach (var nodeBase in centralizedARLocator.SyncNodes)
			{
				if (nodeBase.GetType() == typeof(GpsNodeSync))
				{
					var node = nodeBase.ReturnLatestNode();
					var newGeoPos = map.GeoToWorldPosition(node.LatLon, false);
					newGeoPos.y = _arFpsCamera.position.y - 1f;
					aligment.Position = newGeoPos;
					aligment.Rotation = averageHeading;

					SetPlayerOnAR(newGeoPos);
				}
			}

			Unity.Utilities.Console.Instance.Log(string.Format("Aligning map by GPSNode")
					, "blue"
				);

			OnLocalizationComplete(aligment);
		}


		// Moved to GPSHeadingSync

		//void SaveHeading(float heading, float allowedAngleDifference)
		//{
		//	// TODO: Remember to save first heading before checking average.
		//	float average = GetAverageHeading(_headingValues);

		//	if (heading >= (average + allowedAngleDifference) || heading <= (average - allowedAngleDifference))
		//	{
		//		Debug.Log("setting new headings");
		//		_timeToUpdateHeading = _updateHeadingInterval;
		//		_headingValues = new CircularBuffer<float>(20);
		//		_headingValues.Add(heading);
		//		return;
		//	}

		//	Debug.Log("Saving heading");
		//	_headingValues.Add(heading);
		//}

		void SetPlayerOnAR(Vector3 pos)
		{
			//HACK: This kinda should happen in AligmentStrategy. But only,
			// when gps aligment is used..
			_arRoot.position = pos;
			_arFpsCamera.position = Vector3.zero;

			Unity.Utilities.Console.Instance.Log(
			string.Format(
				"Player Pos Reset: {0}, RootPos: {1}"
				, _arFpsCamera.position

				)
				, "purple"
			);
		}

		//float GetAverageHeading(CircularBuffer<float> headingValues)
		//{
		//	float accuracy = 0;
		//	int valuesCount = headingValues.Count;

		//	foreach (var headingVal in headingValues)
		//	{
		//		accuracy += headingVal;
		//	}

		//	var average = accuracy / valuesCount;
		//	return average;
		//}

		float GetPlayerAngle()
		{
			Vector3 targetPos = new Vector3(_arFpsCamera.position.x, _arFpsCamera.position.y - 1f, _arFpsCamera.position.z);
			Vector3 targetDir = targetPos - _arFpsCamera.position;
			return Vector3.Angle(targetDir, _arFpsCamera.forward);
		}

		private void Update()
		{
			_timeToUpdateHeading -= Time.deltaTime;
		}

		void GetPlaneCoords(BoundedPlane plane)
		{
			_planePosOnY = plane.center.y;
		}

		bool CanSnatchMapMatchingNode(CentralizedLocator arLocator, ref Vector3 vector3)
		{
			foreach (var nodebase in arLocator.SyncNodes)
			{
				if (nodebase.GetType() == typeof(ARMapMatching))
				{
					if (nodebase.ReturnNodes().Count() != 0)
					{
						var nodes = nodebase.ReturnNodes();
						vector3 = arLocator.CurrentMap.GeoToWorldPosition(nodes[0].LatLon, false);
						return true;
					}
				}
			}
			return false;
		}

		bool CheckTracking()
		{

			if (_arInterface.GetTrackingState(ref _trackingState))
			{
				Unity.Utilities.Console.Instance.Log(
				string.Format(
					"ARTracking State: {0}"
						, _trackingState
				)
				, "white"
			);

				Debug.Log((_trackingState));

				if (_trackingState == ARInterface.CustomTrackingState.Good)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}

}

