namespace Manager
{
	public class BPMInfo
	{
		public float defaultBPM;

		public float minBPM;

		public float maxBPM;

		public float firstBPM;

		public BPMInfo()
		{
			init();
		}

		public void init()
		{
			defaultBPM = 150f;
			minBPM = 150f;
			maxBPM = 150f;
			firstBPM = 150f;
		}
	}
}
