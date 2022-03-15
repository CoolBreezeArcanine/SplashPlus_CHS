using System.Collections.Generic;
using System.Linq;

namespace Manager
{
	public class ClickDataList : List<ClickData>
	{
		public bool Empty()
		{
			return !this.Any();
		}
	}
}
