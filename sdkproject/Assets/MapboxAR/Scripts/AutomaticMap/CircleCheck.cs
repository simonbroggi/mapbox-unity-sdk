using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCheck : MonoBehaviour
{

	private void Update()
	{
		Ray ray = new Ray();
		ray.origin = transform.position;
		Vector3 direction = Vector3.right;
		Quaternion yRotation = Quaternion.Euler(Vector3.up);

		for (int i = 0; i < 180; i++)
		{
			var pos = RandomCircle(transform.position, 20, i + 2);
			var dir = transform.position - pos;
			Debug.DrawRay(ray.origin, dir, Color.red, Mathf.Infinity);
		}
	}

	Vector3 RandomCircle(Vector3 center, float radius, int a)
	{
		float ang = a;
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
		pos.y = center.y;
		pos.z = center.z + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		return pos;
	}
}
