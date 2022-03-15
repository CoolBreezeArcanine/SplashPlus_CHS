using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class DataConfig : SerializeBase
	{
		public Version version;

		public Version cardMakerVersion;

		public DataConfig()
		{
			version = new Version();
			cardMakerVersion = new Version();
		}

		public static explicit operator Manager.MaiStudio.DataConfig(DataConfig sz)
		{
			Manager.MaiStudio.DataConfig dataConfig = new Manager.MaiStudio.DataConfig();
			dataConfig.Init(sz);
			return dataConfig;
		}

		public override void AddPath(string parentPath)
		{
			version.AddPath(parentPath);
			cardMakerVersion.AddPath(parentPath);
		}
	}
}
