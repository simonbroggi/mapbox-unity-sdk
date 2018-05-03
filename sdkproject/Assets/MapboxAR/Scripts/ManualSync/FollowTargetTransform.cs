namespace Mapbox.Examples
{
	using UnityEngine;

	public class FollowTargetTransform : MonoBehaviour
	{
		[SerializeField]
		Transform _targetTransform;

		void Update()
		{
			transform.position = new Vector3(_targetTransform.position.x, transform.position.y, _targetTransform.position.z);

			//var targetEulerRotation = _targetTransform.rotation.eulerAngles;
			//var eulerRotation = new Vector3(90f, targetEulerRotation.y, 0f);

			//transform.rotation = Quaternion.Euler(eulerRotation);
		}
	}
}

