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
		GPSHeadingSync _headingSync;

		[SerializeField]
		SaveHeadingValues _heading;

		[SerializeField]
		float _setHeadingIntervalTime;

		ARInterface.CustomTrackingState _trackingState;
		ARInterface _arInterface;
		Transform _arFpsCamera;

		float _previousAccuracy, _previousHeading, _headingUpdateTime;

		NodeSyncBase _gpsNodeSync, _arNodeSync, _mapMatchingNode;


		private void Start()
		{
			_arInterface = ARInterface.GetInterface();
			_trackingState = new ARInterface.CustomTrackingState();
			_arFpsCamera = CentralizedLocator.Instance.ArFirstPerson;
			_headingUpdateTime = _setHeadingIntervalTime;

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


			// Check gps if it's better, then check if heading is better, then if none of those are better then 
			// use only ar or gps.

			// Basic strategy
			if (CheckTracking())
			{
				aligment.Position = firstPersonPos.position;
				aligment.LatLon = map.WorldToGeoPosition(firstPersonPos.position);
				aligment.Rotation = firstPersonPos.eulerAngles.y;

				if (_gpsNodeSync.ReturnLatestNode().Accuracy <= _previousAccuracy)
				{
					aligment.Position = map.GeoToWorldPosition(_gpsNodeSync.ReturnLatestNode().LatLon, false);
					aligment.LatLon = _gpsNodeSync.ReturnLatestNode().LatLon;
					_previousAccuracy = _gpsNodeSync.ReturnLatestNode().Accuracy;
				}

				if (_headingUpdateTime <= 0)
				{
					aligment.Rotation = _headingSync.ReturnAverageHeading();
					Unity.Utilities.Console.Instance.Log(string.Format
												 ("New UserHeading heading: {0}", aligment.Rotation)
												 , "red");
					_headingUpdateTime = _setHeadingIntervalTime;
				}

				OnLocalizationComplete(aligment);
				return;
			}

			// GPS
			aligment.Position = map.GeoToWorldPosition(location.LatitudeLongitude, false);
			aligment.LatLon = location.LatitudeLongitude;
			aligment.Rotation = location.DeviceOrientation;
			OnLocalizationComplete(aligment);

		}

		private void Update()
		{
			_headingUpdateTime -= Time.deltaTime;
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

