namespace Monitor.ModeSelect
{
	public class CommonValue
	{
		public float start;

		public float current;

		public float end;

		public float diff;

		public CommonValue()
		{
			start = 0f;
			current = 0f;
			end = 0f;
			diff = 0f;
		}

		public bool UpdateValue()
		{
			bool result = false;
			if (diff == 0f)
			{
				return true;
			}
			if (diff > 0f)
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
