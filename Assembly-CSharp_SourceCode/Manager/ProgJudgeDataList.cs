using System.Collections.Generic;
using System.Linq;

namespace Manager
{
	public class ProgJudgeDataList : List<ProgJudgeData>
	{
		public uint getEnableJudgeNum()
		{
			uint num = 0u;
			using Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.enable)
				{
					num++;
				}
			}
			return num;
		}

		public bool Empty()
		{
			return !this.Any();
		}
	}
}
