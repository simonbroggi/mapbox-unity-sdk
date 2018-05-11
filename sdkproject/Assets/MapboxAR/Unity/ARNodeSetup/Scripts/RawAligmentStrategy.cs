namespace Mapbox.Unity.Ar
{
	using UnityEngine;
	using Mapbox.Unity.Map;
	using Mapbox.Utils;

	public class RawAligmentStrategy : AbstractAlignmentStrategy
	{
		[SerializeField]
		AbstractMap _map;
		bool _isInitialized;

		private void Awake()
		{
			_map.OnInitialized += () => _isInitialized = true;
		}

		public override void OnAlignmentAvailable(Alignment alignment)
		{
			if (_isInitialized)
			{
				var latlon = _map.WorldToGeoPosition(alignment.Position);
				_map.UpdateMap(latlon, _map.Zoom);
				var pos = new Vector3(_map.transform.position.x, alignment.Position.y, _map.transform.position.z);
				var rotation = Quaternion.Euler(0, alignment.Rotation, 0);
				var inverse = Quaternion.Inverse(rotation);
				_map.transform.SetPositionAndRotation(pos, inverse);
			}
		}
	}
}

