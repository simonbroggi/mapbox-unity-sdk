namespace Mapbox.Unity.MeshGeneration.Data
{
	using Mapbox.VectorTile;
	using System.Collections.Generic;
	using Mapbox.VectorTile.Geometry;
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Utilities;
	using Mapbox.Unity.Map;
	using Mapbox.Map;

	public class VectorFeatureUnity
	{
		public VectorTileFeature Data;
		public Dictionary<string, object> Properties;
		public List<List<Vector3>> Points = new List<List<Vector3>>();
		public UnityTile Tile;

		private double _rectSizex;
		private double _rectSizey;
		private int _geomCount;
		private int _pointCount;
		private List<Vector3> _newPoints = new List<Vector3>();
		private List<List<Point2d<float>>> _geom;

		private bool randomValueAssigned;
		private float randomValue;

		private string latLon;

		public float RandomValue
		{
			get
			{
				return 0;

				if (!randomValueAssigned)
				{
					GeoRandom geoRandom = Object.FindObjectOfType<GeoRandom>();
					AbstractMap abstractMap = Object.FindObjectOfType<AbstractMap>();

					//WorldToGeoPosition
					if (geoRandom == null || abstractMap == null)
					{
						Debug.LogWarning("Warning: No GeoRandom component found in scene");
						return 0f;
					}

					//get first index of geometry in 4096 tile space...
					float x = _geom[0][0].X;
					float y = _geom[0][0].Y;

					float maxX = 0f;
					//float maxZ;

					var centroidVector = new Vector3();
					foreach (var point in Points[0])
					{
						centroidVector += point;
						maxX += point.x;
					}
					//centroidVector = centroidVector / Points[0].Count;

					maxX = Mathf.Abs(maxX) / Tile.TileScale;

					Vector3 firstPoint = Points[0][0];
					Vector3 tilePos = Tile.transform.position;

					Vector2d _latLon = abstractMap.WorldToGeoPosition(centroidVector);

					Vector2 constantTileCoord = Conversions.LatitudeLongitudeToUnityTilePosition(_latLon, Tile);

					Vector2d constantTileCoordOther = Conversions.LatitudeLongitudeToUnityTilePositionVector2d(_latLon, 16, 0.1635333f);

					Vector2 LatitudeLongitudeToVectorTilePosition = Conversions.LatitudeLongitudeToVectorTilePosition(_latLon, 16);// 16);

					int xx = (int)LatitudeLongitudeToVectorTilePosition.x;
					int yy = (int)LatitudeLongitudeToVectorTilePosition.y;
					//var constantTileSpaceCoords = Conversions.LatitudeLongitudeToUnityTilePosition()

					//what else can we derive value from?
					//how can we get lat/lon and use that to grab value from 4096 texture?


					//Debug.Log("FirstPoint : " + firstPoint + ", LatLon: " + _latLon + ", xx: " + xx + ", yy: " + yy + ", Random value : " + randomValue);

					double mult = 1000.0;

					double latLonX_100 = (Mathd.Ceil(_latLon.x * mult)) / mult;
					double latLonY_100 = (Mathd.Ceil(_latLon.y * mult)) / mult;


					UnwrappedTileId myConsistentTileId = Conversions.LatitudeLongitudeToTileId(latLonX_100, latLonY_100, 30);

					

					int tileIdX = (int)Mathf.Repeat(myConsistentTileId.X, 4096);
					int tileIdY = (int)Mathf.Repeat(myConsistentTileId.Y, 4096);


					//randomValue = geoRandom.GetRandomValue(xx, yy);
					randomValue = geoRandom.GetRandomValue(tileIdX, tileIdY);


					Vector3 TilePositionFlat = new Vector3 (Tile.transform.position.x, 0f, Tile.transform.position.z);
					Vector3 CentroidFlat = new Vector3(centroidVector.x, 0f, centroidVector.z);
					float distanceFromTileCenter = Vector3.SqrMagnitude(TilePositionFlat - CentroidFlat);

					if(geoRandom.idToQuery == Data.Id.ToString())
					//if (Data.Id.ToString() == "4513369021")
					{
						Debug.Log("---------------------------------------------");
						Debug.Log(Data.Id.ToString());

						Debug.Log("MaxX : " + maxX);

						Debug.Log("Points[0].Count : " + Points[0].Count);

						/*
						Debug.Log("Centroid : " +  centroidVector / Tile.TileScale);
						Debug.Log("DISTANCE = " + distanceFromTileCenter);
						Debug.Log("LatLon : " + _latLon);
						Debug.Log("LatLon 100 = " + latLonX_100 + ", " + latLonY_100);
						Debug.Log("Texture coords - x : " + tileIdX + ", y : " + tileIdY);
						Debug.Log("Random val = " + randomValue);
						Debug.Log(myConsistentTileId.ToString());
						*/
						//Debug.Log("Zoom : " + abstractMap.Zoom);
						Debug.Log("TileScale : " + Tile.TileScale);
						//Debug.Log("FirstPoint : " + firstPoint);

						//Debug.Log("constantTileCoord : " + constantTileCoord);
						//Debug.Log("constantTileCoordOther : " + constantTileCoordOther);
						//Debug.Log("LatitudeLongitudeToVectorTilePosition : " + LatitudeLongitudeToVectorTilePosition);
						//Debug.Log("XX : " + xx);
						//Debug.Log("YY : " + yy);
						//Debug.Log("Random Value : " + randomValue);


						geoRandom.AddTileId(myConsistentTileId.ToString());

						//Debug.Log("TileScale : " + Tile.TileScale);
						//Debug.Log("x : " + x + ", y : " + y + "; randomValue : " + randomValue + "; Points : " + _newPoints[0].x + ", " + _newPoints[0].z);
						//Debug.Log("LatLon : " + _latLon);
						//Debug.Log("<color=cyan>LatLon : " + _latLon + ", xx : " + xx + ", yy : " + yy + ", random value : " + randomValue + "</color>");
					}

					randomValueAssigned = true;
				}
				return randomValue;
			}
		}

		public VectorFeatureUnity()
		{
			Points = new List<List<Vector3>>();
		}

		public VectorFeatureUnity(VectorTileFeature feature, UnityTile tile, float layerExtent, bool buildingsWithUniqueIds = false)
		{
			Data = feature;
			Properties = Data.GetProperties();
			Points.Clear();
			Tile = tile;

			//this is a temp hack until we figure out how streets ids works
			if (buildingsWithUniqueIds == true) //ids from building dataset is big ulongs 
			{
				_geom = feature.Geometry<float>(); //and we're not clipping by passing no parameters
			}
			else //streets ids, will require clipping
			{
				_geom = feature.Geometry<float>(0); //passing zero means clip at tile edge
			}

			_rectSizex = tile.Rect.Size.x;
			_rectSizey = tile.Rect.Size.y;

			_geomCount = _geom.Count;
			for (int i = 0; i < _geomCount; i++)
			{
				_pointCount = _geom[i].Count;
				_newPoints = new List<Vector3>(_pointCount);
				for (int j = 0; j < _pointCount; j++)
				{
					var point = _geom[i][j];
					_newPoints.Add(new Vector3((float)(point.X / layerExtent * _rectSizex - (_rectSizex / 2)) * tile.TileScale, 0, (float)((layerExtent - point.Y) / layerExtent * _rectSizey - (_rectSizey / 2)) * tile.TileScale));
				}
				Points.Add(_newPoints);
			}
		}

		public VectorFeatureUnity(VectorTileFeature feature, List<List<Point2d<float>>> geom, UnityTile tile, float layerExtent, bool buildingsWithUniqueIds = false, string _latLon = null)
		{
			Data = feature;
			Properties = Data.GetProperties();
			Points.Clear();
			Tile = tile;
			_geom = geom;

			_rectSizex = tile.Rect.Size.x;
			_rectSizey = tile.Rect.Size.y;

			_geomCount = _geom.Count;
			for (int i = 0; i < _geomCount; i++)
			{
				_pointCount = _geom[i].Count;
				_newPoints = new List<Vector3>(_pointCount);
				for (int j = 0; j < _pointCount; j++)
				{
					var point = _geom[i][j];
					_newPoints.Add(new Vector3((float)(point.X / layerExtent * _rectSizex - (_rectSizex / 2)) * tile.TileScale, 0, (float)((layerExtent - point.Y) / layerExtent * _rectSizey - (_rectSizey / 2)) * tile.TileScale));
				}
				Points.Add(_newPoints);
			}
		}

		public bool ContainsLatLon(Vector2d coord)
		{
			////first check tile
			var coordinateTileId = Conversions.LatitudeLongitudeToTileId(
				coord.x, coord.y, Tile.CurrentZoom);
			
			if (Points.Count > 0)
			{
				var from = Conversions.LatLonToMeters(coord.x, coord.y);

				var to = new Vector2d((Points[0][0].x / Tile.TileScale) + Tile.Rect.Center.x, (Points[0][0].z / Tile.TileScale) + Tile.Rect.Center.y);
				var dist = Vector2d.Distance(from, to);
				if (Mathd.Abs(dist) < 50)
				{
					return true;
				}
			}


			//Debug.Log("Distance -> " + dist);
			{
				if ((!coordinateTileId.Canonical.Equals(Tile.CanonicalTileId)))
				{
					return false;
				}

				//then check polygon
				var point = Conversions.LatitudeLongitudeToVectorTilePosition(coord, Tile.CurrentZoom);
				var output = PolygonUtils.PointInPolygon(new Point2d<float>(point.x, point.y), _geom);

				return output;
			}

		}

	}
}
