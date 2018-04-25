namespace Mapbox.Unity.Map
{
	using UnityEngine;

	using System;
	using System.Collections.Generic;
	using Mapbox.Unity.MeshGeneration.Modifiers;
	using Mapbox.Unity.MeshGeneration.Interfaces;
	using Mapbox.Unity.Utilities;

	[Serializable]
	public class VectorSubLayerProperties : ISubLayerProperties
	{

		#region Interface Methods

		public LayerVisualizerBase Visualizer
		{
			get{ return _visualizer; }
		}

		public CoreVectorLayerProperties coreOptions
		{
			get { return _coreOptions; }
			set { _coreOptions = value; }
		}

		public VectorFilterOptions filterOptions
		{
			get { return _filterOptions; }
			set { _filterOptions = value; }
		}

		public GeometryExtrusionOptions extrusionOptions
		{
			get { return _extrusionOptions; }
			set { _extrusionOptions = value; }
		}

		public PositionTargetType moveFeaturePositionTo
		{
			get{ return _moveFeaturePositionTo; }
			set { _moveFeaturePositionTo = value;  }
		}

		[SerializeField]
		private VectorLayerVisualizer _visualizer = new VectorLayerVisualizer();
		[SerializeField]
		private CoreVectorLayerProperties _coreOptions = new CoreVectorLayerProperties();
		[SerializeField]
		private VectorFilterOptions _filterOptions = new VectorFilterOptions();
		[SerializeField]
		private GeometryExtrusionOptions _extrusionOptions = new GeometryExtrusionOptions
		{
			extrusionType = ExtrusionType.None,
			propertyName = "height",
			extrusionGeometryType = ExtrusionGeometryType.RoofAndSide,

		};
		[SerializeField]
		private PositionTargetType _moveFeaturePositionTo;

		#endregion


		public ColliderOptions colliderOptions = new ColliderOptions
		{
			colliderType = ColliderType.None,
		};
		public GeometryMaterialOptions materialOptions = new GeometryMaterialOptions();

		//HACK : workaround to avoid users accidentaly leaving the buildingsWithUniqueIds settign on and have missing buildings. 
		public bool honorBuildingIdSetting = true;
		public bool buildingsWithUniqueIds = false;

		[NodeEditorElement("Mesh Modifiers")]
		public List<MeshModifier> MeshModifiers;
		[NodeEditorElement("Game Object Modifiers")]
		public List<GameObjectModifier> GoModifiers;
	}
}
