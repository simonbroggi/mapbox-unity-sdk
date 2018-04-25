namespace Mapbox.Unity.MeshGeneration.Interfaces
{
	using Mapbox.VectorTile;
	using UnityEngine;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Data;
	using System;

	/// <summary>
	/// Layer visualizers contains sytling logic and processes features
	/// </summary>
	public abstract class LayerVisualizerBase 
    {
        public bool Active = true;
        public abstract string Key { get; set; }

        public abstract void Create(VectorTileLayer layer, UnityTile tile, Action callback = null);
		public abstract void SetProperties(ISubLayerProperties properties, LayerPerformanceOptions performanceOptions);

		public virtual void Initialize()
		{

		}

		public void UnregisterTile(UnityTile tile)
		{
			OnUnregisterTile(tile);
		}

		public virtual void OnUnregisterTile(UnityTile tile)
		{

		}
	}

}
