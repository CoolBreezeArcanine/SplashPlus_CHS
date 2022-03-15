using System.Collections.Generic;
using System.Linq;

namespace Manager
{
	public class BPMChangeDataList : List<BPMChangeData>
	{
		public bool Empty()
		{
			return !this.Any();
		}
	}
}
