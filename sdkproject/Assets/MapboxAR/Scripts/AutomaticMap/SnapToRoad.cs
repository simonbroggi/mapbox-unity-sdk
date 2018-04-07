using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToRoad : MonoBehaviour
{
	public List<GameObject> Roads;
	public static SnapToRoad Instance;
	float _minDistance = 1000, _currentDistance;
	GameObject _returnable;
	Vector3 _returnPoint;

	private void Awake()
	{
		Instance = this;
		Roads = new List<GameObject>();
	}

	public GameObject ReturnClosestRoadToVector(Vector3 vector)
	{

		foreach (var gameobj in Roads)
		{
			_currentDistance = Vector3.Distance(gameobj.transform.position, vector);
			Debug.Log(_currentDistance);
			if (_currentDistance <= _minDistance)
			{
				_minDistance = _currentDistance;
				_returnable = gameobj;
			}
		}
		_minDistance = 1000;
		return _returnable;
	}

	public Vector3 ReturnSnapPoint(GameObject gameObjToGetSnap, Vector3 toSnapTo)
	{
		MeshFilter meshFilter = gameObjToGetSnap.GetComponent<MeshFilter>();
		Matrix4x4 localToWorld = transform.localToWorldMatrix;

		for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
		{
			Vector3 wrldPoint = localToWorld.MultiplyPoint3x4(meshFilter.mesh.vertices[i]);
			float distance = Vector3.Distance(wrldPoint, toSnapTo);
			if (distance <= _minDistance)
			{
				_minDistance = distance;
				_returnPoint = wrldPoint;
			}
		}

		return _returnPoint;
	}
}
