using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;

public class SetARPlayerPos : MonoBehaviour
{
	[SerializeField]
	Transform _arRoot;

	[SerializeField]
	Transform _playerCamera;

	[SerializeField]
	ARController _arController;

	public void SetArPos(Vector3 pos, Quaternion rotation)
	{
		_playerCamera.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		_arRoot.SetPositionAndRotation(pos, rotation);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			_arRoot.eulerAngles = new Vector3(0, 0, 0);
			_arRoot.RotateAround(_playerCamera.transform.position, Vector3.up, 30f);
			//SetArPos(new Vector3(2, 1, 3), Quaternion.identity);
		}
	}
}
