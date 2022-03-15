using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class StringID : SerializeBase
	{
		public const int InvalidID = -1;

		public int id;

		public string str;

		public StringID()
		{
			Clear();
		}

		public static explicit operator Manager.MaiStudio.StringID(StringID sz)
		{
			Manager.MaiStudio.StringID stringID = new Manager.MaiStudio.StringID();
			stringID.Init(sz);
			return stringID;
		}

		public override void AddPath(string parentPath)
		{
		}

		private void Clear()
		{
			id = -1;
			str = "";
		}

		public bool IsValid()
		{
			bool result = false;
			if (id > -1)
			{
				result = true;
			}
			return result;
		}
	}
}
