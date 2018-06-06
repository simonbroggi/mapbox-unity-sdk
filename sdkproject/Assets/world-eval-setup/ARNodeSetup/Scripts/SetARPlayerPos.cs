namespace Mapbox.Unity.Ar
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using DG.Tweening;

	public class SetARPlayerPos : MonoBehaviour
	{
		[SerializeField]
		ComputeARLocalizationStrategy _initialLocation;

		[SerializeField]
		Transform _arRoot;

		[SerializeField]
		Transform _playerCamera;

		[SerializeField]
		AbstractMap _map;

		float _initialHeading;

		private void Awake()
		{
			_initialLocation.OnLocalizationComplete += SaveInitialHeading;
		}

		//HACK this should be accessible from the LocationProvider.
		void SaveInitialHeading(Alignment alignment)
		{
			_initialHeading = alignment.Rotation;
		}

		public void SetArPos(Vector2d location, float heading)
		{
			//These values should be accessible from the location provider as well...
			//Like locationProvider.Instance.AR.position and .rotation

			//calculate heading offset
			var headingDelta = heading - _playerCamera.transform.localEulerAngles.y;

			//offset rotation
			_arRoot.transform.eulerAngles = new Vector3(0, 0, 0);
			var playerPosBefore = _playerCamera.transform.position;
			_arRoot.transform.eulerAngles = new Vector3(0, headingDelta, 0);
			var rotationOffset = _playerCamera.transform.position - playerPosBefore;

			//calculate position offset
			var coords = _map.GeoToWorldPosition(location, false);
			var delta = coords - _map.Root.transform.position;

			//offset position
			var offset = delta - _playerCamera.transform.localPosition - rotationOffset;
			_arRoot.transform.position = new Vector3(offset.x, _arRoot.transform.position.y, offset.z);
		}
	}
}

