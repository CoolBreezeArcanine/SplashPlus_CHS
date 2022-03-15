namespace Manager
{
	public class SlideData : TimingBase
	{
		public SlideType type;

		public int targetNote;

		public TimingBase shoot = new TimingBase();

		public TimingBase arrive = new TimingBase();

		public SlideData()
		{
			init();
		}

		public new void init()
		{
			base.init();
			time.clear();
			type = SlideType.Slide_None;
			targetNote = 0;
			shoot.init();
			shoot.index = 0;
			arrive.init();
			arrive.index = 0;
		}
	}
}
