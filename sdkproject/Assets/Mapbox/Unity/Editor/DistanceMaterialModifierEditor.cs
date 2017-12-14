namespace Mapbox.Editor
{
	using UnityEngine;
	using UnityEditor;
	using Mapbox.Editor.NodeEditor;
	using Mapbox.Unity.MeshGeneration.Modifiers;

	[CustomEditor(typeof(DistanceMaterialModifier))]
	public class DistanceMaterialModifierEditor : MaterialModifierEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			DistanceMaterialModifier t = (DistanceMaterialModifier)target;
			EditorGUI.BeginChangeCheck();

			t.m_method = (DistanceMaterialModifier.LocationInput)EditorGUILayout.EnumPopup("Distance eval method:", t.m_method);

			switch (t.m_method)
			{
				case DistanceMaterialModifier.LocationInput.Vector3:
					t.m_vector3Input = EditorGUILayout.Vector3Field("Vector3: ", t.m_vector3Input);
					break;
				case DistanceMaterialModifier.LocationInput.LatLon:
					t.m_latLonInput = EditorGUILayout.TextField("LatLon: ", t.m_latLonInput);
					break;
				default:
					break;
			}

			t.m_minDistance = EditorGUILayout.FloatField("Min distance:", Mathf.Max(0.01f, t.m_minDistance));
			t.m_maxDistance = EditorGUILayout.FloatField("Max distance:", Mathf.Max(t.m_minDistance, t.m_maxDistance));

			SerializedProperty colorGradient = serializedObject.FindProperty("m_gradient");
			EditorGUILayout.PropertyField(colorGradient, true, null);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}