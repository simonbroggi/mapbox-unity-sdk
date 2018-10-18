using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GeoRandom : MonoBehaviour {

	public Texture2D texture;

	// Use this for initialization
	void Awake()
	{
		Assert.IsNotNull(texture, "No texture assigned");
	}

	public float GetRandomValue(float x, float y)
	{
		return texture.GetPixel((int)x, (int)y).r;
	}
}
