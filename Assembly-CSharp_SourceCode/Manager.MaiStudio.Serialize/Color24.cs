using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class Color24 : SerializeBase
	{
		public byte R;

		public byte G;

		public byte B;

		public Color24()
		{
			R = 0;
			G = 0;
			B = 0;
		}

		public static explicit operator Manager.MaiStudio.Color24(Color24 sz)
		{
			Manager.MaiStudio.Color24 color = new Manager.MaiStudio.Color24();
			color.Init(sz);
			return color;
		}

		public override void AddPath(string parentPath)
		{
		}
	}
}
