//-----------------------------------------------------------------------
// <copyright file="RawPngRasterTile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Mapbox.Map
{
	public sealed class TerrainVectorTile : Tile
	{
		internal override TileResource MakeTileResource(string mapId)
		{
			return new TileResource(string.Format("http://localhost:3001/tiles/{0}.mvt", Id));
		}

		internal override bool ParseTileData(byte[] data)
		{
			throw new NotImplementedException();
		}
	}
}
