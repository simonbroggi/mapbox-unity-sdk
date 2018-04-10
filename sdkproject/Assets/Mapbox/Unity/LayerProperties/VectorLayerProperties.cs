namespace Mapbox.Unity.Map
{
	using System;
	using System.Collections.Generic;
	using Mapbox.Unity.Utilities;
	using UnityEngine;

	[Serializable]
	public class VectorLayerProperties : LayerProperties
	{
		private bool _isActive;
		public bool isActive
		{
			get
			{
				if (vectorSubLayers.Count > 0)
				{
					return true;
				}
				return false;
			}
			set
			{
				if (!value)
					vectorSubLayers.Clear();
				else
				{
					Debug.LogWarning("To turn on vector layer just add visualizers. It will turn on automatically");
				}
			}
		}

		[Tooltip("Use Mapbox style-optimized tilesets, remove any layers or features in the tile that are not represented by a Mapbox style. Style-optimized vector tiles are smaller, served over-the-wire, and a great way to reduce the size of offline caches.")]
		public bool useOptimizedStyle = false;
		[StyleSearch]
		public Style optimizedStyle;
		public LayerPerformanceOptions performanceOptions;
		[NodeEditorElementAttribute("Vector Sublayers")]
		public List<VectorSubLayerProperties> vectorSubLayers = new List<VectorSubLayerProperties>();

		public string GetConcatenatedMapId()
		{
			var combinedString = "";
			foreach (var layer in vectorSubLayers)
			{
				var mapId = layer.coreOptions.sourceOptions.Id;
				if (combinedString == "" || combinedString == null)
				{
					combinedString = mapId;
					continue;
				}
				if(!combinedString.Contains(mapId))
					combinedString += "," + mapId;
			}
			return combinedString;
		}

		public void SetSameSourceForAllVisualizers(string vectorSource)
		{
			foreach (var layer in vectorSubLayers)
			{
				layer.coreOptions.sourceType = VectorSourceType.Custom;
				layer.coreOptions.sourceOptions.Id = vectorSource;
			}
		}
	}
}
