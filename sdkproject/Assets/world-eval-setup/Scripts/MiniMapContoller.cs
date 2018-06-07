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
		bool _isLerping, _isMapInitialized;
		float _timeStartedLerping, timeTakenDuringLerp;
		Vector2d _startLatLong, _endLatlong;
		Vector3 _startPosition, _endPosition;

		private void Start()
		{
			_origShadeTransformPos = _debugShaderTransform.localPosition;
			_origCamPos = _camTransform.localPosition;
			_origMapPos = _map.transform.localPosition;
			_map.OnInitialized += () => _isMapInitialized = true;
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
			_targetRot.eulerAngles = rot;
			//_targetRot.DOKill();
			//_targetRot.DORotate(rot, 1f, RotateMode.Fast);
		}

		public void UpdateMapLocation(Vector2d latlon)
		{

			StartLerping(latlon);
			//var coords = _worldMap.GeoToWorldPosition(latlon, false);
			//var deltaOffset = _map.transform.localPosition - coords;
			////_map.UpdateMap(latlon, _map.AbsoluteZoom);
			//Debug.Log(new Vector3(coords.x, _map.transform.localPosition.y, coords.z).ToString());
			//_map.transform.localPosition = new Vector3(coords.x, _map.transform.localPosition.y, coords.z);
			//_map.transform.DOMove(new Vector3(deltaOffset.x, _map.transform.position.y, deltaOffset.z), 1f, false);
		}

		/// <summary>
		/// Called to begin the linear interpolation
		/// </summary>
		void StartLerping(Vector2d latlon)
		{
			_isLerping = true;
			_timeStartedLerping = Time.time;
			//Debug.Log(Time.deltaTime);
			timeTakenDuringLerp = Time.deltaTime;

			//We set the start position to the current position
			_startLatLong = _map.CenterLatitudeLongitude;
			_endLatlong = latlon;
			_startPosition = _map.GeoToWorldPosition(_startLatLong, false);
			_endPosition = _map.GeoToWorldPosition(_endLatlong, false);
		}

		//We do the actual interpolation in FixedUpdate(), since we're dealing with a rigidbody
		void LateUpdate()
		{
			if (_isMapInitialized && _isLerping)
			{
				//We want percentage = 0.0 when Time.time = _timeStartedLerping
				//and percentage = 1.0 when Time.time = _timeStartedLerping + timeTakenDuringLerp
				//In other words, we want to know what percentage of "timeTakenDuringLerp" the value
				//"Time.time - _timeStartedLerping" is.
				float timeSinceStarted = Time.time - _timeStartedLerping;
				float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

				//Perform the actual lerping.  Notice that the first two parameters will always be the same
				//throughout a single lerp-processs (ie. they won't change until we hit the space-bar again
				//to start another lerp)
				//_startPosition = _map.GeoToWorldPosition(_map.CenterLatitudeLongitude, false);
				_startPosition = _map.GeoToWorldPosition(_startLatLong, false);
				_endPosition = _map.GeoToWorldPosition(_endLatlong, false);
				var position = Vector3.Lerp(_startPosition, _endPosition, percentageComplete);
				var latLong = _map.WorldToGeoPosition(position);
				_map.UpdateMap(latLong, _map.Zoom);

				//When we've completed the lerp, we set _isLerping to false
				if (percentageComplete >= 1.0f)
				{
					_isLerping = false;

				}
			}
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

