namespace Mapbox.Editor
{
	using UnityEditor;
	using UnityEngine;
	using Mapbox.Unity.Map;
	using Mapbox.VectorTile.ExtensionMethods;

	[CustomPropertyDrawer(typeof(CoreVectorLayerProperties))]
	public class CoreVectorLayerPropertiesDrawer : PropertyDrawer
	{
		static float _lineHeight = EditorGUIUtility.singleLineHeight;
		bool _isGUIContentSet = false;
		GUIContent[] _sourceTypeContent;

		private GUIContent _mapIdGui = new GUIContent
		{
			text = "Map Id",
			tooltip = "Map Id corresponding to the tileset."
		};

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position.height = _lineHeight;

			// Draw label.
			EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, _lineHeight), property.FindPropertyRelative("isActive"));
			position.y += _lineHeight;
			var sourceTypeProperty = property.FindPropertyRelative("sourceType");
			var sourceTypeValue = (VectorSourceType)sourceTypeProperty.enumValueIndex;

			var displayNames = sourceTypeProperty.enumDisplayNames;
			int count = sourceTypeProperty.enumDisplayNames.Length;
			if (!_isGUIContentSet)
			{
				_sourceTypeContent = new GUIContent[count];
				for (int extIdx = 0; extIdx < count; extIdx++)
				{
					_sourceTypeContent[extIdx] = new GUIContent
					{
						text = displayNames[extIdx],
						tooltip = ((VectorSourceType)extIdx).Description(),
					};
				}
				_isGUIContentSet = true;
			}

			var typePosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent { text = "Data Source", tooltip = "Source tileset for Vector Data" });

			sourceTypeProperty.enumValueIndex = EditorGUI.Popup(typePosition, sourceTypeProperty.enumValueIndex, _sourceTypeContent);
			sourceTypeValue = (VectorSourceType)sourceTypeProperty.enumValueIndex;

			position.y += _lineHeight;
			var sourceOptionsProperty = property.FindPropertyRelative("sourceOptions");
			var layerSourceProperty = sourceOptionsProperty.FindPropertyRelative("layerSource");
			var layerSourceId = layerSourceProperty.FindPropertyRelative("Id");
			var isActiveProperty = sourceOptionsProperty.FindPropertyRelative("isActive");
			switch (sourceTypeValue)
			{
				case VectorSourceType.MapboxStreets:
				case VectorSourceType.MapboxStreetsWithBuildingIds:
					var sourcePropertyValue = MapboxDefaultVector.GetParameters(sourceTypeValue);
					layerSourceId.stringValue = sourcePropertyValue.Id;
					GUI.enabled = false;
					EditorGUILayout.PropertyField(sourceOptionsProperty, _mapIdGui);
					GUI.enabled = true;
					isActiveProperty.boolValue = true;
					break;
				case VectorSourceType.Custom:
					//layerSourceId.stringValue = string.Empty;
					EditorGUILayout.PropertyField(sourceOptionsProperty, _mapIdGui);
					isActiveProperty.boolValue = true;
					break;
				case VectorSourceType.None:
					isActiveProperty.boolValue = false;
					break;
				default:
					isActiveProperty.boolValue = false;
					break;
			}
			//position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("sourceOptions"));

			//position.y += _lineHeight;

			var primitiveType = property.FindPropertyRelative("geometryType");

			var geometryTypePosition = EditorGUI.PrefixLabel(new Rect(position.x, position.y, position.width, _lineHeight), GUIUtility.GetControlID(FocusType.Passive), new GUIContent { text = "Primitive Type", tooltip = "Primitive geometry type of the visualizer, allowed primitives - point, line, polygon." });
			EditorGUI.indentLevel--;
			primitiveType.enumValueIndex = EditorGUI.Popup(geometryTypePosition, primitiveType.enumValueIndex, primitiveType.enumDisplayNames);
			EditorGUI.indentLevel++;

			position.y += _lineHeight;
			EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, _lineHeight), property.FindPropertyRelative("layerName"));

			position.y += _lineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("snapToTerrain"));

			position.y += _lineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("groupFeatures"));

			if ((VectorPrimitiveType)primitiveType.enumValueIndex == VectorPrimitiveType.Line)
			{
				position.y += _lineHeight;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("lineWidth"));
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var sourceTypeProperty = property.FindPropertyRelative("geometryType");

			float height = 0.0f;
			height += (((((VectorPrimitiveType)sourceTypeProperty.enumValueIndex == VectorPrimitiveType.Line)) ? 6.0f : 5.0f) * EditorGUIUtility.singleLineHeight);

			return height;
		}
	}
}