namespace Manager
{
	public class TimingBase
	{
		public NotesTime time;

		public int index;

		public TimingBase(TimingBase tb)
		{
			time = tb.time;
			index = tb.index;
		}

		public TimingBase()
		{
			init();
		}

		public void init()
		{
			time.clear();
			index = 0;
		}

		public static bool compareByTime(TimingBase left, TimingBase right)
		{
			return left.time < right.time;
		}
	}
}
