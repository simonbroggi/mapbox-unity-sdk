namespace Mapbox.Unity.Map
{

	using System;
	using Mapbox.Unity.MeshGeneration.Interfaces;
	using Mapbox.Unity.MeshGeneration.Modifiers;

	public interface ISubLayerProperties
	{
		/// <summary>
		/// Instance of the LayerVisualizer that will be used to process these sublayer properties
		/// </summary>
		/// <value>The type of the visualizer.</value>
		LayerVisualizerBase Visualizer { get; }
		CoreVectorLayerProperties coreOptions { get; set; }
		VectorFilterOptions filterOptions { get; set; }
		GeometryExtrusionOptions extrusionOptions { get; set; }
		PositionTargetType moveFeaturePositionTo { get; set;  }

	}


}