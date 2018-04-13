namespace Mapbox.Unity.Ar
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.MapMatching;
	using Mapbox.Utils;

	public class ARMapMatching : MonoBehaviour
	{
		[SerializeField]
		MapMatching.Profile _profile;

		public Action<Vector2d[]> ReturnPoints;
		IEnumerator _mapMatching;
		WaitForSeconds _waitFor;

		public void Response()
		{
			var response = new Vector2d[0];
		}
	}

}