using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class StringsCollection : SerializeBase
	{
		public List<StringID> list;

		public StringsCollection()
		{
			list = new List<StringID>();
		}

		public static explicit operator Manager.MaiStudio.StringsCollection(StringsCollection sz)
		{
			Manager.MaiStudio.StringsCollection stringsCollection = new Manager.MaiStudio.StringsCollection();
			stringsCollection.Init(sz);
			return stringsCollection;
		}

		public override void AddPath(string parentPath)
		{
			foreach (StringID item in list)
			{
				item.AddPath(parentPath);
			}
		}
	}
}
