using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class RomConfig : SerializeBase
	{
		public string projectName;

		public Version version;

		public string region;

		public RomConfig()
		{
			projectName = "";
			version = new Version();
			region = "";
		}

		public static explicit operator Manager.MaiStudio.RomConfig(RomConfig sz)
		{
			Manager.MaiStudio.RomConfig romConfig = new Manager.MaiStudio.RomConfig();
			romConfig.Init(sz);
			return romConfig;
		}

		public override void AddPath(string parentPath)
		{
			version.AddPath(parentPath);
		}
	}
}
