namespace Mapbox.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Map;
	using UnityEngine.UI;

	public class DropPinOnMap : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		Camera _cam;

		[SerializeField]
		GameObject _pin;

		private void Start()
		{

		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0) && _cam.rect.y >= 0)
			{
				Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit))
				{
					Debug.Log(hit.collider.gameObject.name);
					Instantiate(_pin, hit.point, Quaternion.identity, _map.gameObject.transform);
				}
			}
		}

		public void SetCameraViewPort()
		{
			var rectVal = _cam.rect.y >= 0f ? -.66f : 0;
			_cam.rect = new Rect(_cam.rect.x, rectVal, _cam.rect.width, _cam.rect.height);
		}
	}
}
