using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class StringsCollection : AccessorBase
	{
		public ReadOnlyCollection<StringID> list { get; private set; }

		public StringsCollection()
		{
			list = new ReadOnlyCollection<StringID>(new List<StringID>());
		}

		public void Init(Manager.MaiStudio.Serialize.StringsCollection sz)
		{
			List<StringID> list = new List<StringID>();
			foreach (Manager.MaiStudio.Serialize.StringID item in sz.list)
			{
				list.Add((StringID)item);
			}
			this.list = new ReadOnlyCollection<StringID>(list);
		}
	}
}
