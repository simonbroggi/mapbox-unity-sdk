namespace Mapbox.Unity.MiniMap
{
	using UnityEngine;
	using Mapbox.Unity.Map;
	//using DG.Tweening;
	using Mapbox.Utils;
	using Mapbox.Unity.Utilities;

	public enum Mode { AR, TopDown };

	public class MiniMapContoller : MonoBehaviour
	{
		// TODO : Update to not use tween engine. 
		[SerializeField]
		Transform _camTransform;

		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		Transform _targetRot;

		[SerializeField]
		Transform _debugShaderTransform;

		[SerializeField]
		Transform _debugTransformPos;

		Vector3 _origShadeTransformPos, _origCamPos;
		Vector3 _origMapPos;

		private void Start()
		{
			_origShadeTransformPos = _debugShaderTransform.localPosition;
			_origCamPos = _camTransform.localPosition;
			_origMapPos = _map.transform.localPosition;
		}

		public void SetMapMode(Mode mode)
		{
			switch (mode)
			{
				case Mode.AR:
					//_camTransform.DOLocalMove(_origCamPos, 1f, false);
					//_debugShaderTransform.DOLocalMove(_origShadeTransformPos, .8f, false);
					//_camTransform.DOLocalRotate(new Vector3(16, 0, 0), 1f, RotateMode.Fast);
					break;
				case Mode.TopDown:
					//_camTransform.DOLocalMove(new Vector3(0, _camTransform.localPosition.y, 0), 1f, false);
					//_debugShaderTransform.DOLocalMove(new Vector3(0, _debugShaderTransform.localPosition.y, 0), .8f, false);
					//_camTransform.DOLocalRotate(new Vector3(90, 0, 0), 1f, RotateMode.Fast);
					break;
				default:
					return;
			}
		}

		public void RotateMap(float heading)
		{
			//inverse
			heading *= -1;
			var rot = new Vector3(0, heading, 0);
			//_targetRot.DOKill();
			//_targetRot.DORotate(rot, 1f, RotateMode.Fast);
		}

		public void UpdateMapLocation(Vector2d latlon)
		{
			var coords = _map.GeoToWorldPosition(latlon, false);
			var deltaOffset = _map.transform.position - coords;
			//_map.transform.DOMove(new Vector3(deltaOffset.x, _map.transform.position.y, deltaOffset.z), 1f, false);
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				SetMapMode(Mode.TopDown);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				SetMapMode(Mode.AR);
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				RotateMap(-50f);
			}

			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				RotateMap(50f);
			}

			if (Input.GetKeyDown(KeyCode.L))
			{
				var location = new Vector2d(48.401243, 15.580996);
				UpdateMapLocation(location);
			}
		}

	}

}

