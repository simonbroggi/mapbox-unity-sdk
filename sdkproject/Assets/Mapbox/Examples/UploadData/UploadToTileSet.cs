namespace Mapbox.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using Mapbox.Unity.Map;
	using Mapbox.Utils;
	using UnityEngine;
	using UnityEngine.Networking;
	using System;
	using SimpleJSON;

	public class UploadToTileSet : MonoBehaviour
	{
		[SerializeField]
		private Camera _referenceCamera;

		[SerializeField]
		private AbstractMap _mapManager;

		[SerializeField]
		private string myDatasetId;
		[SerializeField]
		private string myId;
		[SerializeField]
		private string myTilesetId;
		[SerializeField]
		private string myLayerName;
		[SerializeField]
		private GameObject _marker;
		private string myAccessToken;

		private bool addedFeature = false;
		private bool datasetUpdated = false;
		float _elapsedTime;
		float _updateInterval = 10;

		// Use this for initialization
		void Start()
		{
			myAccessToken = Unity.MapboxAccess.Instance.Configuration.AccessToken;
		}

		// Update is called once per frame
		void LateUpdate()
		{
			if (Input.GetMouseButtonUp(0))
			{
				var mousePosScreen = Input.mousePosition;
				//assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
				//http://answers.unity3d.com/answers/599100/view.html
				mousePosScreen.z = _referenceCamera.transform.localPosition.y;
				var pos = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

				var latlongDelta = _mapManager.WorldToGeoPosition(pos);
				//Debug.Log("Latitude: " + latlongDelta.x + " Longitude: " + latlongDelta.y);
				var marker = Instantiate(_marker);
				marker.transform.position = pos;
				marker.transform.SetParent(_mapManager.Root);
				var newGuid = Guid.NewGuid();
				StartCoroutine(PostNewDatasetFeature(newGuid.ToString(), "Point-" + newGuid.ToString(), latlongDelta));
			}
		}

		IEnumerator PostNewDatasetFeature(string myFeatureId, string featureName, Vector2d latlongDelta)
		{
			string postURL = string.Format(
				"https://api.mapbox.com/datasets/v1/{0}/{1}/features/{2}?access_token={3}"
				, myId
				, myDatasetId
				, myFeatureId
				, myAccessToken
			);

			//Debug.Log(postURL);
			string typeProperty = string.Format("{{\"type\":\"Point\",\"coordinates\":[{0},{1}]}}", latlongDelta.y, latlongDelta.x);
			string geometryProperty = string.Format("\"geometry\":");
			string nameProperty = string.Format("{{\"name\":\"{0}\"}}", featureName);
			string combined = string.Format("{0},{1}{2}", nameProperty, geometryProperty, typeProperty);

			string geoJSONstring2 = string.Format(
				"{{\"type\":\"Feature\",\"properties\":{0}}}"
				, combined);

			//Debug.Log(geoJSONstring2);
			UnityWebRequest www = UnityWebRequest.Put(postURL, geoJSONstring2);
			www.SetRequestHeader("Content-Type", "application/json");
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log("www error: " + www.error + www.responseCode);
			}
			else
			{
				Debug.Log("www = " + www.downloadHandler.text);

				// If we receive a successful upload response, we should be good to send a tileset update request. 
				StartCoroutine(ApplyDatasetToTileset());
			}
		}

		//IEnumerator CheckIfDataSetIsReady(bool start = false)
		//{
		//	string postURL = string.Format("https://api.mapbox.com/datasets/v1/{0}/{1}/?access_token={2}"
		//									, myId
		//									, myDatasetId
		//									, myAccessToken
		//									);
		//	//Debug.Log("Check Update : " + postURL);
		//	UnityWebRequest www = UnityWebRequest.Get(postURL);
		//	www.SetRequestHeader("content-type", "application/json");

		//	yield return www.SendWebRequest();

		//	if (www.isNetworkError || www.isHttpError)
		//	{
		//		Debug.Log("CheckIfDataSetIsReady www error: " + www.error);
		//	}
		//	else
		//	{
		//		Debug.Log("CheckIfDataSetIsReady www = " + www.downloadHandler.text);
		//		var datasetObj = SimpleJSON.JSON.Parse(www.downloadHandler.text);
		//		currentFeatureCount = datasetObj["features"].AsInt;
		//		Debug.Log("Start : " + start);
		//		if (start)
		//		{
		//			previousFeatureCount = currentFeatureCount;
		//		}
		//		else
		//		{
		//			Debug.Log("Current -> " + currentFeatureCount + " Previous -> " + previousFeatureCount);
		//			if (currentFeatureCount > previousFeatureCount)
		//			{
		//				Debug.Log("Updated..");
		//				datasetObj = true;
		//				addedFeature = false;
		//				previousFeatureCount = currentFeatureCount;
		//				StartCoroutine(ApplyDatasetToTileset());
		//			}
		//		}

		//		Debug.Log("Features = " + currentFeatureCount);
		//	}
		//}


		IEnumerator ApplyDatasetToTileset()
		{
			string postURL = string.Format(
				"https://api.mapbox.com/uploads/v1/{0}?access_token={1}"
				, myId
				, myAccessToken
			);

			//Debug.Log(postURL);
			string datasetTilesetInfo = string.Format(
				@"{{""tileset"":""{1}"",""url"":""mapbox://datasets/{0}/{2}"",""name"":""{3}""}}"
				, myId
				, myTilesetId
				, myDatasetId
				, myLayerName
			);
			//Debug.Log(datasetTilesetInfo);
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(datasetTilesetInfo);
			// initialize as PUT
			UnityWebRequest www = UnityWebRequest.Put(postURL, bytes);
			www.SetRequestHeader("content-type", "application/json");
			// HACK!!! override method and convert to POST
			www.method = "POST";

			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log("ApplyDatasetToTileset www error: " + www.error + www.responseCode);
			}
			else
			{
				Debug.Log("ApplyDatasetToTileset www = " + www.downloadHandler.text);
			}
		}
	}
}
