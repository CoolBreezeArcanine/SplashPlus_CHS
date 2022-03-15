using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class Version : SerializeBase
	{
		public int major;

		public int minor;

		public int release;

		public Version()
		{
			major = 0;
			minor = 0;
			release = 0;
		}

		public static explicit operator Manager.MaiStudio.Version(Version sz)
		{
			Manager.MaiStudio.Version version = new Manager.MaiStudio.Version();
			version.Init(sz);
			return version;
		}

		public override void AddPath(string parentPath)
		{
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
