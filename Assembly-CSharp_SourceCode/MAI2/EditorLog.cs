using System.Diagnostics;
using UnityEngine;

namespace MAI2
{
	public static class EditorLog
	{
		[Conditional("UNITY_EDITOR")]
		public static void log(string msg)
		{
			UnityEngine.Debug.Log(msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void logFormat(string msg, params object[] args)
		{
			UnityEngine.Debug.LogFormat(msg, args);
		}

		[Conditional("UNITY_EDITOR")]
		public static void logWarning(string msg)
		{
			UnityEngine.Debug.LogWarning(msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void logWarningFormat(string msg, params object[] args)
		{
			UnityEngine.Debug.LogWarningFormat(msg, args);
		}

		[Conditional("UNITY_EDITOR")]
		public static void logError(string msg)
		{
			UnityEngine.Debug.LogError(msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void logErrorFormat(string msg, params object[] args)
		{
			UnityEngine.Debug.LogErrorFormat(msg, args);
		}
	}
}
