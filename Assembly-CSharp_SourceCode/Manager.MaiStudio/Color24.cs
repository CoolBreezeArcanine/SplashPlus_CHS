using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class Color24 : AccessorBase
	{
		public byte R { get; private set; }

		public byte G { get; private set; }

		public byte B { get; private set; }

		public Color24()
		{
			R = 0;
			G = 0;
			B = 0;
		}

		public void Init(Manager.MaiStudio.Serialize.Color24 sz)
		{
			R = sz.R;
			G = sz.G;
			B = sz.B;
		}
	}
}
