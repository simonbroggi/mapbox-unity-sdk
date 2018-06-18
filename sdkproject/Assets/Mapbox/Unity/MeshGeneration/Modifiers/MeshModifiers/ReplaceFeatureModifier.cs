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

	/// <summary>
	/// ReplaceBuildingFeatureModifier takes in POIs and checks if the feature layer has those points and deletes them
	/// </summary>
	[CreateAssetMenu(menuName = "Mapbox/Modifiers/Replace Feature Modifier")]
	public class ReplaceFeatureModifier : GameObjectModifier, IReplacementCriteria
	{
		//object pool
		private Dictionary<ulong, GameObject> _objects;
		private GameObject _poolGameObject;

		//spawn settings
		[SerializeField]
		private SpawnPrefabOptions _options;
		private List<GameObject> _prefabList = new List<GameObject>();
		private List<string> _latLonToSpawn;

		[SerializeField]
		[Geocode]
		private List<string> _prefabLocations;

		//maximum distance to trigger feature replacement ( in tile space ), 
		// !! this changes depending on the latitude of the replacement
		[SerializeField]
		private const float _replacementRange = 500f;

		public override void Initialize()
		{
			base.Initialize();

			//duplicate the list of lat/lons to track which coordinates have already been spawned
			_latLonToSpawn = new List<string>(_prefabLocations);

			//object pool
			if (_objects == null)
			{
				_objects = new Dictionary<ulong, GameObject>();
				_poolGameObject = new GameObject("_inactive_prefabs_pool");
			}
		}

		public override void SetProperties(ModifierProperties properties)
		{
			_options = (SpawnPrefabOptions)properties;
		}

		/// <summary>
		/// Check the feature against the list of lat/lons in the modifier
		/// </summary>
		/// <returns><c>true</c>, if the feature overlaps with a lat/lon in the modifier <c>false</c> otherwise.</returns>
		/// <param name="feature">Feature.</param>
		public bool ShouldReplaceFeature(VectorFeatureUnity feature)
		{
			for (int i = 0; i < _prefabLocations.Count; i++)
			{
				var point = _prefabLocations[i];

				try
				{
					var from = Conversions.LatitudeLongitudeToVectorTilePosition(Conversions.StringToLatLon(point), feature.Tile.InitialZoom);
					var to = feature.Data.Geometry<float>()[0][0];
					if (Vector2.SqrMagnitude(new Vector2(from.x, from.y) - new Vector2(to.X, to.Y)) < Math.Pow(_replacementRange, 2f))
					{
						return true;
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}

			}
			return false;
		}

		/// <summary>
		/// Checks if the feature should be used to spawn a prefab, once per lat/lon
		/// </summary>
		/// <returns><c>true</c>, if the feature should be spawned <c>false</c> otherwise.</returns>
		/// <param name="feature">Feature.</param>
		private bool ShouldSpawnFeature(VectorFeatureUnity feature)
		{
			//don't spawn if the feature isn't valid
			if (feature == null)
			{
				return false;
			}

			//spawn the feature if it's already in the object pool
			if (_objects.ContainsKey(feature.Data.Id))
			{
				return true;
			}

			foreach (var point in _latLonToSpawn)
			{
				var coord = Conversions.StringToLatLon(point);
				if (feature.ContainsLatLon(coord))
				{
					_latLonToSpawn.Remove(point);
					return true;
				}
			}

			return false;
		}

		public override void Run(VectorEntity ve, UnityTile tile)
		{
			//replace the feature only once per lat/lon
			if (ShouldSpawnFeature(ve.Feature))
			{
				SpawnPrefab(ve, tile);
			}
		}

		private void SpawnPrefab(VectorEntity ve, UnityTile tile)
		{
			GameObject go;

			var featureId = ve.Feature.Data.Id;
			if (_objects.ContainsKey(featureId))
			{
				go = _objects[featureId];
				go.SetActive(true);
				go.transform.SetParent(ve.GameObject.transform, false);
			}
			else
			{
				go = Instantiate(_options.prefab);
				_prefabList.Add(go);
				_objects.Add(featureId, go);
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
			go.transform.localScale = _options.prefab.transform.localScale;
			RectTransform goRectTransform;
			IFeaturePropertySettable settable = null;
			var centroidVector = new Vector3();
			foreach (var point in ve.Feature.Points[0])
			{
				centroidVector += point;
			}
			centroidVector = centroidVector / ve.Feature.Points[0].Count;

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

		public override void OnPoolItem(VectorEntity vectorEntity)
		{
			base.OnPoolItem(vectorEntity);
			var featureId = vectorEntity.Feature.Data.Id;

			if (!_objects.ContainsKey(featureId))
			{
				return;
			}

			var go = _objects[featureId];
			if(go == null || _poolGameObject == null)
			{
				return;
			}

			go.SetActive(false);
			go.transform.SetParent(_poolGameObject.transform, false);
		}
	}
}
