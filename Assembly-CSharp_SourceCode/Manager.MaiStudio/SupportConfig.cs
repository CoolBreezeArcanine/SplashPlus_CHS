using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class SupportConfig : AccessorBase
	{
		public SupportConfigMode mode { get; private set; }

		public bool window { get; private set; }

		public SupportConfig()
		{
			mode = SupportConfigMode.Target;
			window = false;
		}

		public void Init(Manager.MaiStudio.Serialize.SupportConfig sz)
		{
			mode = sz.mode;
			window = sz.window;
		}
	}
}
