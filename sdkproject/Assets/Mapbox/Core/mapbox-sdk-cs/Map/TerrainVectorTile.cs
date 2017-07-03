//-----------------------------------------------------------------------
// <copyright file="RawPngRasterTile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Mapbox.Map
{

	using Mapbox.Utils;
	using System;
	using mbxvt = Mapbox.VectorTile;

	public sealed class TerrainVectorTile : Tile
	{


		public float[,] Data { get; private set; }


		internal override TileResource MakeTileResource(string mapId)
		{
			return new TileResource(string.Format("http://localhost:3001/tiles/{0}.mvt", Id));
		}

		internal override bool ParseTileData(byte[] data)
		{
			data = Compression.Decompress(data);

			mbxvt.VectorTile vt = new mbxvt.VectorTile(data, false);
			mbxvt.VectorTileLayer lyr = vt.GetLayer(vt.LayerNames()[0]);
			mbxvt.VectorTileFeature feat = lyr.GetFeature(0);
			if (feat.GeometryType != mbxvt.Geometry.GeomType.XYZ)
			{
				throw new Exception("Not a terrain vector tile");
			}

			// Currently Mapbox.VectorTile.XYZ() returns heights as they are encoded
			// TODO: add scale parameter
			// Currently values have to be divided by 4: 0.25m resolution saved as integers
			int[,] matrix = feat.XYZ();
			int width = matrix.GetLength(0);
			int height = matrix.GetLength(1);
			Data = new float[width, height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Data[x, y] = matrix[x, y] / 4f;
				}
			}

			feat = null;
			lyr = null;
			vt = null;

			return true;
		}


	}
}
