using System.Collections.Generic;
using System.Linq;

namespace Manager
{
	public class BarDataList : List<BarData>
	{
		public bool Empty()
		{
			return !this.Any();
		}
	}
}
