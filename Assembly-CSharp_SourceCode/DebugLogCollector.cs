using System;
using System.IO;
using UnityEngine;

public class DebugLogCollector
{
	private static int FrameCount;

	private const string CounterTextName = "CounterText.txt";

	private const string TextName = "DebugLog.txt";

	private string _log;

	private static DebugLogCollector _instance;

	public static DebugLogCollector Instance
	{
		get
		{
			if (_instance == null)
			{
				return _instance = new DebugLogCollector();
			}
			return _instance;
		}
	}

	public int LoadCounter()
	{
		FrameCount = int.Parse(File.ReadAllLines(Application.dataPath + "/CounterText.txt")[0]);
		return FrameCount;
	}

	public void AddLog(string message)
	{
		_log = _log + message + "\n";
	}

	public void SaveLog()
	{
		StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/DebugLog.txt", append: true);
		string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		try
		{
			streamWriter.WriteLine("=======" + text + "=======現在のフレーム間隔" + FrameCount + "\n\n" + _log + "==============");
		}
		finally
		{
			streamWriter.Close();
		}
	}

	private void SaveFrameCounter()
	{
		FrameCount++;
		StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/CounterText.txt", append: false);
		try
		{
			streamWriter.WriteLine(FrameCount);
		}
		finally
		{
			streamWriter.Close();
		}
	}
}
