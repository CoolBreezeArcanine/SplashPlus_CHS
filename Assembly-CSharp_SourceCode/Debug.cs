using System;
using System.Diagnostics;
using UnityEngine;

public static class Debug
{
	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, string message, UnityEngine.Object context)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, object message, UnityEngine.Object context)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, string message)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, object message)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, UnityEngine.Object context)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void AssertFormat(bool condition, string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void AssertFormat(bool condition, UnityEngine.Object context, string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Break()
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void ClearDeveloperConsole()
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DebugBreak()
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Log(object message)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Log(object message, UnityEngine.Object context)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogAssertion(object message, UnityEngine.Object context)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogAssertion(object message)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogAssertionFormat(string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogError(object message, UnityEngine.Object context)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogError(object message)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogErrorFormat(string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogException(Exception exception, UnityEngine.Object context)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogException(Exception exception)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogFormat(string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogWarning(object message)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogWarning(object message, UnityEngine.Object context)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogWarningFormat(string format, params object[] args)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
	}
}
