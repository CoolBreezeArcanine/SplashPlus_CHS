using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

[ProjectPrefs("outputlogPathKey", "ログ出力ファイル先", "DebugLog", typeof(string))]
public static class CustomDebug
{
	private const string LOG_PATH = "outputlogPathKey";

	public const string LOG_CLASS_NAME = "";

	private static StringBuilder sb = new StringBuilder();

	private static Dictionary<string, DebugWatchData> watchList = new Dictionary<string, DebugWatchData>();

	public static bool IsWatching { get; private set; }

	[Conditional("APP_DEBUG")]
	public static void Log(object message)
	{
		LogInternal(message, null, Color.white);
	}

	[Conditional("APP_DEBUG")]
	public static void Log(object message, UnityEngine.Object context)
	{
		LogInternal(message, context, Color.white);
	}

	[Conditional("APP_DEBUG")]
	public static void Log(object message, UnityEngine.Object context, Color color)
	{
		LogInternal(message, context, color);
	}

	[Conditional("UNITY_EDITOR")]
	public static void Watch(string keyName, object message)
	{
		if (watchList == null)
		{
			watchList = new Dictionary<string, DebugWatchData>();
		}
		if (watchList.ContainsKey(keyName))
		{
			watchList[keyName].Message = message;
		}
		else
		{
			watchList.Add(keyName, new DebugWatchData
			{
				Key = keyName,
				Message = message,
				StackTrace = StackTraceUtility.ExtractStackTrace()
			});
		}
		IsWatching = true;
	}

	public static int GetWatchCount()
	{
		if (watchList != null)
		{
			return watchList.Count;
		}
		return 0;
	}

	public static string GetWatch()
	{
		if (watchList == null)
		{
			return string.Empty;
		}
		sb.Length = 0;
		foreach (string key in watchList.Keys)
		{
			sb.AppendLine(key + "\t" + watchList[key]);
		}
		IsWatching = false;
		return sb.ToString();
	}

	public static DebugWatchData GetWatchData(int i)
	{
		int num = 0;
		foreach (DebugWatchData value in watchList.Values)
		{
			if (num == i)
			{
				return value;
			}
			num++;
		}
		return null;
	}

	public static void ClearWatchs()
	{
		if (watchList != null)
		{
			watchList.Clear();
			watchList = null;
		}
	}

	private static void LogInternal(object message, UnityEngine.Object context, Color32 color)
	{
		_ = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		DateTime.Now.ToString("HH:mm:ss");
	}

	private static void LogWarning(object message, Color32 color)
	{
		_ = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		DateTime.Now.ToString("HH:mm:ss");
	}

	private static void LogError(object message, Color32 color)
	{
		_ = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		DateTime.Now.ToString("HH:mm:ss");
	}
}
