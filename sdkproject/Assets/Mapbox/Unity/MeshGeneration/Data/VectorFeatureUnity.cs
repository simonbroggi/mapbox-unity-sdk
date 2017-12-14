namespace Mapbox.Unity.MeshGeneration.Data
{
	using Mapbox.VectorTile;
	using System.Collections.Generic;
	using Mapbox.VectorTile.Geometry;
	using UnityEngine;
	using Mapbox.Unity.Utilities;

	public class VectorFeatureUnity
	{
		public VectorTileFeature Data;
		public Dictionary<string, object> Properties;
		public List<List<Vector3>> Points = new List<List<Vector3>>();
		public List<List<Vector3>> WorldSpacePoints = new List<List<Vector3>>();

		public Vector3 worldSpaceCenter;

		private double _rectSizex;
		private double _rectSizey;
		private int _geomCount;
		private int _pointCount;
		private List<Vector3> _newPoints = new List<Vector3>();
		private List<Vector3> _newWorldSpacePoints = new List<Vector3>();
		private List<List<Point2d<float>>> _geom;

		public VectorFeatureUnity()
		{
			Points = new List<List<Vector3>>();
		}

		public VectorFeatureUnity(VectorTileFeature feature, UnityTile tile, float layerExtent)
		{
			Data = feature;
			Properties = Data.GetProperties();

			// foreach(var p in Properties)
			// {
			// 	Debug.Log(p);
			// }
			Points.Clear();
			WorldSpacePoints.Clear();

			_rectSizex = tile.Rect.Size.x;
			_rectSizey = tile.Rect.Size.y;

			_geom = feature.Geometry<float>(0);
			_geomCount = _geom.Count;
			for (int i = 0; i < _geomCount; i++)
			{
				_pointCount = _geom[i].Count;
				_newPoints = new List<Vector3>(_pointCount);
				_newWorldSpacePoints = new List<Vector3>(_pointCount);
				for (int j = 0; j < _pointCount; j++)
				{
					var point = _geom[i][j];
					Vector3 ppoint = new Vector3((float)(point.X / layerExtent * _rectSizex - (_rectSizex / 2))* tile.TileScale, 0, (float)((layerExtent - point.Y) / layerExtent * _rectSizey - (_rectSizey / 2)) * tile.TileScale);
					_newPoints.Add(ppoint);
					Vector3 worldSpacePoint = tile.transform.position + ppoint;//Conversions.GeoToWorldPosition(ppoint);
					_newWorldSpacePoints.Add(worldSpacePoint);
				}
				Points.Add(_newPoints);
				WorldSpacePoints.Add(_newWorldSpacePoints);
			}
		}
	}
}
