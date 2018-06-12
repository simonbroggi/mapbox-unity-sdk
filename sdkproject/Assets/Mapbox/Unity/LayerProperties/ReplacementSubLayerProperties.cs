using System;
using System.Collections.Generic;
using Mapbox.Unity.MeshGeneration.Filters;

namespace Mapbox.Unity.Map
{
	[Serializable]
	public class ReplacementSubLayerProperties : VectorSubLayerProperties
	{
		public ReplacementSubLayerProperties()
		{
			replaceFeaturesInLayer = "building";
			coreOptions = new CoreVectorLayerProperties
			{
				isActive = true,
				geometryType = VectorPrimitiveType.Point,
				groupFeatures = false,
				layerName = "poi_label",
				snapToTerrain = true,
				sublayerName = "POIReplacementLayer"
			};

			//Filters
			LayerFilter layerFilter = new LayerFilter(LayerFilterOperationType.Contains)
			{
				Key = "type",
				PropertyValue = "restaurant"
			};
			List<LayerFilter> filters = new List<LayerFilter>();

			filters.Add(layerFilter);
			filterOptions = new VectorFilterOptions
			{
				filters = filters,
				combinerType = LayerFilterCombinerOperationType.Any
			};
		}
	}
}
