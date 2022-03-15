namespace Monitor.TicketSelect
{
	public class CommonValueInt
	{
		public int start;

		public int current;

		public int end;

		public int diff;

		public CommonValueInt()
		{
			start = 0;
			current = 0;
			end = 0;
			diff = 0;
		}

		public bool UpdateValue()
		{
			bool result = false;
			if (diff == 0)
			{
				return true;
			}
			if (diff > 0)
			{
				current += diff;
				if (current >= end)
				{
					current = end;
					return true;
				}
			}
			else
			{
				current += diff;
				if (current <= end)
				{
					current = end;
					return true;
				}
			}
			return result;
		}
	}
}
