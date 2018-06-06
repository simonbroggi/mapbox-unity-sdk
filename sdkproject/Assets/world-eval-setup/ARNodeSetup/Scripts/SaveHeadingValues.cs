using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Location;

public class SaveHeadingValues : MonoBehaviour
{

	AngleSmoothingAverage _average;

	void Start()
	{
		_average = new AngleSmoothingAverage();
		LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated += SaveHeading;
	}

	void SaveHeading(Location location)
	{
		_average.Add(location.DeviceOrientation);
	}

	public float ReturnAverage()
	{
		float average = (float)_average.Calculate();
		return average;
	}
}
