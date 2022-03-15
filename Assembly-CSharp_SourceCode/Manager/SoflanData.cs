namespace Manager
{
	public class SoflanData : TimingBase
	{
		public float speed;

		public NotesTime end;

		public SoflanData()
		{
			init();
		}

		public new void init()
		{
			base.init();
			speed = 1f;
			end.clear();
		}

		public int compare(float msecTarget)
		{
			if (msecTarget < time.msec)
			{
				return -1;
			}
			if (msecTarget < end.msec)
			{
				return 0;
			}
			return 1;
		}
	}
}
