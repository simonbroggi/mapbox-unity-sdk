namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Unity.MeshGeneration.Components;

	public class FeatureSelectionDetector : MonoBehaviour
	{
		private FeatureUiMarker _marker;
		private VectorEntity _feature;

		public void OnEnable(){
			GetComponent<MeshRenderer> ().material.color = Color.red;
			transform.localScale = new Vector3 (1.1f,1.1f,1.1f);
		}

		public void OnMouseUpAsButton()
		{
			_marker.Show(_feature);
		}

		private void OnMouseOver()
		{
			_marker.Show(_feature);
			GetComponent<MeshRenderer> ().enabled = true;
		}

		private void OnMouseExit()
		{
			GetComponent<MeshRenderer> ().enabled = false;
		}

		internal void Initialize(FeatureUiMarker marker, VectorEntity ve)
		{
			_marker = marker;
			_feature = ve;
		}
	}
}