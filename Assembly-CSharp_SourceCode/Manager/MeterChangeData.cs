namespace Manager
{
	public class MeterChangeData : TimingBase
	{
		public uint denomi;

		public uint num;

		public MeterChangeData()
		{
			init();
		}

		public new void init()
		{
			base.init();
			denomi = 4u;
			num = 4u;
		}
	}
}
