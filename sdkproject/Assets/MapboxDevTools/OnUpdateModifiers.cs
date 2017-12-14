using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnUpdateModifiers : MonoBehaviour {

	private UnityEvent m_onUpdateModifiers = new UnityEvent();

	public void AddListener(UnityAction action)
	{
		m_onUpdateModifiers.AddListener(action);
	}

	private void OnDestroy()
	{
		m_onUpdateModifiers.RemoveAllListeners();
	}
	// Update is called once per frame
	private void Update () 
	{
		if(m_onUpdateModifiers != null)
		{
			m_onUpdateModifiers.Invoke();
		}		
	}
}
