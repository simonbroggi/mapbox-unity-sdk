using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

using Mapbox.Unity.Map;

public class MapImageTarget : MonoBehaviour {

	[SerializeField]
	private ARReferenceImage referenceImage;

	private AbstractMap _parentMap;
	private Vector3 _positionOffsetToMapRoot;
	private Quaternion _rotationOffsetToMapRoot;

	// Use this for initialization
	void Start () {

		_parentMap = GetComponentInParent<AbstractMap>();
		_positionOffsetToMapRoot = _parentMap.Root.position - transform.position;
		_rotationOffsetToMapRoot = Quaternion.Inverse(transform.rotation) * _parentMap.Root.rotation;

		UnityARSessionNativeInterface.ARImageAnchorAddedEvent += AddImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent += UpadteImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorRemovedEvent += RemoveImageAnchor;

	}
	
	void AddImageAnchor(ARImageAnchor arImageAnchor)
	{
		Debug.Log("image anchor added");
		if (arImageAnchor.referenceImageName == referenceImage.imageName)
		{
			Vector3 position = UnityARMatrixOps.GetPosition(arImageAnchor.transform);
			Quaternion rotation = UnityARMatrixOps.GetRotation(arImageAnchor.transform);

			AnchorDetected(position, rotation);
		}
	}

	public void AnchorDetected( Vector3 globalPos, Quaternion globalRot)
	{
		_parentMap.Root.rotation = globalRot * _rotationOffsetToMapRoot;

		var positionOffset = globalPos - transform.position;
		_parentMap.Root.position += positionOffset;
	}

	void UpadteImageAnchor(ARImageAnchor arImageAnchor)
	{
		Debug.Log("image anchor updated");
		if (arImageAnchor.referenceImageName == referenceImage.imageName)
		{
			//re-align map
		}

	}

	void RemoveImageAnchor(ARImageAnchor arImageAnchor)
	{
		Debug.Log("image anchor removed");

	}

	void OnDestroy()
	{
		UnityARSessionNativeInterface.ARImageAnchorAddedEvent -= AddImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent -= UpadteImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorRemovedEvent -= RemoveImageAnchor;

	}
}
