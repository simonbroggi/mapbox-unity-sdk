using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionSimulator : MonoBehaviour {

	public MapImageTarget _simulatedTarget;

	// Use this for initialization
	void OnEnable () {

		Invoke("SimulateDetection", 2f);
		
	}

	void SimulateDetection()
	{
		_simulatedTarget.AnchorDetected(transform.position, transform.rotation);
	}

}
