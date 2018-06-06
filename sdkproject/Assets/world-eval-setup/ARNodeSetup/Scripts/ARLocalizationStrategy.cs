namespace Mapbox.Unity.Ar
{
	using UnityEngine;
	using System;
	using UnityARInterface;
	using Mapbox.Unity.Location;
	using System.Linq;
	using Mapbox.Utils;
	using Mapbox.Unity.MiniMap;

	public class ARLocalizationStrategy : ComputeARLocalizationStrategy
	{
		public override event Action<Alignment> OnLocalizationComplete;

		[SerializeField]
		SaveHeadingValues _heading;

		[SerializeField]
		MiniMapContoller _miniMap;

		[SerializeField]
		float _updateHeadingInterval = 10;

		Vector3 _mapMatchNode;
		float _timeToUpdateHeading;

		ARInterface.CustomTrackingState _trackingState;
		ARInterface _arInterface;
		bool _isHeadingCached;
		float _planePosOnY = -.5f;
		Transform _arFpsCamera;
		float _cacheHeading;
		NodeSyncBase _gpsNodeSync, _arNodeSync, _mapMatchingNode;

		private void Start()
		{
			//Well this should be here...
			_arInterface = ARInterface.GetInterface();
			_trackingState = new ARInterface.CustomTrackingState();
			_arFpsCamera = CentralizedLocator.Instance.ArFirstPerson;
			//Not sure about this.. Should it also... I need to place the map..
			//Because were not really anymore updating the map...
			//Guess not.. maan what the fuck am I doing?.
			ARInterface.planeAdded += GetPlaneCoords;
			ARInterface.planeUpdated += GetPlaneCoords;
			_timeToUpdateHeading = _updateHeadingInterval;


			//Awful
			foreach (var nodeBase in CentralizedLocator.Instance.SyncNodes)
			{
				if (nodeBase.GetType() == typeof(GpsNodeSync))
				{
					_gpsNodeSync = nodeBase;
				}
				if (nodeBase.GetType() == typeof(ARMapMatching))
				{
					_mapMatchingNode = nodeBase;
				}
				if (nodeBase.GetType() == typeof(ArNodesSync))
				{
					_arNodeSync = nodeBase;
				}
			}
		}

		public override void ComputeLocalization(CentralizedLocator centralizedARLocator)
		{
			var aligment = new Alignment();
			var map = centralizedARLocator.CurrentMap;
			var firstPersonPos = centralizedARLocator.ArFirstPerson;
			var location = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;

			//Bascically 
			if (CheckTracking())
			{
				aligment.Position = firstPersonPos.position;
				aligment.LatLon = map.WorldToGeoPosition(firstPersonPos.position);
				aligment.Rotation = firstPersonPos.eulerAngles.y;

				Debug.Log(aligment.LatLon);

				if (_timeToUpdateHeading <= 0)
				{
					aligment.Rotation = _heading.ReturnAverage();
					_cacheHeading = _heading.ReturnAverage();
					_isHeadingCached = true;
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

			if (CanSnatchMapMatchingNode(centralizedARLocator, ref _mapMatchNode) && _gpsNodeSync.ReturnLatestNode().Accuracy <= 20)
			{
				Unity.Utilities.Console.Instance.Log(string.Format("Snatched MapMatchNode: {0}", _mapMatchNode)
					, "yellow"
				);

				aligment.Position = _mapMatchNode;
				//aligment.Rotation = averageHeading;

				Unity.Utilities.Console.Instance.Log(string.Format("Aligning map by MapMatchingNode")
					, "yellow"
				);

				OnLocalizationComplete(aligment);
				return;
			}

			//GPS
			aligment.Position = map.GeoToWorldPosition(location.LatitudeLongitude, false);
			aligment.LatLon = location.LatitudeLongitude;
			aligment.Rotation = _heading.ReturnAverage();
			OnLocalizationComplete(aligment);

		}

		float ReturnPlayerAngle()
		{
			Vector3 targetPos = new Vector3(_arFpsCamera.position.x, _arFpsCamera.position.y - 1f, _arFpsCamera.position.z);
			Vector3 targetDir = targetPos - _arFpsCamera.position;
			return Vector3.Angle(targetDir, _arFpsCamera.forward);
		}

		private void Update()
		{
			_timeToUpdateHeading -= Time.deltaTime;


			if (ReturnPlayerAngle() >= 70)
			{
				_miniMap.SetMapMode(Mode.AR);
			}

			if (ReturnPlayerAngle() < 50)
			{
				_miniMap.SetMapMode(Mode.TopDown);
			}
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

