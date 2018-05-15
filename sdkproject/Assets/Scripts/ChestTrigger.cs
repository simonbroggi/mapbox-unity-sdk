using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTrigger : MonoBehaviour {

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag=="Player")
		{
			Debug.Log("Open Chest");
			GetComponent<Animator>().Play("Open");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.tag=="Player")
		{
			Debug.Log("Close Chest");
			GetComponent<Animator>().Play("Close");
		}
	}
}
