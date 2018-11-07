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
				if (!randomValueAssigned)
				{
					GeoRandom geoRandom = Object.FindObjectOfType<GeoRandom>();
					AbstractMap abstractMap = Object.FindObjectOfType<AbstractMap>();

					if (geoRandom == null || abstractMap == null)
					{
						Debug.LogWarning("Warning: No GeoRandom component found in scene");
						return 0f;
					}

					//get first index of geometry in 4096 tile space...
					float x = _geom[0][0].X;
					float y = _geom[0][0].Y;

					var centroidVector = new Vector3();
					foreach (var point in Points[0])
					{
						centroidVector += point;
					}
					centroidVector = centroidVector / Points[0].Count;

					Vector3 tilePos = Tile.transform.position;

					Vector2d _latLon = abstractMap.WorldToGeoPositionDouble(centroidVector);

					Vector2 latlonToUnityTilePosition = Conversions.LatitudeLongitudeToUnityTilePosition(_latLon, Tile);
					Vector2d latlonToUnityTilePositionVector2d = Conversions.LatitudeLongitudeToUnityTilePositionVector2d(_latLon, Tile);

					Vector2d constantTileCoordOther = Conversions.LatitudeLongitudeToUnityTilePositionVector2d(_latLon, 16, 0.1635333f);

					Vector2 LatitudeLongitudeToVectorTilePosition = Conversions.LatitudeLongitudeToVectorTilePosition(_latLon, 16);// 16);


					UnwrappedTileId myConsistentTileId = Conversions.LatitudeLongitudeToTileId(_latLon.x, _latLon.y, 30);

					string latLonX_string = _latLon.x.ToString();
					string latLonY_string = _latLon.y.ToString();

					string latLonX_subString = latLonX_string.Split('.')[1];
					string latLonY_subString = latLonY_string.Split('.')[1];

					char smallestX = latLonX_subString[1];
					char smallestY = latLonY_subString[1];


					int tileIdX = (int)Mathf.Repeat(myConsistentTileId.X, 4096);
					int tileIdY = (int)Mathf.Repeat(myConsistentTileId.Y, 4096);


					//randomValue = geoRandom.GetRandomValue(xx, yy);
					randomValue = geoRandom.GetRandomValue(tileIdX, tileIdY);


					Vector3 TilePositionFlat = new Vector3 (Tile.transform.position.x, 0f, Tile.transform.position.z);
					Vector3 CentroidFlat = new Vector3(centroidVector.x, 0f, centroidVector.z);

					long height = (long)Data.GetProperties()["height"];


					string heightString = height.ToString();

					int total = 1;
					for (int i = 0; i < heightString.Length; i++)
					{
						int s = (int)heightString[i];
						if(s != 0)
						{
							total = total * s;
						}
						else
						{
							total = 1;
						}
					}

					//int heightMultiple = 
					int lastH = (int)heightString[heightString.Length - 1];

					//int sum = (((int)smallestX * lastH) / ((int)smallestY) / lastH));// + lastH;
					//Debug.Log(_latLon + ": " + smallestX + ", " + smallestY + ", " + lastH);// + ", " + sum);
					Debug.Log(total);
					if (Data.Id.ToString() == "4513369021")
					{
						//long height = (long)Data.GetProperties()["height"];
						//Debug.Log(height.GetType().ToString());
						Debug.Log(height);
						Debug.Log("Deterministic randomness test ---------->");
						//Debug.Log("Feature ID : " + Data.Id.ToString());
						Debug.Log("Lat/Lon : " + _latLon);
						Debug.Log(latLonX_string);
						Debug.Log(latLonY_string);
						//Debug.Log(smallestX);
						//Debug.Log(smallestY);
						//Debug.Log("LatLon => tile space as Vector2  : " + latlonToUnityTilePosition);
						//Debug.Log("LatLon => tile space as Vector2d : " + latlonToUnityTilePositionVector2d);
						//Debug.Log("myConsistentTileId : " + myConsistentTileId.ToString() + "; X/Y : " + myConsistentTileId.X +  ", " + myConsistentTileId.Y);



						//Debug.Log("Centroid : " +  centroidVector / Tile.TileScale);
						//Debug.Log("LatLon : " + _latLon);
						//Debug.Log("LatLon 100 = " + latLonX_100 + ", " + latLonY_100);
						//Debug.Log("Texture coords - x : " + tileIdX + ", y : " + tileIdY);
						//Debug.Log("Random val = " + randomValue);
						//Debug.Log(myConsistentTileId.ToString());

						//Debug.Log("Zoom : " + abstractMap.Zoom);
						//Debug.Log("TileScale : " + Tile.TileScale);
						//Debug.Log("FirstPoint : " + firstPoint);

						//Debug.Log("constantTileCoord : " + constantTileCoord);
						//Debug.Log("constantTileCoordOther : " + constantTileCoordOther);
						//Debug.Log("LatitudeLongitudeToVectorTilePosition : " + LatitudeLongitudeToVectorTilePosition);
						//Debug.Log("XX : " + xx);
						//Debug.Log("YY : " + yy);
						//Debug.Log("Random Value : " + randomValue);


						//geoRandom.AddTileId(myConsistentTileId.ToString());

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
