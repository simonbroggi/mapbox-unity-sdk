using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSphereCast : MonoBehaviour
{
	[SerializeField]
	Transform _playerTransform;
	float _previousDistance, _sphereRadius = 200;

	void Start()
	{

	}

	void Update()
	{

		Ray ray = new Ray();
		ray.origin = _playerTransform.position;
		float inverseResolution = 10f;
		Vector3 direction = Vector3.right;
		int steps = Mathf.FloorToInt(360f / inverseResolution);
		Quaternion xRotation = Quaternion.Euler(Vector3.right * inverseResolution);
		Quaternion yRotation = Quaternion.Euler(Vector3.up * inverseResolution);
		Quaternion zRotation = Quaternion.Euler(Vector3.forward * inverseResolution);
		for (int x = 0; x < steps / 2; x++)
		{
			direction = zRotation * direction;
			for (int y = 0; y < steps; y++)
			{
				direction = xRotation * direction;
				Debug.DrawLine(ray.origin, ray.origin + direction); // for science
				RaycastHit hit;
				if (Physics.Raycast(ray.origin, ray.origin + direction, out hit, 20, 0))
				{
					Debug.Log(hit.collider.gameObject.name);
				}
			}
		}
	}

	private void SphereCast()
	{
		RaycastHit hit;

		if (Physics.SphereCast(_playerTransform.position, _sphereRadius, _playerTransform.up, out hit, 1, 0, QueryTriggerInteraction.Ignore))
		{
			Debug.Log(hit.collider.gameObject.name);
			if (hit.collider.gameObject.tag == "road")
			{
				Debug.Log("hit a road");
			}
		}
	}
}
