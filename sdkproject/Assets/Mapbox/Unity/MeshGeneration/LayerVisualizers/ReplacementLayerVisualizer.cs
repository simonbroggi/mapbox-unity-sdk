using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Filters;
using Mapbox.VectorTile;
using System.Collections;
using System.Collections.Generic;
using System;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.MeshGeneration.Modifiers;
using System.Linq;
using KDTree;
using UnityEngine;

namespace Mapbox.Unity.MeshGeneration.Interfaces
{
	public class ReplacementLayerVisualizer : VectorLayerVisualizer
	{
		public event Action<UnityTile, KDTree<VectorFeatureUnity>> OnReplacementTileFeaturesReady = delegate { };

		public void SetProperties(ReplacementSubLayerProperties properties, LayerPerformanceOptions performanceOptions)
		{
			SubLayerProperties = properties;
		}

		public override void Create(VectorTileLayer layer, UnityTile tile, Action callback, KDTree<VectorFeatureUnity> replacementFeatures = null)
		{
			//base.Create(layer, tile, callback);
			if (!_activeCoroutines.ContainsKey(tile))
				_activeCoroutines.Add(tile, new List<int>());
			_activeCoroutines[tile].Add(Runnable.Run(ProcessLayer(layer, tile, callback)));
		}

		private IEnumerator ProcessLayer(VectorTileLayer layer, UnityTile tile, Action callback = null)
		{
			//HACK to prevent request finishing on same frame which breaks modules started/finished events
			yield return null;

			if (tile == null)
			{
				yield break;
			}

			VectorLayerVisualizerProperties tempLayerProperties = new VectorLayerVisualizerProperties();
			tempLayerProperties.vectorTileLayer = layer;
			tempLayerProperties.featureProcessingStage = FeatureProcessingStage.PreProcess;

			//Get all filters in the array.
			tempLayerProperties.layerFeatureFilters = base.SubLayerProperties.filterOptions.filters.Select(m => m.GetFilterComparer()).ToArray();

			// Pass them to the combiner
			tempLayerProperties.layerFeatureFilterCombiner = new Filters.LayerFilterComparer();
			switch (base.SubLayerProperties.filterOptions.combinerType)
			{
				case Filters.LayerFilterCombinerOperationType.Any:
					tempLayerProperties.layerFeatureFilterCombiner = Filters.LayerFilterComparer.AnyOf(tempLayerProperties.layerFeatureFilters);
					break;
				case Filters.LayerFilterCombinerOperationType.All:
					tempLayerProperties.layerFeatureFilterCombiner = Filters.LayerFilterComparer.AllOf(tempLayerProperties.layerFeatureFilters);
					break;
				case Filters.LayerFilterCombinerOperationType.None:
					tempLayerProperties.layerFeatureFilterCombiner = Filters.LayerFilterComparer.NoneOf(tempLayerProperties.layerFeatureFilters);
					break;
				default:
					break;
			}

			tempLayerProperties.buildingsWithUniqueIds = (base.SubLayerProperties.honorBuildingIdSetting) && base.SubLayerProperties.buildingsWithUniqueIds;

			#region PreProcess & Process. 

			var featureCount = tempLayerProperties.vectorTileLayer.FeatureCount();
			KDTree<VectorFeatureUnity> _injectedFeaturesTree = new KDTree<VectorFeatureUnity>(2);
			do
			{
				for (int i = 0; i < featureCount; i++)
				{
					var feature = GetFeatureinTileAtIndex(i, tile, tempLayerProperties);

					if (feature.Points != null && feature.Points.Count > 0 && feature.Points[0] != null && feature.Points[0].Count > 0)
					{
						var point = feature.Points[0][0];
						_injectedFeaturesTree.AddPoint(new double[] { point.x, point.z }, feature);
					}
					else
					{
						Debug.Log(feature);
					}

					if (IsCoroutineBucketFull)
					{
						//Reset bucket..
						_entityInCurrentCoroutine = 0;
						yield return null;
					}
				}

				// move processing to next stage. 
				tempLayerProperties.featureProcessingStage++;
			} while (tempLayerProperties.featureProcessingStage == FeatureProcessingStage.PreProcess
			|| tempLayerProperties.featureProcessingStage == FeatureProcessingStage.Process);

			if (OnReplacementTileFeaturesReady != null)
			{
				OnReplacementTileFeaturesReady(tile,_injectedFeaturesTree);
			}
			#endregion

			#region PostProcess
			// TODO : Clean this up to follow the same pattern. 
			var mergedStack = _defaultStack as MergedModifierStack;
			if (mergedStack != null && tile != null)
			{
				mergedStack.End(tile, tile.gameObject, layer.Name);
			}
			#endregion

			if (callback != null)
				callback();
		}

		/// <summary>
		/// Function to fetch feature in vector tile at the index specified. 
		/// </summary>
		/// <returns>The feature in tile at the index requested.</returns>
		/// <param name="tile">Unity Tile containing the feature.</param>
		/// <param name="index">Index of the vector feature being requested.</param>
		private VectorFeatureUnity GetFeatureinTileAtIndex(int index, UnityTile tile, VectorLayerVisualizerProperties layerProperties)
		{
			return new VectorFeatureUnity(layerProperties.vectorTileLayer.GetFeature(index),
													 tile,
										  layerProperties.vectorTileLayer.Extent,
										  layerProperties.buildingsWithUniqueIds);
		}

		/// <summary>
		/// Gets a value indicating whether this entity per coroutine bucket is full.
		/// </summary>
		/// <value><c>true</c> if coroutine bucket is full; otherwise, <c>false</c>.</value>
		private bool IsCoroutineBucketFull
		{
			get
			{
				return (_performanceOptions != null && _performanceOptions.isEnabled && _entityInCurrentCoroutine >= _performanceOptions.entityPerCoroutine);
			}
		}
	}
}
