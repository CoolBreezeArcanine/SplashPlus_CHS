namespace Monitor.TicketSelect
{
	public class CommonValueUint
	{
		public uint start;

		public uint current;

		public uint end;

		public uint diff;

		public bool sub;

		public CommonValueUint()
		{
			start = 0u;
			current = 0u;
			end = 0u;
			diff = 0u;
			sub = false;
		}

		public bool UpdateValue()
		{
			bool result = false;
			if (diff == 0)
			{
				return true;
			}
			if (!sub)
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
				if (current >= diff)
				{
					current -= diff;
				}
				else
				{
					current = 0u;
				}
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
