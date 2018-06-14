namespace Mapbox.Unity.MeshGeneration.Modifiers
{
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.MeshGeneration.Data;
	using System;
	using Mapbox.Unity.Map;
	using Mapbox.Utils;
	using Mapbox.Unity.Utilities;
	using Mapbox.VectorTile.Geometry;
	using Mapbox.Unity.MeshGeneration.Interfaces;
	using KDTree;

	/// <summary>
	/// ReplaceBuildingFeatureModifier takes in POIs and checks if the feature layer has those points and deletes them
	/// </summary>
	[CreateAssetMenu(menuName = "Mapbox/Modifiers/Replace Feature Modifier")]
	public class ReplaceFeatureModifier : GameObjectModifier, IReplacementCriteria
	{
		private List<string> _latLonToSpawn;

		private Dictionary<GameObject, GameObject> _objects;
		[SerializeField]
		private SpawnPrefabOptions _options;
		private List<GameObject> _prefabList = new List<GameObject>();
		private KDTree.KDTree<VectorFeatureUnity> _injectedFeatures = new KDTree.KDTree<VectorFeatureUnity>(2);

		[NonSerialized]
		Dictionary<UnityTile, KDTree<VectorFeatureUnity>> replacementFeaturesDictionary = new Dictionary<UnityTile, KDTree<VectorFeatureUnity>>();
		[SerializeField]
		[Geocode]
		private List<string> _prefabLocations;

		[SerializeField]
		private List<string> _explicitlyBlockedFeatureIds;
		/// <summary>
		/// List of featureIds to test against. 
		/// We need a list of featureIds per location. 
		/// A list is required since buildings on tile boundary will have multiple id's for the same feature.
		/// </summary>
		private HashSet<string> overlappingFeatureIdSchema = new HashSet<string>();
		private HashSet<string> confirmedOverlappingFeatures = new HashSet<string>();

		[NonSerialized]
		List<UnityTile> tempTileList = new List<UnityTile>() ;
		private string _tempFeatureId;

		public override void Initialize()
		{
			base.Initialize();
			//duplicate the list of lat/lons to track which coordinates have already been spawned
			_latLonToSpawn = new List<string>(_prefabLocations);
			if (_objects == null)
			{
				_objects = new Dictionary<GameObject, GameObject>();
			}
			_latLonToSpawn = new List<string>(_prefabLocations);
		}

		public override void SetProperties(ModifierProperties properties)
		{
			_options = (SpawnPrefabOptions)properties;
		}

		public override void FeaturePreProcess(VectorFeatureUnity feature, UnityTile tile, KDTree<VectorFeatureUnity> replacementFeatures)
		{
			int index = -1;
			foreach (var point in _prefabLocations)
			{
				try
				{
					index++;
					var coord = Conversions.StringToLatLon(point);
					if (feature.ContainsLatLon(coord) && (feature.Data.Id != 0))
					{
						_tempFeatureId = feature.Data.Id.ToString();
						confirmedOverlappingFeatures.Add(_tempFeatureId);

						_tempFeatureId = _tempFeatureId.Substring(0, _tempFeatureId.Length - 3);

						if (!overlappingFeatureIdSchema.Contains(_tempFeatureId))
						{
							overlappingFeatureIdSchema.Add(_tempFeatureId);
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}

			//Meta-data driven replacement
			if(replacementFeatures == null || replacementFeatures.Size==0)
			{
				return;
			}

			if (!replacementFeaturesDictionary.ContainsKey(tile))
			{
				replacementFeaturesDictionary.Add(tile, replacementFeatures); //just to make sure this executes only once per tile
			}

			var centroidVector = GetFeatureCentroid(feature);
			var neighboringFeatures = replacementFeatures.NearestNeighbors(new double[] { centroidVector.x, centroidVector.z }, 11, 100);

			while(neighboringFeatures.MoveNext())
			{
				var featureItem = neighboringFeatures.Current;

				var point = featureItem.Data.Geometry<float>()[0][0];
				if (feature.ContainsTileSpacePoint(point) && (feature.Data.Id != 0))
				{
					_tempFeatureId = feature.Data.Id.ToString();

					confirmedOverlappingFeatures.Add(_tempFeatureId);
					_tempFeatureId = _tempFeatureId.Substring(0, _tempFeatureId.Length - 3);
					if (!overlappingFeatureIdSchema.Contains(_tempFeatureId))
					{
						overlappingFeatureIdSchema.Add(_tempFeatureId);
					}

				}
			}

		}

		private Vector3 GetFeatureCentroid(VectorFeatureUnity feature)
		{
			var centroidVector = new Vector3();
			foreach (var point in feature.Points[0])
			{
				centroidVector += point;
			}
			centroidVector = centroidVector / feature.Points[0].Count;

			return centroidVector;
		}

		/// <summary>
		/// Check the feature against the list of lat/lons in the modifier
		/// </summary>
		/// <returns><c>true</c>, if the feature overlaps with a lat/lon in the modifier <c>false</c> otherwise.</returns>
		/// <param name="feature">Feature.</param>
		public bool ShouldReplaceFeature(VectorFeatureUnity feature, UnityTile tile)
		{
			int index = -1;
			int count = _prefabLocations.Count + replacementFeaturesDictionary[tile].Size;
			var featureId = feature.Data.Id.ToString();
			featureId = featureId.Substring(0, featureId.Length - 3);
			while(count>0)
			{
				count--;
				try
				{
					index++;
					//preventing spawning of explicitly blocked features
					foreach (var blockedId in _explicitlyBlockedFeatureIds)
					{
						if (feature.Data.Id.ToString() == blockedId)
						{
							return true;
						}
					}

					if (overlappingFeatureIdSchema.Count > 0)
					{
						if (overlappingFeatureIdSchema.Contains(featureId))
						{
							return true;
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
			return false;
		}

		public override void Run(VectorEntity ve, UnityTile tile)
		{
			//replace the feature only once per lat/lon
			if (ShouldSpawnFeature(ve.Feature, tile))
			{
				SpawnPrefab(ve, tile);
			}
		}

		private void SpawnPrefab(VectorEntity ve, UnityTile tile)
		{
			GameObject go;

			if (_objects.ContainsKey(ve.GameObject))
			{
				go = _objects[ve.GameObject];
			}
			else
			{
				go = Instantiate(_options.prefab);
				_prefabList.Add(go);
				_objects.Add(ve.GameObject, go);
				go.transform.SetParent(ve.GameObject.transform, false);
			}

			PositionScaleRectTransform(ve, tile, go);

			if (_options.AllPrefabsInstatiated != null)
			{
				_options.AllPrefabsInstatiated(_prefabList);
			}
		}

		public void PositionScaleRectTransform(VectorEntity ve, UnityTile tile, GameObject go)
		{
			RectTransform goRectTransform;
			IFeaturePropertySettable settable = null;
			var centroidVector = GetFeatureCentroid(ve.Feature);

			go.name = ve.Feature.Data.Id.ToString();

			goRectTransform = go.GetComponent<RectTransform>();
			if (goRectTransform == null)
			{
				go.transform.localPosition = centroidVector;
			}
			else
			{
				goRectTransform.anchoredPosition3D = centroidVector;
			}
			//go.transform.localScale = Constants.Math.Vector3One;

			settable = go.GetComponent<IFeaturePropertySettable>();
			if (settable != null)
			{
				settable.Set(ve.Feature.Properties);
			}

			if (_options.scaleDownWithWorld)
			{
				go.transform.localScale = (go.transform.localScale * (tile.TileScale));
			}
		}

		/// <summary>
		/// Checks if the feature should be used to spawn a prefab, once per lat/lon
		/// </summary>
		/// <returns><c>true</c>, if the feature should be spawned <c>false</c> otherwise.</returns>
		/// <param name="feature">Feature.</param>
		private bool ShouldSpawnFeature(VectorFeatureUnity feature, UnityTile tile)
		{
			if (feature == null)
			{
				return false;
			}

			var featureId = feature.Data.Id.ToString();
			if(confirmedOverlappingFeatures.Contains(featureId))
			{
				return true;
			}

			return false;
		}
	}
}
