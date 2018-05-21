namespace Mapbox.Unity.Ar
{
	using UnityEngine;
	using Mapbox.Unity.Map;
	using Mapbox.Utils;

	public class RawAligmentStrategy : AbstractAlignmentStrategy
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		Transform _mapHolder;

		bool _isInitialized;

		private void Awake()
		{
			_map.OnInitialized += () => _isInitialized = true;
		}

		public override void OnAlignmentAvailable(Alignment alignment)
		{
			if (_isInitialized)
			{
				// Place map on arplane;
				var mapPos = _map.transform.position;
				_map.transform.position = new Vector3(mapPos.x, alignment.ARPlaneY, mapPos.z);

				// Update root pos...

				var rotation = Quaternion.Euler(0, alignment.Rotation, 0);
				var inverse = Quaternion.Inverse(rotation);

			}
		}

		Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			return Quaternion.Euler(angles) * (point - pivot) + pivot;
		}
	}
}

