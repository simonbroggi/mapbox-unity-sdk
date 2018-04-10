using UnityEngine;
using System.Collections;
using Mapbox.Unity.Map;

public class LocationPrefabsLayerProperties : LayerProperties
{
	public LayerSourceOptions sourceOptions = new LayerSourceOptions()
	{
		layerSource = new Style()
		{
			Id = "mapbox.mapbox-streets-v7"
		},
		isActive = true
	};
	public GameObject prefab;
}
