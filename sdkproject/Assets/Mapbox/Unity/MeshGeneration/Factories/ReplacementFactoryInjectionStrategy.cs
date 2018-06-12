using System;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Filters;
using UnityEngine;
namespace Mapbox.Unity.MeshGeneration.Factories
{
	[CreateAssetMenu(menuName = "Mapbox/Factory Injection/Replacement Factory Injection")]
	public class ReplacementFactoryInjectionStrategy : ScriptableObject, IFactoryInjectionStrategy
	{
		public AbstractTileFactory CreateFactory()
		{
			//setting up layer properties through code
			//Setting vector layer properties
			VectorLayerProperties _layerProperty = new VectorLayerProperties();

			_layerProperty.sourceType = VectorSourceType.MapboxStreetsWithBuildingIds;

			//Setting vector sublayer properties
			var sublayerProps = new VectorSubLayerProperties();
			sublayerProps.coreOptions = new CoreVectorLayerProperties
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
			sublayerProps.filterOptions = new VectorFilterOptions
			{
				filters = filters,
				combinerType = LayerFilterCombinerOperationType.Any
			};

			_layerProperty.vectorSubLayers.Add(sublayerProps);

			VectorTileFactory _vectorTileFactory = ScriptableObject.CreateInstance<VectorTileFactory>();
			_vectorTileFactory.SetOptions(_layerProperty);
			return _vectorTileFactory;
		}
	}
}
