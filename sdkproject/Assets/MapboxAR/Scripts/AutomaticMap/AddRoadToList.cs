using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRoadToList : MonoBehaviour
{
	private void Start()
	{
		SnapToRoad.Instance.Roads.Add(gameObject);
	}
}
