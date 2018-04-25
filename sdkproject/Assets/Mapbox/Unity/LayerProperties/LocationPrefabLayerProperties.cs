using UnityEngine;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using System;

[Serializable]
public class LocationPrefabsLayerProperties : LayerProperties
{
	//we want the vector tile factory is iterating though this list as 
	//ISubLayerProperties, not the concrete properties
	public List<ISubLayerProperties> locationPrefabList = new List<ISubLayerProperties>();
}