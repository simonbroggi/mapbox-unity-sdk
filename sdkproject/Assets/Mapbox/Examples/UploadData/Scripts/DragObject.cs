namespace Mapbox.Examples
{
	using Mapbox.Unity.MeshGeneration.Data;
	using UnityEngine;

	public class DragObject : MonoBehaviour
	{
		private Vector3 screenPoint;
		private Vector3 offset;
		private VectorEntity _feature;

		[SerializeField]
		UploadToTileSet _uploadManager;

		void OnMouseUp()
		{
			_uploadManager.UpdateFeature(gameObject.transform.position, _feature.Feature.Properties["id"].ToString());
		}

		void OnMouseDown()
		{
			screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		}

		void OnMouseDrag()
		{
			Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
			cursorPosition.y = transform.position.y;
			transform.position = cursorPosition;
		}

		internal void Initialize(VectorEntity ve, UploadToTileSet uploadManager)
		{
			_feature = ve;
			_uploadManager = uploadManager;
		}
	}
}
