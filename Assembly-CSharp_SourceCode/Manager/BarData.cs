namespace Manager
{
	public class BarData : TimingBase
	{
		public uint numTotal;

		public uint numBar;

		public BarData()
		{
			init();
		}

		public new void init()
		{
			base.init();
			numTotal = 0u;
			numBar = 0u;
			time.clear();
		}
	}
}
