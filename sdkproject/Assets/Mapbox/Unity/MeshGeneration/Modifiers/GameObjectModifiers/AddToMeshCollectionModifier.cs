namespace Mapbox.Unity.MeshGeneration.Modifiers
{
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Components;
    using Mapbox.Unity.MeshGeneration.Data;

    [CreateAssetMenu(menuName = "Mapbox/Modifiers/Add To Mesh Collection Modifier")]
    public class AddToMeshCollectionModifier : MeshModifier
    {
        [SerializeField]
        private MeshDataCollectionBase _collection;
		
        public override void Run(VectorFeatureUnity feature, MeshData md, UnityTile tile = null)
		{
			_collection.AddFeature(feature, md, tile);
		}
    }
}
