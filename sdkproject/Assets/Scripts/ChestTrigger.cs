using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTrigger : MonoBehaviour {

	private string[] particles = { "SparksDot", "SparksMain", "SparksStar", "model" };
	private void OnTriggerEnter(Collider other)
	{
		if(other.tag=="Player")
		{
			Debug.Log("Open Chest");
			GetComponent<Animator>().Play("Open");
			StopAllCoroutines();
			StartCoroutine(OpenChest());
		}
	}

	private IEnumerator OpenChest()
	{
		yield return new WaitForSeconds(1f);
		foreach (var particle in particles)
		{
			transform.Find(particle).gameObject.SetActive(true);
			yield return null;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.tag=="Player")
		{
			Debug.Log("Closed Chest");
			GetComponent<Animator>().Play("Closed");
			foreach (var particle in particles)
			{
				transform.Find(particle).gameObject.SetActive(false);
			}
		}
	}
}
