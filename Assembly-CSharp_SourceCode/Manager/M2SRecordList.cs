using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Manager
{
	public class M2SRecordList : ReadOnlyCollection<M2SRecord>
	{
		public M2SRecordList(List<M2SRecord> list)
			: base((IList<M2SRecord>)list)
		{
		}
	}
}
