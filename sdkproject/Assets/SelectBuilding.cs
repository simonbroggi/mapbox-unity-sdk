using KDTree;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Modifiers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBuilding : MonoBehaviour
{
	public MeshDataCollectionBase Collection;
	public AbstractMap Map;
	private NearestNeighbour<VectorFeatureUnity> pIter;
	Ray ray;
	RaycastHit info;
	public int MaxCount = 1;
	public float Range = 10;
	public GameObject go;
	MeshFilter mf;
	Mesh mesh;
	int _counter = 0;
	public ModifierStack ModStack;

	void Start()
	{
		mf = go.GetComponent<MeshFilter>();
		mesh = mf.mesh;
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out info))
			{
				var relativePoint = Map.Root.InverseTransformPoint(info.point);
				pIter = Collection.Entities.NearestNeighbors(new double[] { relativePoint.x, relativePoint.z }, MaxCount, Range);
				while (pIter.MoveNext())
				{
					var feature = pIter._Current;
					var meshData = new MeshData();//Collection.Data[feature];

					foreach (var modifier in ModStack.MeshModifiers)
					{
						modifier.Run(feature, meshData);
					}
					
					mesh.Clear();
					mesh.subMeshCount = meshData.Triangles.Count;
					mesh.SetVertices(meshData.Vertices);
					mesh.SetNormals(meshData.Normals);
					if (meshData.Tangents.Count > 0)
						mesh.SetTangents(meshData.Tangents);
					_counter = meshData.Triangles.Count;
					for (int i = 0; i < _counter; i++)
					{
						mesh.SetTriangles(meshData.Triangles[i], i);
					}
					_counter = meshData.UV.Count;
					for (int i = 0; i < _counter; i++)
					{
						mesh.SetUVs(i, meshData.UV[i]);
					}
					mf.mesh = mesh;
					go.transform.localPosition = Collection.Positions[feature];
					break;
				}
			}
		}
	}
}
