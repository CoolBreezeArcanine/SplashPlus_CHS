using System.Collections.Generic;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class Version : AccessorBase
	{
		public int major { get; private set; }

		public int minor { get; private set; }

		public int release { get; private set; }

		public Version()
		{
			major = 0;
			minor = 0;
			release = 0;
		}

		public void Init(Manager.MaiStudio.Serialize.Version sz)
		{
			major = sz.major;
			minor = sz.minor;
			release = sz.release;
		}

		public void Set(int ma, int mi, int re)
		{
			major = ma;
			minor = mi;
			release = re;
		}

		public void Set(string verStr)
		{
			string[] array = verStr.Split('.');
			if (3 != array.Length)
			{
				return;
			}
			List<int> list = new List<int>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (int.TryParse(array2[i], out var result))
				{
					list.Add(result);
				}
			}
			if (3 == list.Count)
			{
				Set(list[0], list[1], list[2]);
			}
		}

		public string GetStr()
		{
			return major + "." + minor + "." + release;
		}

		public uint GetCode()
		{
			return (uint)(0 + major * 1000 * 1000 + minor * 1000 + release);
		}
	}
}
