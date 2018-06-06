namespace Mapbox.Unity.Ar
{
	using UnityEngine;

	public class ArUserAlignmentStrategy : AbstractAlignmentStrategy
	{

		private void OnDisable()
		{
			Unregister(CentralizedLocator.Instance);
		}

		public override void OnAlignmentAvailable(Alignment alignment)
		{
			//calculate heading offset
			var player = CentralizedLocator.Instance.ArFirstPerson;
			var headingDelta = alignment.Rotation - player.localEulerAngles.y;

			//offset rotation
			var arRoot = CentralizedLocator.Instance.ARRoot;
			arRoot.eulerAngles = new Vector3(0, 0, 0);
			var playerPosBefore = player.transform.position;
			arRoot.eulerAngles = new Vector3(0, headingDelta, 0);
			var rotationOffset = player.position - playerPosBefore;

			//calculate position offset
			var delta = alignment.Position - CentralizedLocator.Instance.CurrentMap.transform.position;

			//offset position
			var offset = delta - player.localPosition - rotationOffset;
			arRoot.position = new Vector3(offset.x, arRoot.position.y, offset.z);
		}
	}

}
