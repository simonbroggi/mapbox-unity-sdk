namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Map;

	/// <summary>
	///  Generates and filters ArNodes for ARLocationManager.
	/// </summary>
	public class ArNodesSync : MonoBehaviour
	{

		[SerializeField]
		Transform _targetTransform;

		[SerializeField]
		AbstractMap _map;

		IEnumerator _saveARnodes;

		//Average best here... The median scale of accuracy..
		//Only add the best points if a point drops below mediuan remove it from the list..
		void Start()
		{
			//TODO : This needs to have InitializdedARMode notifier.
			//That is notified on ARPlanePlacement....

			//_map.Initialize();
		}

		void Update()
		{

		}

		IEnumerator saveArNodes()
		{
			while (true)
			{
				yield return WaitForSeconds(60);
			}
		}
	}
}

