namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Location;
	using Mapbox.Utils;

	public class GPSHeadingSync : HeadingSyncBase
	{
		[SerializeField]
		float _allowedHeadingChange;

		float _initialHeading;
		CircularBuffer<float> _headingValues;

		private void Awake()
		{
			_headingValues = new CircularBuffer<float>(20);
			LocationProviderFactory.Instance.DeviceLocationProvider.OnLocationUpdated += SaveInitialHeading;
			LocationProviderFactory.Instance.DeviceLocationProvider.OnLocationUpdated += GetLocationUpdates;
		}

		void SaveInitialHeading(Location location)
		{
			_initialHeading = location.DeviceOrientation;
			LocationProviderFactory.Instance.DeviceLocationProvider.OnLocationUpdated -= SaveInitialHeading;
		}

		void GetLocationUpdates(Location location)
		{
			SaveHeading(location.DeviceOrientation, _allowedHeadingChange);
		}

		void SaveHeading(float heading, float allowedAngleDifference)
		{
			float average = GetAverageHeading(_headingValues);

			if (heading >= (average + allowedAngleDifference) || heading <= (average - allowedAngleDifference))
			{
				Debug.Log("setting new headings");
				_headingValues = new CircularBuffer<float>(20);
				_headingValues.Add(heading);
				return;
			}

			Debug.Log("Saving heading");
			_headingValues.Add(heading);
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

		// Think of this.. Because each time you pass in the initial heading..
		// You must remember if tracking goes bananas you need re-establish the 
		// Initial new heading and pass that to the offset Method. Where do you do that?
		// AR Alignment strategy

		public override float ReturnAverageHeading()
		{
			return GetAverageHeading(_headingValues);
		}

		public override float ReturnLatestHeading()
		{
			return _headingValues[0];
		}

		public override float ReturnAverageOffsetHeading(float initialHeading, float arHeading)
		{
			var offset = (initialHeading + GetAverageHeading(_headingValues) - arHeading);
			return offset;
		}

		public override float ReturnInitialHeading()
		{
			return _initialHeading;
		}
	}

}
