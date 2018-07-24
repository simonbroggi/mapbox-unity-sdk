using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Modifiers;
using System.IO;
using System.Linq;

namespace Mapbox.Editor
{

	class BlockedFeatureEditor : EditorWindow
	{
		private const float _windowWidthMin = 600;
		private const float _windowHeightMin = 600f;

		private const float _windowWidthMax = 850;
		private const float _windowHeightMax = 1000f;

		private const float _leftColumnWidth = 250f;
		private const float _rightColumnWidth = 400f;

		private const float _leftColumnButtonWidth = 230f;
		private const float _rightColumnButtonWidth = 380f;

		private static BlockedFeatureEditor window;

		[MenuItem("Mapbox/Blocked Feature Editor")]
		public static void ShowWindow()
		{
			window = EditorWindow.GetWindow(typeof(BlockedFeatureEditor)) as BlockedFeatureEditor;
			window.minSize = new Vector2(_windowWidthMin, _windowHeightMin);
			window.maxSize = new Vector2(_windowWidthMax, _windowHeightMax);
			window.ShowUtility();
		}

		List<ScriptableObject> _assets;
		List<string> featureIds;

		Object selectedModifier;

		Vector2 scrollPos;

		GUIStyle header;

		private int _idFontSize = 14;

		private int _selected = -1;

		/*

		string[] GetSelectedMapFeatureNameAndId()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			//string
			if (activeGameObject == null)
			{
				return null;
			}
			int lastSpaceIndex = activeGameObject.name.LastIndexOf(' ');
			string[] nameSplit = n.Split(' - ');
			if (nameSplit != null && nameSplit.Length == 2)
			{
				string[] nameSplitSplit = nameSplit[1].Split(' ');
				toAdd = nameSplitSplit[1];
				if (toAdd != null)
				{
					addOk = true;
				}
			}

		}
	*/
		void OnGUI()
		{
			
			if (_assets == null || _assets.Count == 0)
			{
				var list = AssetDatabase.FindAssets("t:" + typeof(ReplaceFeatureModifier).Name);
				_assets = new List<ScriptableObject>();
				foreach (var item in list)
				{
					var assetPath = AssetDatabase.GUIDToAssetPath(item);
					var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(ReplaceFeatureModifier)) as ScriptableObject;
					_assets.Add(asset);
				}
				_assets = _assets.OrderBy(x => x.GetType().Name).ThenBy(x => x.name).ToList();
			}

			GUIStyle titleStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));

			titleStyle.fontSize = 32;
			titleStyle.normal.textColor = Color.white;
			titleStyle.fontStyle = FontStyle.Bold;

			titleStyle.border.bottom = 2;
			titleStyle.border.top = 2;
			titleStyle.border.right = 2;
			titleStyle.border.left = 2;
			titleStyle.alignment = TextAnchor.MiddleLeft;

			GUIStyle headerStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));

			headerStyle.fontSize = 18;
			headerStyle.normal.textColor = Color.white;
		
			headerStyle.alignment = TextAnchor.MiddleLeft;

			GUIStyle featureButtonStyle = new GUIStyle(GUI.skin.GetStyle("ShurikenModuleTitle"));

			featureButtonStyle.fontSize = _idFontSize;
			featureButtonStyle.normal.textColor = Color.white;

			featureButtonStyle.alignment = TextAnchor.MiddleCenter;

			featureButtonStyle.font = (new GUIStyle("Label")).font;

			featureButtonStyle.alignment = TextAnchor.MiddleLeft;

			featureButtonStyle.border.bottom = 2;
			featureButtonStyle.border.top = 2;
			featureButtonStyle.border.right = 2;
			featureButtonStyle.border.left = 2;

			featureButtonStyle.fixedHeight = 22;
			//featureButtonStyle.contentOffset = new Vector2(20f, -2f);

			GUIStyle idStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));

			idStyle.fontSize = _idFontSize;
			idStyle.normal.textColor = Color.white;
			idStyle.fixedHeight = 20;
			idStyle.border.bottom = 2;
			idStyle.border.top = 2;
			idStyle.border.right = 2;
			idStyle.border.left = 2;
			idStyle.alignment = TextAnchor.MiddleCenter;

			GUIStyle idRemoveButtonStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));

			idRemoveButtonStyle.fontSize = _idFontSize;
			idRemoveButtonStyle.normal.textColor = Color.white;

			idRemoveButtonStyle.alignment = TextAnchor.MiddleCenter;

			EditorGUILayout.LabelField("Blocked Feature Editor", titleStyle, GUILayout.Height(60));

			GUILayout.BeginHorizontal("box");

			GUILayout.BeginVertical("box", GUILayout.Width(_leftColumnWidth));
			EditorGUILayout.LabelField("Replace Feature Modifiers", headerStyle, GUILayout.Height(40));

			Color color = GUI.color;

			var st = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
	
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, st);
			for (int i = 0; i < _assets.Count; i++)
			{
				ScriptableObject asset = _assets[i];
				if (asset == null) //yea turns out this can happen
					continue;
				GUI.color = new Color(0.8f, 0.8f, 0.8f);
				if(_selected == i)
				{
					GUI.color = new Color(1,1,1);
				}
				if (GUILayout.Button(new GUIContent(asset.name), GUILayout.Width(_leftColumnButtonWidth)))
				{
					selectedModifier = asset;
					_selected = i;
				}
			}
			GUI.color = color;
			EditorGUILayout.EndScrollView();
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			EditorGUILayout.LabelField("Blocked IDs", headerStyle, GUILayout.Height(40));

			if (selectedModifier != null)
			{
				ReplaceFeatureModifier mod = selectedModifier as ReplaceFeatureModifier;//selectedObject
				if (mod != null)
				{
					featureIds = mod.ExplicitlyBlockedFeatureIds;
					if (featureIds != null)
					{
						for (int i = 0; i < featureIds.Count; i++)
						{
							string id = featureIds[i];

							if (string.IsNullOrEmpty(id))
							{
								continue;
							}

							GUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(id, idStyle, GUILayout.Width(100));
							if (GUILayout.Button(new GUIContent("X"), GUILayout.Width(30), GUILayout.Height(20)))
							{
								featureIds.Remove(id);
							}
							GUILayout.EndHorizontal();
						}
					}
				}
			}

			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			EditorGUILayout.LabelField("Selected object:", GUILayout.Height(40));


			EditorGUILayout.Separator();
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("select a map feature", idStyle);//, GUILayout.Width(100));

			if (GUILayout.Button(new GUIContent("Add"), featureButtonStyle))//, GUILayout.Width(200)))
			{
				//if (addOk)
				//{
				//	featureIds.Add(toAdd);
				//}
				//else
				//{
					Debug.LogError("Please select a map feature to add");
				//}

			}

			GUILayout.EndHorizontal();
			EditorGUILayout.Separator();
						/*
						GUI.color = new Color(0.8f, 0.8f, 0.8f);
						if (addOk)
						{
							GUI.color = new Color(1, 1, 1);
						}


						if (GUILayout.Button(new GUIContent("Add"), featureButtonStyle, GUILayout.Width(200)))
						{
							if (addOk)
							{
								featureIds.Add(toAdd);
							}
							else
							{
								Debug.LogError("Please select a map feature to add");
							}

						}


					}
				}
			}
					//scrollPos = EditorGUILayout.BeginScrollView(scrollPos, st);
					//List<string> featureIds = mod.ExplicitlyBlockedFeatureIds;
					//if(featureIds != null)
					//{
					//	Debug.Log("HAHHAAH");

					//	for (int i = 0; i < featureIds.Count; i++)
					//	{
							//string id = featureIds[i];

							//if (!string.IsNullOrEmpty(id))
							//{
								//Debug.Log(id);
							//}
								/*
								GUILayout.BeginHorizontal();
								EditorGUILayout.LabelField(id, idStyle, GUILayout.Width(100));

								if (GUILayout.Button(new GUIContent("X"), GUILayout.Width(30), GUILayout.Height(20)))
								{
									featureIds.Remove(id);
								}
								GUILayout.EndHorizontal();
							}
							*/
					//	}
						//EditorGUILayout.EndScrollView();

						/*
						GameObject selectedObj = Selection.activeGameObject;
						string n = "";
						string toAdd = null;
						bool addOk = false;
						if (selectedObj != null)
						{
							n = selectedObj.name;

							string[] nameSplit = n.Split('-');
							if (nameSplit != null && nameSplit.Length == 2)
							{
								string[] nameSplitSplit = nameSplit[1].Split(' ');
								toAdd = nameSplitSplit[1];
								if (toAdd != null)
								{
									addOk = true;
								}
							}
						}



						//GUILayout.BeginHorizontal();
						//EditorGUILayout.LabelField("Selected object:", GUILayout.Height(40));
						//EditorGUILayout.LabelField(n, idStyle, GUILayout.Width(100));
						/*
						GUI.color = new Color(0.8f, 0.8f, 0.8f);
						if (addOk)
						{
							GUI.color = new Color(1, 1, 1);
						}


						if (GUILayout.Button(new GUIContent("Add"), featureButtonStyle, GUILayout.Width(200)))
						{
							if (addOk)
							{
								featureIds.Add(toAdd);
							}
							else
							{
								Debug.LogError("Please select a map feature to add");
							}

						}
						*/
					//}

			//		GUI.color = color;
			//		GUILayout.EndHorizontal();
			//	}
			//}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			Repaint();
		}
	}
}