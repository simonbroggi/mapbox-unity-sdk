using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Mapbox/Feature Collection (Mesh Data)")]
public class MeshDataCollectionBase : ScriptableObject
{
	public KDTree.KDTree<VectorFeatureUnity> Entities;
	public Dictionary<VectorFeatureUnity, MeshData> Data;
	public Dictionary<VectorFeatureUnity, Vector3> Positions;
	public int Count;

	private void OnEnable()
	{
		Entities = new KDTree.KDTree<VectorFeatureUnity>(2);
		Data = new Dictionary<VectorFeatureUnity, MeshData>();
		Positions = new Dictionary<VectorFeatureUnity, Vector3>();
	}

	public void AddFeature(VectorFeatureUnity feature, MeshData md, UnityTile tile)
	{
		var pos = Vector3.zero;
		foreach (var item in md.Vertices)
		{
			pos += item;
		}
		pos /= md.Vertices.Count;
		pos += tile.transform.position;
		Entities.AddPoint(new double[] { pos.x, pos.z }, feature);
		Data.Add(feature, md);
		Positions.Add(feature, tile.transform.position);
		Count = Entities.Size;
	}
}
