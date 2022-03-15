using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class SerializeSortData : AccessorBase
	{
		public ReadOnlyCollection<StringID> SortList { get; private set; }

		public string dataName { get; private set; }

		public SerializeSortData()
		{
			SortList = new ReadOnlyCollection<StringID>(new List<StringID>());
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.SerializeSortData sz)
		{
			List<StringID> list = new List<StringID>();
			foreach (Manager.MaiStudio.Serialize.StringID sort in sz.SortList)
			{
				list.Add((StringID)sort);
			}
			SortList = new ReadOnlyCollection<StringID>(list);
			dataName = sz.dataName;
		}
	}
}
