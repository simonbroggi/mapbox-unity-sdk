using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mapbox.Examples
{
	public class DragableDirectionWaypoint : MonoBehaviour
	{
		public Transform MoveTarget;
		private Vector3 screenPoint;
		private Vector3 offset;
		private Plane _yPlane;

		private bool _dragging = false;

		public void Start()
		{
			_yPlane = new Plane(Vector3.up, Vector3.zero);
		}

		void OnMouseDrag()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float enter = 0.0f;
			if (_yPlane.Raycast(ray, out enter))
			{
				MoveTarget.position = ray.GetPoint(enter);
			}

			_dragging = true;
		}

		private void OnMouseUp()
		{
			if (_dragging)
			{
				OnPinDropped(transform.position);
			}
		}

		public delegate void PinDroppedEventHandler(object sender, PinEventArgs e);
		public event PinDroppedEventHandler PinDropped;

		private void OnPinDropped(Vector3 pos)
		{
			if (PinDropped == null) return;

			var args = new PinEventArgs(pos);
			PinDropped(this, args);
		}
	}

	public class PinEventArgs : EventArgs
	{
		public Vector3 Position { get; private set; }

		public PinEventArgs(Vector3 pos)
		{
			Position = pos;
		}
	}
}
