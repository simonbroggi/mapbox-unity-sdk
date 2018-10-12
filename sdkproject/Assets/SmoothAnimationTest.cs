using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Mapbox.Unity.Map;
using UnityEngine;

public class SmoothAnimationTest : MonoBehaviour
{
	public AbstractMap Map;
	public float Speed = 1;

	private void Start()
	{
		if (Map == null)
		{
			Map = FindObjectOfType<AbstractMap>();
		}
	}

	[ContextMenu("Zoom In")]
	public void ZoomIn()
	{
		StartCoroutine(ZoomInMap());
	}

	private IEnumerator ZoomInMap()
	{
		while (Map.Zoom < 16)
		{
			Map.UpdateMap(Mathf.Min(16, Map.Zoom + Time.deltaTime * Speed));
			yield return null;
		}
	}

	[ContextMenu("Zoom Out")]
	public void ZoomOut()
	{
		StartCoroutine(ZoomOutMap());
	}

	private IEnumerator ZoomOutMap()
	{
		while (Map.Zoom > 2)
		{
			Map.UpdateMap(Mathf.Max(2, Map.Zoom - Time.deltaTime * Speed));
			yield return null;
		}
	}

	[ContextMenu("Clear Tiles")]
	public void ClearTiles()
	{
		Map.ClearPreloadedTiles();
	}
}