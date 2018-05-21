namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class HeadingSyncBase : MonoBehaviour
	{
		/// <summary>
		/// Returns the average heading of collected headings.
		/// </summary>
		/// <returns>The average heading.</returns>
		public abstract float ReturnAverageHeading();
		/// <summary>
		/// Returns the latest heading value.
		/// </summary>
		/// <returns>The latest heading.</returns>
		public abstract float ReturnLatestHeading();
		/// <summary>
		/// Returns the average offset heading. Takes in consideration the inital Heading value and the offset by ARheading.
		/// </summary>
		/// <returns>The average offset heading.</returns>
		public abstract float ReturnAverageOffsetHeading(float initialHeading, float arHeading);
		/// <summary>
		/// Returns the initial heading of the session.
		/// </summary>
		/// <returns>The initial heading.</returns>
		public abstract float ReturnInitialHeading();
	}
}
