namespace Manager
{
	public class ClickData : TimingBase
	{
		public bool Played;

		public ClickData()
		{
			init();
		}

		public new void init()
		{
			base.init();
			time.clear();
			Played = false;
		}
	}
}
