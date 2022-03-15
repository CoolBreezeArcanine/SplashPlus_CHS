using System.Collections.Generic;
using System.Linq;

namespace Manager
{
	public class SlideDataList : List<SlideData>
	{
		public uint getSlideNodeNum()
		{
			uint num = 0u;
			using Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				_ = enumerator.Current;
				num++;
			}
			return num;
		}

		public bool Empty()
		{
			return !this.Any();
		}
	}
}
