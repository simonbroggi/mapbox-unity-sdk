namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Location;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;

	public class GPSHeadingSync : HeadingSyncBase
	{

		[SerializeField]
		float _allowedHeadingChange = 70;

		float _initialHeading;
		CircularBuffer<float> _headingValues;


		private void Start()
		{
			_headingValues = new CircularBuffer<float>(20);
			LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated += SaveInitialHeading;
		}

		void SaveInitialHeading(Location location)
		{
			Debug.Log("Saved Initial heading: " + location.DeviceOrientation);

			_initialHeading = location.DeviceOrientation;

			_headingValues.Add(_initialHeading);

			LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated -= SaveInitialHeading;
			LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated += GetLocationUpdates;
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

		public override float ReturnAverageHeading()
		{
			return GetAverageHeading(_headingValues);
		}

		public override float ReturnLatestHeading()
		{
			return _headingValues[0];
		}

		public override float ReturnInitialHeading()
		{
			return _initialHeading;
		}
	}

}
