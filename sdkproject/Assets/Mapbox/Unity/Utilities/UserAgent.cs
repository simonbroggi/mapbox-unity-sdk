using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



namespace Mapbox.Unity.Utilities
{
	public static class UserAgent
	{

		public static string GetUserAgent()
		{
			var userAgent = string.Format(
				"{0}/{1}/{2} MapboxEventsUnity{3}/{4}"
				, Application.identifier
				, Application.version
				, "0"
				, Application.platform
				, Constants.SDK_VERSION
			);
			return userAgent;
		}


		public static string GetUserAgentEditor()
		{
			var userAgent = string.Format(
				"{0}/{1}/{2} MapboxEventsUnityEditor/{3}",
				PlayerSettings.applicationIdentifier,
				PlayerSettings.bundleVersion,
#if UNITY_IOS
				PlayerSettings.iOS.buildNumber,
#elif UNITY_ANDROID
				PlayerSettings.Android.bundleVersionCode,
#else
				 "0",
#endif
				 Constants.SDK_VERSION
			);
			return userAgent;
		}
	}
}
