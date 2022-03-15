using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace MAI2
{
	public static class Log
	{
		public delegate void OnResultTakeScreenShot(Texture2D ss);

		[Conditional("APP_DEBUG")]
		public static void log(string msg)
		{
			UnityEngine.Debug.Log(msg);
		}

		[Conditional("APP_DEBUG")]
		public static void logFormat(string msg, params object[] args)
		{
			UnityEngine.Debug.LogFormat(msg, args);
		}

		[Conditional("APP_DEBUG")]
		public static void logWarning(string msg)
		{
			UnityEngine.Debug.LogWarning(msg);
		}

		[Conditional("APP_DEBUG")]
		public static void logWarningFormat(string msg, params object[] args)
		{
			UnityEngine.Debug.LogWarningFormat(msg, args);
		}

		[Conditional("APP_DEBUG")]
		public static void logError(string msg)
		{
			UnityEngine.Debug.LogError(msg);
		}

		[Conditional("APP_DEBUG")]
		public static void logErrorFormat(string msg, params object[] args)
		{
			UnityEngine.Debug.LogErrorFormat(msg, args);
		}

		public static IEnumerator takeScreenShot(OnResultTakeScreenShot onResult)
		{
			yield return new WaitForEndOfFrame();
			Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);
			texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			texture2D.Apply();
			onResult?.Invoke(texture2D);
		}

		public static IEnumerator takeScreenShot(string finename, bool waitNextFrame)
		{
			if (waitNextFrame)
			{
				yield return null;
			}
			yield return new WaitForEndOfFrame();
			Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);
			texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			texture2D.Apply();
			string path = Application.dataPath + "/../" + finename;
			byte[] bytes = texture2D.EncodeToPNG();
			try
			{
				File.WriteAllBytes(path, bytes);
			}
			catch
			{
			}
		}

		public static IEnumerator takeCrashScreenShot(string finename)
		{
			yield return new WaitForEndOfFrame();
			Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);
			texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			texture2D.Apply();
			byte[] bytes = texture2D.EncodeToPNG();
			try
			{
				File.WriteAllBytes(finename, bytes);
			}
			catch
			{
			}
		}
	}
}
