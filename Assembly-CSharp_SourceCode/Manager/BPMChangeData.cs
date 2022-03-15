namespace Manager
{
	public class BPMChangeData : TimingBase
	{
		public float bpm;

		public BPMChangeData()
		{
			init();
		}

		public new void init()
		{
			base.init();
			bpm = 150f;
		}
	}
}
