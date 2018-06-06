namespace Mapbox.Unity.Ar
{
	using System;
	using UnityEngine;
	using Mapbox.Utils;

	public interface ISynchronizationContext
	{
		event Action<Alignment> OnAlignmentAvailable;
		event Action OnAlignmentComplete;
	}

	public struct Alignment
	{
		public Vector3 Position;
		public float Rotation;
		public Vector2d LatLon;
	}
}
