using System;
using System.Text;
using MAI2.Util;
using UnityEngine;

namespace MAI2System
{
	public class SystemConfig : Singleton<SystemConfig>
	{
		private Config _config = new Config();

		private StringBuilder _stringBuilder = new StringBuilder();

		public Config config => _config;

		public StringBuilder getStringBuilder()
		{
			_stringBuilder.Length = 0;
			return _stringBuilder;
		}

		public static void unloadUnusedAndGC()
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		public void initialize()
		{
			_config.initialize();
		}

		public void initializeAfterAMDaemonReady()
		{
			_config.initializeAfterAMDaemonReady();
		}

		public void terminate()
		{
		}

		public void execute()
		{
		}
	}
}
