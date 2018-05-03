using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Disable shadow casting on meshes
/// </summary>

public class ArBuildingBehavior : MonoBehaviour {

	private MeshRenderer _meshRenderer;

	// Use this for initialization
	void Start () {

		_meshRenderer = GetComponent<MeshRenderer>();
		_meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		
	}
}
