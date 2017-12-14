namespace Mapbox.Unity.MeshGeneration.Modifiers
{
	using UnityEngine;
	using System.Collections.Generic;
	using Mapbox.Unity.MeshGeneration.Components;
	using Mapbox.Unity.MeshGeneration.Data;
	using Mapbox.Unity.Map;
	using System;

	/// <summary>
	/// Texture Modifier is a basic modifier which simply adds a TextureSelector script to the features.
	/// Logic is all pushed into this TextureSelector mono behaviour to make it's easier to change it in runtime.
	/// </summary>
	[CreateAssetMenu(menuName = "Mapbox/Modifiers/Distance Material Modifier")]
	//public class DistanceMaterialModifier : GameObjectModifier

	public class UnityTileMeshRendererBundle
	{
		private UnityTile m_tile;
		private MeshRenderer m_meshRenderer;

		public UnityTile Tile { get { return m_tile; }}
		public MeshRenderer MeshRenderer { get { return m_meshRenderer; }}

		public UnityTileMeshRendererBundle(UnityTile tile, MeshRenderer mr)
		{
			m_tile = tile;
			m_meshRenderer = mr;
		}
	}

	public class DistanceMaterialModifier : MaterialModifier
	{
		
		public enum LocationInput
        {
            Vector3,
            LatLon,
			GameObject
        }

		public LocationInput m_method;

		public string m_latLonInput;
		public Vector3 m_vector3Input;

		public Gradient m_gradient;

		public float m_minDistance;
		public float m_maxDistance;

		[SerializeField]
		private bool _projectMapImagery;
		[SerializeField]
		private MaterialList[] _materials;

		private Vector3 m_inputPosition;

		private bool m_hasRun;

		private List<UnityTileMeshRendererBundle> m_unityTileRenderers = new List<UnityTileMeshRendererBundle>();

		private void SetInput()
		{
			switch (m_method)
			{
				case LocationInput.Vector3:
					break;
				case LocationInput.LatLon:			
					break;
				case LocationInput.GameObject:
					GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        			cylinder.transform.position = Vector3.zero;
					cylinder.transform.localScale = new Vector3(1.0f, 100.0f,1.0f);
					cylinder.GetComponent<MeshRenderer>().sharedMaterial.color = Color.black;
					OnTransformHasChanged onTransformHasChanged = cylinder.AddComponent<OnTransformHasChanged>();
					onTransformHasChanged.AddListener(UpdateInputPosition);
					break;
				default:
					break;
			}
		}

		public void UpdateFromVector3()
		{
			UpdateInputPosition(m_vector3Input);
		}

		public void UpdateInputPosition(Vector3 position)
		{
			m_inputPosition = position;
			for(int i = 0; i < m_unityTileRenderers.Count; i++)
			{
				CalculateDistance(m_unityTileRenderers[i]);
			}
		}

		private void OnDisable()
		{
			Debug.Log("OnDisable");
			m_hasRun = false;
		}

		public void CalculateDistance(UnityTileMeshRendererBundle bundle)
		{
			Vector3 tilePosition = bundle.Tile.transform.position;
			
			float distance = Vector3.Distance(m_inputPosition, tilePosition);
			float gradientPosition = Mathf.InverseLerp(m_minDistance, m_maxDistance, distance);

			Color color = m_gradient.Evaluate(gradientPosition);
			for(int i = 0; i < bundle.MeshRenderer.materials.Length; i++)
			{
				bundle.MeshRenderer.materials[i].color = color;
			}
		}

		public override void Run(VectorEntity ve, UnityTile tile)
		{
			if(!m_hasRun)
			{
				SetInput();
				m_hasRun = true;
			}
			base.Run(ve, tile);

			UnityTileMeshRendererBundle unityTileMeshRendererBundle = new UnityTileMeshRendererBundle(tile, ve.MeshRenderer);
			m_unityTileRenderers.Add(unityTileMeshRendererBundle);

			CalculateDistance(unityTileMeshRendererBundle);
		}
	}
}
