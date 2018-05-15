using UnityEngine;
using System.Collections;
using UnityEditor;
[ExecuteInEditMode]
public class Screenshot : MonoBehaviour {


	public static string screenshotPath = "C:\\UnityScreenshots\\screenshot.png";

	[MenuItem("Utils/Create Screenshot")]
	public static void CreateScreenshot()
	{
		ScreenCapture.CaptureScreenshot(screenshotPath);
	}
}
