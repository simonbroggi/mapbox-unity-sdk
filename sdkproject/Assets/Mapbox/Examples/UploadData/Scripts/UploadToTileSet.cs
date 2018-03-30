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
			//Get access token. 
			myAccessToken = Unity.MapboxAccess.Instance.Configuration.AccessToken;
		}

		/// <summary>
		/// Updates the feature.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="featureId">Feature identifier.</param>
		public void UpdateFeature(Vector3 position, string featureId)
		{
			// Convert the world position of the feature (treasure chest) into latitude longitude. 
			var latlong = _mapManager.WorldToGeoPosition(position);
			// Trigger upload of the feature with updated position. 
			StartCoroutine(UpdateFeatureInDataset(featureId.ToString(), "Point-" + featureId.ToString(), latlong));
		}
		/// <summary>
		/// Sends the web request to update the feature in the dataset using the Mapbox Uploads API.
		/// </summary>
		/// <returns>The new dataset feature.</returns>
		/// <param name="myFeatureId">feature identifier.</param>
		/// <param name="featureName">Feature name.</param>
		/// <param name="latlong">Position in latitude longitude.</param>
		IEnumerator UpdateFeatureInDataset(string myFeatureId, string featureName, Vector2d latlong)
		{
			// Check to make sure latitude longitude are in the correct range. 
			var xDelta = latlong.x;
			var yDelta = latlong.y;

			xDelta = xDelta > 0 ? Mathd.Min(xDelta, Mapbox.Utils.Constants.LatitudeMax) : Mathd.Max(xDelta, -Mapbox.Utils.Constants.LatitudeMax);
			yDelta = yDelta > 0 ? Mathd.Min(yDelta, Mapbox.Utils.Constants.LongitudeMax) : Mathd.Max(yDelta, -Mapbox.Utils.Constants.LongitudeMax);

			// Form the URL string. 
			string postURL = string.Format(
				"https://api.mapbox.com/datasets/v1/{0}/{1}/features/{2}?access_token={3}"
				, myId
				, myDatasetId
				, myFeatureId
				, myAccessToken
			);

			//Construct the geoJSON for the feature. Use a JSON library to do this. 
			// Using strings works for our simple case. 
			string typeProperty = string.Format("{{\"type\":\"Point\",\"coordinates\":[{0},{1}]}}", yDelta, xDelta);
			string geometryProperty = string.Format("\"geometry\":");
			string nameProperty = string.Format("\"name\":\"{0}\"", featureName);
			string idProperty = string.Format("\"id\":\"{0}\"", myFeatureId);
			string nameIdCombined = string.Format("{{{0},{1}}}", nameProperty, idProperty);
			string combined = string.Format("{0},{1}{2}", nameIdCombined, geometryProperty, typeProperty);

			string geoJSON = string.Format(
				"{{\"type\":\"Feature\",\"properties\":{0}}}"
				, combined);

			UnityWebRequest www = UnityWebRequest.Put(postURL, geoJSON);
			www.SetRequestHeader("Content-Type", "application/json");

			// Send web request to update feature in the dataset. 
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log("www error: " + www.error + www.responseCode);
			}
			else
			{
				//Debug.Log("www = " + www.downloadHandler.text);

				// If we receive a successful upload response, we should be good to send a tileset update request. 
				StartCoroutine(ApplyDatasetToTileset());
			}
		}

		/// <summary>
		/// Applies the dataset to tileset.
		/// </summary>
		/// <returns>The dataset to tileset.</returns>
		IEnumerator ApplyDatasetToTileset()
		{
			string postURL = string.Format(
				"https://api.mapbox.com/uploads/v1/{0}?access_token={1}"
				, myId
				, myAccessToken
			);

			string datasetTilesetInfo = string.Format(
				@"{{""tileset"":""{1}"",""url"":""mapbox://datasets/{0}/{2}"",""name"":""{3}""}}"
				, myId
				, myTilesetId
				, myDatasetId
				, myLayerName
			);

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
