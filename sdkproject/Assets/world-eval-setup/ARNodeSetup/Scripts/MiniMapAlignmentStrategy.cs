namespace Mapbox.Unity.Ar
{
	using MiniMap;
	using UnityEngine;

	public class MiniMapAlignmentStrategy : AbstractAlignmentStrategy
	{
		[SerializeField]
		MiniMapContoller _minimap;

		void Start()
		{
			Register(CentralizedLocator.Instance);
		}

		private void OnDisable()
		{
			//Unregister(CentralizedLocator.Instance);
		}

		public override void OnAlignmentAvailable(Alignment alignment)
		{
			_minimap.RotateMap(alignment.Rotation);
			_minimap.UpdateMapLocation(alignment.LatLon);
		}

	}

}
