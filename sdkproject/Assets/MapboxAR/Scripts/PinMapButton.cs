namespace Mapbox.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class PinMapButton : MonoBehaviour
	{
		[SerializeField]
		Button _button;

		[SerializeField]
		ManualTouchCamera _touchCam;

		[SerializeField]
		CameraMovement _touchOnPinMap;

		bool _toggle;

		void Start()
		{
			_toggle = true;
			_button.onClick.AddListener(ToggleMadness);
		}

		void ToggleMadness()
		{
			_toggle = _toggle == true ? false : true;

			_touchCam.enabled = _toggle;

			if (_toggle)
			{
				_touchOnPinMap.enabled = false;
			}
			else
			{
				_touchOnPinMap.enabled = true;
			}
		}
	}
}
