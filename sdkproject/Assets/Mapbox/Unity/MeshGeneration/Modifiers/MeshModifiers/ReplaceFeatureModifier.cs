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

		private List<Vector2d> _latLonToSpawn;

		private Dictionary<string, GameObject> _objects;
		private Dictionary<string, Vector2d> _objectPosition;
		private GameObject _poolGameObject;
		[SerializeField]
		private SpawnPrefabOptions _options;
		private List<GameObject> _prefabList = new List<GameObject>();

		[SerializeField]
		[Geocode]
		private List<string> _prefabLocations;

		[SerializeField]
		private List<string> _explicitlyBlockedFeatureIds;
		//maximum distance to trigger feature replacement ( in tile space )
		private const float _maxDistanceToBlockFeature_tilespace = 1000f;

		/// <summary>
		/// List of featureIds to test against. 
		/// We need a list of featureIds per location. 
		/// A list is required since buildings on tile boundary will have multiple id's for the same feature.
		/// </summary>
		private List<List<string>> _featureId;
		private string _tempFeatureId;

		private string parentIdToBlock;
		private bool parentIdObtained;

		private List<string> partIdsToBlock = new List<string>();

		private bool debugged;

		public SpawnPrefabOptions SpawnPrefabOptions
		{
			set
			{
				_options = value;
			}
		}

		public List<string> PrefabLocations
		{
			set
			{
				_prefabLocations = value;
			}
		}

		public List<string> BlockedIds
		{
			set
			{
				_explicitlyBlockedFeatureIds = value;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			//duplicate the list of lat/lons to track which coordinates have already been spawned
			Debug.Log("Initialize");
			parentIdToBlock = "";
			parentIdObtained = false;
			_featureId = new List<List<string>>();

			for (int i = 0; i < _prefabLocations.Count; i++)
			{
				_featureId.Add(new List<string>());
			}
			if (_objects == null)
			{
				_objects = new Dictionary<string, GameObject>();
				_objectPosition = new Dictionary<string, Vector2d>();
				_poolGameObject = new GameObject("_inactive_prefabs_pool");
			}
			_latLonToSpawn = new List<Vector2d>();
			foreach (var loc in _prefabLocations)
			{
				_latLonToSpawn.Add(Conversions.StringToLatLon(loc));
			}
		}

		public override void SetProperties(ModifierProperties properties)
		{
			Debug.Log("SetProperties");
			_options = (SpawnPrefabOptions)properties;
		}

		public override void FeaturePreProcess(VectorFeatureUnity feature)
		{
			int index = -1;
			foreach (var point in _prefabLocations)
			{
				try
				{
					index++;
					var coord = Conversions.StringToLatLon(point);
					string idString = feature.Properties["id"].ToString();
					//if this feature contains the lat lon of the desired spawn location...
					if (feature.ContainsLatLon(coord))// && (idString != "0"))
					{

						object parent = "";
						//if I have a parent attribute, cache this...
						if (feature.Properties.TryGetValue("parent", out parent))
						{
							if (!string.IsNullOrEmpty(parent.ToString()))
							{
								SetParentIdToBlock(parent.ToString());
							}
						}
						//if I do not have a parent id, then I am the parent, and set the parent id to my id...
						else
						{
							SetParentIdToBlock(idString);
						}

					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}

			}
		}

		private void SetParentIdToBlock(string id)
		{
			if(parentIdObtained == false)
			{
				parentIdToBlock = id;
				Debug.Log("SetParentIdToBlock : " + parentIdToBlock);
				parentIdObtained = true;
			}
		}

		/// <summary>
		/// Check the feature against the list of lat/lons in the modifier
		/// </summary>
		/// <returns><c>true</c>, if the feature overlaps with a lat/lon in the modifier <c>false</c> otherwise.</returns>
		/// <param name="feature">Feature.</param>
		public bool ShouldReplaceFeature(VectorFeatureUnity feature)
		{
			int index = -1;
			string featureId = feature.Properties["id"].ToString();
			//preventing spawning of explicitly blocked features
			foreach (var blockedId in _explicitlyBlockedFeatureIds)
			{
				if (featureId == blockedId)
				{
					return true;
				}
			}
			if(featureId == parentIdToBlock)
			{
				return true;
			}
			object parent = "";
			if (feature.Properties.TryGetValue("parent", out parent))
			{
				if (parent.ToString() == parentIdToBlock)
				{
					return true;
				}
			}

			foreach (var point in _prefabLocations)
			{
				try
				{
					index++;
					if (_featureId[index] != null)
					{
						foreach (var idString in _featureId[index])
						{
							var latlngVector = Conversions.StringToLatLon(point);
							var from = Conversions.LatLonToMeters(latlngVector.x, latlngVector.y);
							var to = new Vector2d((feature.Points[0][0].x / feature.Tile.TileScale) + feature.Tile.Rect.Center.x, (feature.Points[0][0].z / feature.Tile.TileScale) + feature.Tile.Rect.Center.y);
							var dist = Vector2d.Distance(from, to);
							if (dist > 500)
							{
								return false;
							}
							if (idString.StartsWith(featureId, StringComparison.CurrentCulture))
							{
								return true;
							}
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
			Vector2d latLong = Vector2d.zero;
			if (ShouldSpawnFeature(ve.Feature, out latLong))
			{
				SpawnPrefab(ve, tile, latLong);
			}
		}

		private void SpawnPrefab(VectorEntity ve, UnityTile tile, Vector2d latLong)
		{
			GameObject go;

			string featureId = ve.Feature.Properties["id"].ToString();
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
				_objectPosition.Add(featureId, latLong);
				go.transform.SetParent(ve.GameObject.transform, false);
			}

			PositionScaleRectTransform(ve, tile, go, latLong);

			if (_options.AllPrefabsInstatiated != null)
			{
				_options.AllPrefabsInstatiated(_prefabList);
			}
		}

		public void PositionScaleRectTransform(VectorEntity ve, UnityTile tile, GameObject go, Vector2d latLong)
		{
			go.transform.localScale = _options.prefab.transform.localScale;
			RectTransform goRectTransform;
			IFeaturePropertySettable settable = null;
			var latLongPosition = new Vector3();
			var centroidVector = new Vector3();
			foreach (var point in ve.Feature.Points[0])
			{
				centroidVector += point;
			}
			centroidVector = centroidVector / ve.Feature.Points[0].Count;

			latLongPosition = Conversions.LatitudeLongitudeToUnityTilePosition(latLong, tile.CurrentZoom, tile.TileScale, 4096).ToVector3xz();
			latLongPosition.y = centroidVector.y;

			go.name = ve.Feature.Properties["id"].ToString();

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
		private bool ShouldSpawnFeature(VectorFeatureUnity feature, out Vector2d latLong)
		{
			latLong = Vector2d.zero;
			if (feature == null)
			{
				return false;
			}

			if (_objects.ContainsKey(feature.Properties["id"].ToString()))
			{
				_objectPosition.TryGetValue(feature.Properties["id"].ToString(), out latLong);
				_latLonToSpawn.Remove(latLong);
				return true;
			}

			foreach (var point in _latLonToSpawn)
			{
				if (feature.ContainsLatLon(point))
				{
					_latLonToSpawn.Remove(point);
					latLong = point;
					return true;
				}
			}

			return false;
		}

		public override void OnPoolItem(VectorEntity vectorEntity)
		{
			base.OnPoolItem(vectorEntity);
			string featureId = vectorEntity.Feature.Properties["id"].ToString();

			if (!_objects.ContainsKey(featureId))
			{
				return;
			}

			var go = _objects[featureId];
			if (go == null || _poolGameObject == null)
			{
				return;
			}

			go.SetActive(false);
			go.transform.SetParent(_poolGameObject.transform, false);
		}

		public override void ClearCaches()
		{
			foreach (var gameObject in _objects.Values)
			{
				Destroy(gameObject);
			}
			_objects.Clear();
			_objectPosition.Clear();
			Destroy(_poolGameObject);
		}
	}
}
