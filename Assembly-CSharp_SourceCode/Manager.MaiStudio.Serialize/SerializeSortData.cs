using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class SerializeSortData : SerializeBase
	{
		public List<StringID> SortList;

		public string dataName;

		public SerializeSortData()
		{
			SortList = new List<StringID>();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.SerializeSortData(SerializeSortData sz)
		{
			Manager.MaiStudio.SerializeSortData serializeSortData = new Manager.MaiStudio.SerializeSortData();
			serializeSortData.Init(sz);
			return serializeSortData;
		}

		public override void AddPath(string parentPath)
		{
			foreach (StringID sort in SortList)
			{
				sort.AddPath(parentPath);
			}
		}
	}
}
