namespace Mapbox.Examples
{
	using Mapbox.Unity.MeshGeneration.Data;
	using Mapbox.Unity.MeshGeneration.Modifiers;
	using UnityEngine;

	[CreateAssetMenu(menuName = "Mapbox/Modifiers/Upload Point Feature Modifier")]
	public class UploadPointFeatureModifier : GameObjectModifier
	{

		private UploadToTileSet _uploadManager;
		private DragObject _dragObject;

		public override void Initialize()
		{
			_uploadManager = FindObjectOfType<UploadToTileSet>();
		}

		public override void Run(VectorEntity ve, UnityTile tile)
		{
			_dragObject = ve.GameObject.GetComponentInChildren<DragObject>();
			_dragObject.Initialize(ve, _uploadManager);

		}
	}
}