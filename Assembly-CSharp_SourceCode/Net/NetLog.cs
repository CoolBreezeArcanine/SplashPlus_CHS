using System;
using System.IO;
using UnityEngine;

namespace Net
{
	public class NetLog : MonoBehaviour
	{
		private static NetLog _instance;

		private StreamWriter _writer;

		public static void OpenNetLog()
		{
			if (_instance == null)
			{
				_instance = new GameObject("NetLog").AddComponent<NetLog>();
			}
		}

		public static void CloseNetLog()
		{
			_instance._writer?.Flush();
			_instance._writer?.Close();
		}

		public static void Log(string log)
		{
			if (_instance != null)
			{
				string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				_instance._writer.WriteLine("[" + text + "]" + log);
			}
		}

		private void Start()
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
			string text2 = Application.dataPath + "/../NetLog/";
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			_writer = new StreamWriter(text2 + text + ".log", append: false);
		}

		private void OnApplicationQuit()
		{
			CloseNetLog();
		}
	}
}
