using System.Collections.Generic;
using System.Linq;

namespace Manager
{
	public class TimeRangeList : List<TimeRange>
	{
		public bool Empty()
		{
			return !this.Any();
		}

		public TimeRange getRange(NotesTime time)
		{
			if (Empty())
			{
				return null;
			}
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TimeRange current = enumerator.Current;
					if (current.isIn(time))
					{
						return current;
					}
				}
			}
			return null;
		}

		public bool isIn(NotesTime time)
		{
			return getRange(time) != null;
		}

		public void addRange(TimeRange zone)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.merge(zone))
					{
						return;
					}
				}
			}
			Add(zone);
		}
	}
}
