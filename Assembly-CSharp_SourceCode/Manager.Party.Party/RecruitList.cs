using System.Collections.Generic;

namespace Manager.Party.Party
{
	public class RecruitList : List<RecruitInfo>
	{
		public RecruitList()
		{
		}

		public RecruitList(IEnumerable<RecruitInfo> list)
			: base(list)
		{
		}

		public override string ToString()
		{
			string text = "RecruitList {\n";
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RecruitInfo current = enumerator.Current;
					text += $"{current}\n";
				}
			}
			return text + "}\n";
		}
	}
}
