using System.Collections.Generic;
using System.Linq;

namespace Manager
{
	public class MeterChangeDataList : List<MeterChangeData>
	{
		public bool Empty()
		{
			return !this.Any();
		}
	}
}
