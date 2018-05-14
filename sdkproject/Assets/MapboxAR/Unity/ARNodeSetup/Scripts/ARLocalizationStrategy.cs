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
		Transform _player;

		[SerializeField]
		Transform _arRoot;

		ARInterface.CustomTrackingState _trackingState;
		ARInterface _arInterface;
		bool _isTrackingGood, _setUserHeading;
		float _planePosOnY = -.5f;

		[SerializeField]
		float _updateHeadingInterval = 30;

		private Vector3 _mapMatchNode;
		private float _timeToUpdateHeading;
		CircularBuffer<float> _headingValues;
		private float _cacheHeading;

		private void Start()
		{
			_arInterface = ARInterface.GetInterface();
			_trackingState = new ARInterface.CustomTrackingState();
			ARInterface.planeAdded += GetPlaneCoords;
			ARInterface.planeRemoved += GetPlaneCoords;
			_headingValues = new CircularBuffer<float>(20);
			_timeToUpdateHeading = _updateHeadingInterval;
		}

		public override void ComputeLocalization(CentralizedARLocator centralizedARLocator)
		{
			var currentLocation = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
			var aligment = new Alignment();
			var map = centralizedARLocator.CurrentMap;

			if (_setUserHeading)
			{
				SaveHeading(currentLocation.DeviceOrientation, 70f);
			}

			if (!_setUserHeading)
			{
				_headingValues.Add(currentLocation.DeviceOrientation);
				_cacheHeading = currentLocation.DeviceOrientation;
				_setUserHeading = true;
				Debug.Log("testing");
			}


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
					Debug.Log("average heading: " + GetAverageHeading(_headingValues));
					aligment.Rotation = GetAverageHeading(_headingValues);
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
				aligment.Rotation = GetAverageHeading(_headingValues);

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
					newGeoPos.y = _player.position.y - 1f;
					aligment.Position = newGeoPos;
					aligment.Rotation = GetAverageHeading(_headingValues);

					SetPlayerOnAR(newGeoPos);
				}
			}

			Unity.Utilities.Console.Instance.Log(string.Format("Aligning map by GPSNode")
					, "blue"
				);

			OnLocalizationComplete(aligment);
		}

		void SaveHeading(float heading, float allowedAngleDifference)
		{
			// TODO: Remember to save first heading before checking average.
			float average = GetAverageHeading(_headingValues);

			if (heading >= (average + allowedAngleDifference) || heading <= (average - allowedAngleDifference))
			{
				Debug.Log("setting new headings");
				_timeToUpdateHeading = _updateHeadingInterval;
				_headingValues = new CircularBuffer<float>(20);
				_headingValues.Add(heading);
				return;
			}

			Debug.Log("Saving heading");
			_headingValues.Add(heading);
		}

		void SetPlayerOnAR(Vector3 pos)
		{
			//HACK: This kinda should happen in AligmentStrategy. But only,
			// when gps aligment is used..

			_player.position = Vector3.zero;
			_arRoot.position = pos;

		}

		float GetAverageHeading(CircularBuffer<float> headingValues)
		{
			float accuracy = 0;
			int valuesCount = headingValues.Count;

			foreach (var headingVal in headingValues)
			{
				accuracy += headingVal;
			}

			var average = accuracy / valuesCount;
			return average;
		}

		float GetPlayerAngle()
		{
			Vector3 targetPos = new Vector3(_player.position.x, _player.position.y - 1f, _player.position.z);
			Vector3 targetDir = targetPos - _player.position;
			return Vector3.Angle(targetDir, _player.forward);
		}

		private void Update()
		{
			_timeToUpdateHeading -= Time.deltaTime;
		}

		void GetPlaneCoords(BoundedPlane plane)
		{
			_planePosOnY = plane.center.y;
		}

		bool CanSnatchMapMatchingNode(CentralizedARLocator arLocator, ref Vector3 vector3)
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

