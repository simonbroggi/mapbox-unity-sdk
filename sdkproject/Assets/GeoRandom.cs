using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mapbox.Utils;

public class GeoRandom : MonoBehaviour {

	public Texture2D texture;
	public string idToQuery;

	public List<string> tileIds;

	public bool same;
	// Use this for initialization
	void Awake()
	{
		Assert.IsNotNull(texture, "No texture assigned");
		tileIds = new List<string>();
		same = true;
	}

	public float GetRandomValue(float x, float y)
	{
		return texture.GetPixel((int)x, (int)y).r;
	}

	public void AddTileId(string id)
	{
		tileIds.Add(id);
		if(tileIds.Count == 1)
		{
			return;
		}
		string last = tileIds[0];
		for (int i = 1; i < tileIds.Count; i++)
		{
			//if(tileIds[i] != last)
			if(!string.Equals(tileIds[i], last))
			{
				same = false;
				return;
			}
			last = tileIds[i];
		}
	}
}
