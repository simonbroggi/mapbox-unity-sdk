namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;

	public class WorldAnchorPoint : MonoBehaviour
	{

		[SerializeField]
		double _latitude;

		[SerializeField]
		double _longitude;

		Vector2d _latLon;

		void Start()
		{
			StaticMapReference.Instance.MapInitialized += SetWorldPoint;
		}

		void SetWorldPoint()
		{
			_latLon = new Vector2d(_latitude, _longitude);
			var map = StaticMapReference.Instance.ReturnMap();
			transform.position = map.GeoToWorldPosition(_latLon);
		}
	}
}
