using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class SupportConfig : SerializeBase
	{
		public SupportConfigMode mode;

		public bool window;

		public SupportConfig()
		{
			mode = SupportConfigMode.Target;
			window = false;
		}

		public static explicit operator Manager.MaiStudio.SupportConfig(SupportConfig sz)
		{
			Manager.MaiStudio.SupportConfig supportConfig = new Manager.MaiStudio.SupportConfig();
			supportConfig.Init(sz);
			return supportConfig;
		}

		public override void AddPath(string parentPath)
		{
		}
	}
}
