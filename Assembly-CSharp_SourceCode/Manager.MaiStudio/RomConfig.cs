using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class RomConfig : AccessorBase
	{
		public string projectName { get; private set; }

		public Version version { get; private set; }

		public string region { get; private set; }

		public RomConfig()
		{
			projectName = "";
			version = new Version();
			region = "";
		}

		public void Init(Manager.MaiStudio.Serialize.RomConfig sz)
		{
			projectName = sz.projectName;
			version = (Version)sz.version;
			region = sz.region;
		}
	}
}
