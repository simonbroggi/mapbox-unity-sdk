using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Vector3Event : UnityEvent<Vector3>
{
}

public class OnTransformHasChanged : MonoBehaviour 
{
	private Vector3Event m_onTransformHasChanged = new Vector3Event();

	public void AddListener(UnityAction<Vector3> action )
	{
		m_onTransformHasChanged.AddListener(action);
	}
	
	public void RemoveListener(UnityAction<Vector3>  action )
	{
		m_onTransformHasChanged.RemoveListener(action);
	}

	private void OnDestroy()
	{
		m_onTransformHasChanged.RemoveAllListeners();
	}

	// Update is called once per frame
	void Update () 
	{
		if(transform.hasChanged)
		{
			if(m_onTransformHasChanged != null)
			{
				m_onTransformHasChanged.Invoke(transform.position);
			}
			transform.hasChanged = false;
		}
	}
}
