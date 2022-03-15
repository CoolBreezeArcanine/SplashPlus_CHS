using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MAI2
{
	public static class LogCallback
	{
		private class LogSender : MonoBehaviour
		{
			public struct Log
			{
				public DateTime localTime_;

				public string message_;

				public string stackTrace_;

				public int level_;

				public string ip_;

				public byte[] bytes_;
			}

			public const int State_Init = 0;

			public const int State_Proc = 1;

			private int state_;

			private float time_;

			private List<Log> checkSame_ = new List<Log>(8);

			private List<Log> logs_ = new List<Log>();

			private UnityWebRequest uwr_;

			public static void create(string message, string stackTrace, int level, string ip)
			{
				GameObject gameObject = GameObject.Find("/LogSender");
				LogSender logSender;
				if (null != gameObject)
				{
					logSender = gameObject.GetComponent<LogSender>();
				}
				else
				{
					gameObject = new GameObject("LogSender");
					logSender = gameObject.AddComponent<LogSender>();
				}
				logSender.push(message, stackTrace, level, ip);
			}

			private void push(string message, string stackTrace, int level, string ip)
			{
				DateTime dateTime = DateTime.UtcNow.ToLocalTime();
				while (8 < checkSame_.Count)
				{
					checkSame_.RemoveAt(0);
				}
				for (int i = 0; i < checkSame_.Count; i++)
				{
					double totalSeconds = (dateTime - checkSame_[i].localTime_).TotalSeconds;
					if (checkSame_[i].message_ == message && totalSeconds <= 60.0)
					{
						return;
					}
				}
				StartCoroutine(pushImpl(dateTime, message, stackTrace, level, ip));
			}

			private IEnumerator pushImpl(DateTime localTime, string message, string stackTrace, int level, string ip)
			{
				yield return new WaitForEndOfFrame();
				Texture2D texture2D = new Texture2D(Screen.width, Screen.height);
				texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
				texture2D.Apply();
				Texture2D texture2D2 = new Texture2D(Screen.width >> 1, Screen.height >> 1);
				for (int i = 0; i < texture2D2.height; i++)
				{
					float y = Mathf.Clamp01((float)i / (float)texture2D2.height);
					for (int j = 0; j < texture2D2.width; j++)
					{
						float x = Mathf.Clamp01((float)j / (float)texture2D2.width);
						texture2D2.SetPixel(j, i, texture2D.GetPixelBilinear(x, y));
					}
				}
				texture2D2.Apply();
				byte[] bytes_ = texture2D2.EncodeToJPG();
				Log item = default(Log);
				item.localTime_ = localTime;
				item.message_ = message;
				item.stackTrace_ = stackTrace;
				item.level_ = level;
				item.ip_ = ip;
				item.bytes_ = bytes_;
				checkSame_.Add(item);
				logs_.Add(item);
			}

			private void Awake()
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}

			private void Update()
			{
				switch (state_)
				{
				case 0:
					if (0 < logs_.Count)
					{
						time_ = 0f;
						Log log = logs_[0];
						logs_.RemoveAt(0);
						WWWForm wWWForm = new WWWForm();
						wWWForm.AddField("date", log.localTime_.ToString());
						wWWForm.AddField("message", log.message_);
						wWWForm.AddField("stack", log.stackTrace_);
						wWWForm.AddField("level", log.level_);
						wWWForm.AddField("ip", log.ip_);
						wWWForm.AddBinaryData("image", log.bytes_, "ss.jpg");
						uwr_ = UnityWebRequest.Post(LogSendAddress, wWWForm);
						state_ = 1;
					}
					else
					{
						time_ += Time.deltaTime;
						if (20f < time_)
						{
							UnityEngine.Object.Destroy(base.gameObject);
						}
					}
					break;
				case 1:
					if (uwr_ == null || uwr_.isDone)
					{
						if (uwr_ != null)
						{
							uwr_.Dispose();
							uwr_ = null;
						}
						state_ = 0;
					}
					break;
				}
			}
		}

		public const int LogLevel_Log = 0;

		public const int LogLevel_Warning = 1;

		public const int LogLevel_Assert = 2;

		public const int LogLevel_Error = 3;

		public const int LogLevel_Exception = 4;

		private static string LogSendAddress = string.Empty;

		private static int LogSendLevel = 2;

		private static string LogLocalIP = string.Empty;

		public static void initialize(string logSendAddress, int logSendLevel, string logLocalAddress)
		{
			LogSendAddress = logSendAddress;
			LogSendLevel = Mathf.Clamp(logSendLevel, 0, 4);
			LogLocalIP = logLocalAddress;
			Application.logMessageReceived += logCallback;
		}

		private static void logCallback(string logString, string stackTrace, LogType type)
		{
			if (string.IsNullOrEmpty(LogSendAddress))
			{
				return;
			}
			switch (type)
			{
			case LogType.Error:
				if (LogSendLevel <= 3)
				{
					LogSender.create(logString, stackTrace, 3, LogLocalIP);
				}
				break;
			case LogType.Assert:
				if (LogSendLevel <= 2)
				{
					LogSender.create(logString, stackTrace, 2, LogLocalIP);
				}
				break;
			case LogType.Warning:
				if (LogSendLevel <= 1)
				{
					LogSender.create(logString, stackTrace, 1, LogLocalIP);
				}
				break;
			case LogType.Log:
				if (LogSendLevel <= 0)
				{
					LogSender.create(logString, stackTrace, 0, LogLocalIP);
				}
				break;
			case LogType.Exception:
				if (LogSendLevel <= 4)
				{
					LogSender.create(logString, stackTrace, 4, LogLocalIP);
				}
				break;
			}
		}
	}
}
