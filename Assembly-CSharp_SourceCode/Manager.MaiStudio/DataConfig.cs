using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class DataConfig : AccessorBase
	{
		public Version version { get; private set; }

		public Version cardMakerVersion { get; private set; }

		public DataConfig()
		{
			version = new Version();
			cardMakerVersion = new Version();
		}

		public void Init(Manager.MaiStudio.Serialize.DataConfig sz)
		{
			version = (Version)sz.version;
			cardMakerVersion = (Version)sz.cardMakerVersion;
		}
	}
}
