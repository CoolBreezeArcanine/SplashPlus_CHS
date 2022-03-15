using System.Collections.Generic;
using System.IO;
using MAI2.Util;
using UnityEngine;

namespace MAI2System
{
	public static class Path
	{
		private static string _binPath;

		private static string _firmPath;

		private static string _logPath;

		private static string _photoPath;

		private static string _uploadPath;

		private const string BinDataPath = "bin";

		private const string FirmDataPath = "firm";

		private const string LogDataPath = "Errorlog";

		private const string CaptureDataPath = "FaceIconCapture";

		private const string UploadDataPath = "UploadData";

		public const int LogCount = 5;

		public static string BinPath
		{
			get
			{
				if (string.IsNullOrEmpty(_binPath))
				{
					if (Singleton<SystemConfig>.Instance.config.IsTarget)
					{
						_binPath = "Y:/SDEZ/bin/";
					}
					else
					{
						_binPath = Application.dataPath + "/../bin/";
					}
				}
				return _binPath;
			}
		}

		public static string FirmPath
		{
			get
			{
				if (string.IsNullOrEmpty(_binPath))
				{
					if (Singleton<SystemConfig>.Instance.config.IsTarget)
					{
						_firmPath = "Y:/SDEZ/firm/";
					}
					else
					{
						_firmPath = Application.dataPath + "/../firm/";
					}
				}
				return _firmPath;
			}
		}

		public static string ErrorLogPath
		{
			get
			{
				if (string.IsNullOrEmpty(_logPath))
				{
					if (Singleton<SystemConfig>.Instance.config.IsTarget)
					{
						_logPath = "Y:/SDEZ/Errorlog/";
					}
					else
					{
						_logPath = Application.dataPath + "/../Errorlog/";
					}
				}
				return _logPath;
			}
		}

		public static string PhotoPath
		{
			get
			{
				if (string.IsNullOrEmpty(_photoPath))
				{
					if (Singleton<SystemConfig>.Instance.config.IsTarget)
					{
						_photoPath = "Y:/SDEZ/FaceIconCapture/";
					}
					else
					{
						_photoPath = Application.dataPath + "/../FaceIconCapture/";
					}
				}
				return _photoPath;
			}
		}

		public static string UploadPath
		{
			get
			{
				if (string.IsNullOrEmpty(_uploadPath))
				{
					if (Singleton<SystemConfig>.Instance.config.IsTarget)
					{
						_uploadPath = "Y:/SDEZ/UploadData/";
					}
					else
					{
						_uploadPath = Application.dataPath + "/../UploadData/";
					}
				}
				return _uploadPath;
			}
		}

		public static void CreateFolder()
		{
			string photoPath = PhotoPath;
			if (!Directory.Exists(photoPath))
			{
				Directory.CreateDirectory(photoPath);
			}
			photoPath = UploadPath;
			if (!Directory.Exists(photoPath))
			{
				Directory.CreateDirectory(photoPath);
			}
			photoPath = ErrorLogPath;
			if (!Directory.Exists(photoPath))
			{
				Directory.CreateDirectory(photoPath);
				return;
			}
			DeleteOldErrorFiles(ErrorLogPath, ".log", 5);
			DeleteOldErrorFiles(ErrorLogPath, ".png", 5);
		}

		public static bool DeleteOldErrorFiles(string filepath, string extendStr, int num)
		{
			List<string> errorFiles = GetErrorFiles(filepath, extendStr);
			int count = errorFiles.Count;
			count -= num;
			foreach (string item in errorFiles)
			{
				if (count <= 0)
				{
					break;
				}
				try
				{
					File.Delete(item);
				}
				catch
				{
				}
				count--;
			}
			return true;
		}

		public static List<string> GetErrorFiles(string filepath, string extendStr)
		{
			List<string> list = new List<string>();
			if (!Directory.Exists(filepath))
			{
				return list;
			}
			try
			{
				string[] files = Directory.GetFiles(filepath);
				foreach (string text in files)
				{
					if (text.Contains(extendStr))
					{
						list.Add(text);
					}
				}
				return list;
			}
			catch
			{
				return list;
			}
		}
	}
}
